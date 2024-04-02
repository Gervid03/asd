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
    public GameObject tilemapPrefab;
    public Transform parentTilemap;
    public int currentTilemap;
    public float calculatedCellWith;
    public float calculatedCellHeight;
    public bool add; //false remove
    public float xBottomLeft, yBottomLeft, xTopRight, yTopRight;
    public int numberOfCellsInARow;
    public int numberOfCellsInAColumn;
    public int currentTool; //0 - remove, 1 - add basic tile, 2 - add button, 3 - add buttonforcube, 4 - add lever, 5 - add portal, 6 select
    public List<tool> tools;
    public GameObject menu;
    public details[][] detail;

    [System.Serializable]
    public struct tool
    {
        public string name;
        public TileBase tile;
    }

    [System.Serializable]
    public struct details
    {
        //it stores with which colors the object will interact
        public int interactColor; //by button which color it activates deactivates, by buttonforcube the color of the cube, by portals the portalindex etc.
        public int activate; //0 if it deactivates, 1 if it activates, -1 if it's not a button

        public void SetButton(int colorIndex, bool itActivates)
        {
            if(itActivates) activate = 1;
            else activate = 0;
            interactColor = colorIndex;
        }

        public void SetButtonForCube(int colorIndex)
        {
            activate = -1;
            interactColor = colorIndex;
        }

        public void SetLever(int colorIndex)
        {
            activate = -1;
            interactColor = colorIndex;
        }

        public void SetPortal(int portalIndex)
        {
            activate = -1;
            interactColor = portalIndex;
        }

    }

    private void Start()
    {
        calculatedCellWith = (xTopRight - xBottomLeft) / numberOfCellsInARow;
        calculatedCellHeight = (yTopRight - xBottomLeft) / numberOfCellsInAColumn;
        detail = new details[numberOfCellsInAColumn][];
        for(int i = 0; i < numberOfCellsInAColumn; i++)
        {
            detail[i] = new details[numberOfCellsInARow];
        }
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
        if (!menu.activeSelf && Input.GetMouseButton(0) && Input.mousePosition.x > xBottomLeft && Input.mousePosition.x < xTopRight && Input.mousePosition.y > yBottomLeft && Input.mousePosition.y < yTopRight)
        {
            Use(Mathf.FloorToInt((Input.mousePosition.x - xBottomLeft) / calculatedCellWith),
                Mathf.FloorToInt((Input.mousePosition.y - yBottomLeft) / calculatedCellHeight));
        }
    }

    public void AddTile(int x, int y)
    {
        RemoveAllTileAtThisPositon(x, y);
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), tools[currentTool].tile);
    }

    public void RemoveAllTileAtThisPositon(int x, int y)
    {
        int a = currentTilemap;
        for(int i = 0; i < tilemaps.Count; i++)
        {
            currentTilemap = i;
            RemoveTile(x, y);
        }
        currentTilemap = a;
    }

    public void RemoveTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), clear);
    }

    public void Use(int x, int y)
    {
        if (currentTool == 0) RemoveTile(x, y);
        //if(currentTool == 6) AddDetails(x, y);
        else AddTile(x, y);
    }

    public void AddDetails(int x, int y)
    {
        //if it's a lever, button, buttonforcube or anything that interacts with other colors it shows the settings
        for(int i = 0; i < tilemaps.Count; i++)
        {
            TileBase tileBase = tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0));
            if (tileBase != null && tileBase != clear)
            {
                if(tileBase != tools[1].tile)
                {

                }
                break;
            }
        }
    }

    public void ChangeColor(int index)
    {
        if (index > tilemaps.Count)
        {
            currentTilemap = index % tilemaps.Count;
            Debug.LogWarning(index + " index még nem létezik, így egy másikat használunk");
        }
        else currentTilemap = index;
    }

    public int AddColor(Color color)
    {
        GameObject tm = Instantiate(tilemapPrefab, parentTilemap);
        tilemaps.Add(tm.GetComponent<Tilemap>());
        tm.GetComponent<Tilemap>().color = color;
        return tilemaps.Count - 1;
    }

    public void RemoveColor(int index) {
        int a = currentTool;
        currentTool = index;
        for(int i = 0; i < numberOfCellsInARow; i++)
        {
            for(int j = 0; j < numberOfCellsInAColumn; j++)
            {
                RemoveTile(i, j);
            }
        }
        if (a == index) ChangeColor(a + 1);
        else ChangeColor(a);
    }

    public void ModifyColor(int index, Color color)
    {
        tilemaps[index].color = color;
    }
}
