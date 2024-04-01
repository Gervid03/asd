using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class MapEditor : MonoBehaviour
{
    public TileBase basicTile;
    public TileBase clear;
    public List<Tilemap> tilemaps;
    public int currentTilemap;
    public float calculatedCellWith;
    public float calculatedCellHeight;
    public bool add; //false remove
    public float xBottomLeft, yBottomLeft, xTopRight, yTopRight;
    public int numberOfCellsInARow;
    public int numberOfCellsInAColumn;
    public int currentTool; //0 - remove, 1 - add basic tile, 2 - add button, 3 - add buttonforcube, 4 - add lever, 5 - add portal
    public List<tool> tools;

    [System.Serializable]
    public struct tool
    {
        public string name;
        public TileBase tile;
    }

    private void Start()
    {
        calculatedCellWith = (xTopRight - xBottomLeft) / numberOfCellsInARow;
        calculatedCellHeight = (yTopRight - xBottomLeft) / numberOfCellsInAColumn;
    }

    private void Update()
    {
        HandleClick();
    }

    public void SetTool(int index)
    {
        currentTool = index; 
    }

    public void HandleClick()
    {//     If clicked inside the editor board
        if (Input.GetMouseButton(0) && Input.mousePosition.x > xBottomLeft && Input.mousePosition.x < xTopRight && Input.mousePosition.y > yBottomLeft && Input.mousePosition.y < yTopRight)
        {
            Use(Mathf.FloorToInt((Input.mousePosition.x - xBottomLeft) / calculatedCellWith),
                Mathf.FloorToInt((Input.mousePosition.y - yBottomLeft) / calculatedCellHeight));
        }
    }

    public void AddTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), tools[currentTool].tile);
    }

    public void RemoveTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), clear);
    }

    public void Use(int x, int y)
    {
        if (currentTool == 0) RemoveTile(x, y);
        else AddTile(x, y);
    }
}
