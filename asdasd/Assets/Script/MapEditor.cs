using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        calculatedCellWith = (xTopRight - xBottomLeft) / numberOfCellsInARow;
        calculatedCellHeight = (yTopRight - xBottomLeft) / numberOfCellsInAColumn;
    }

    private void Update()
    {
        HandleClick();
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
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), basicTile);
    }

    public void RemoveTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), clear);
    }

    public void Use(int x, int y)
    {
        Debug.Log(x + " " + y);
        if (add) AddTile(x, y);
        else RemoveTile(x, y);
    }
}
