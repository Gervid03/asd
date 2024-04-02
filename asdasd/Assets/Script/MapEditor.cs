using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public int columns;
    public int rows;
    public int currentTool; //0 - remove, 1 - add basic tile, 2 - add button, 3 - add buttonforcube, 4 - add lever, 5 - add portal
    public List<tool> tools;
    public GameObject menu;
    public List<SettingForInteract> infos;
    public GameObject buttonSettingsPrefab;
    public GameObject buttonForCubeSettingsPrefab;
    public GameObject LeverSettingsPrefab;
    public GameObject portalSettingsPrefab;
    public Transform settingParentTr;

    [System.Serializable]
    public struct tool
    {
        public string name;
        public TileBase tile;
    }

    private void Start()
    {
        calculatedCellWith = (xTopRight - xBottomLeft) / columns;
        calculatedCellHeight = (yTopRight - xBottomLeft) / rows;
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
        if(currentTool != 0 && currentTool != 1)
        {
            InteractiveAdded(x, y);
        }
    }

    public void InteractiveAdded(int x, int y)
    {
        //open menu, add settings
        if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[2].tile) AddButton(x, y);
        else if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[3].tile) AddButtonForCube(x, y);
        else if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[4].tile) AddLever(x, y);
        else if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[5].tile) AddPortal(x, y);
    }

    public void AddButton(int x, int y)
    {
        GameObject a = Instantiate(buttonSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
    }

    public void AddButtonForCube(int x, int y)
    {
        GameObject a = Instantiate(buttonForCubeSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
    }

    public void AddLever(int x, int y)
    {
        GameObject a = Instantiate(LeverSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
    }

    public void AddPortal(int x, int y)
    {
        GameObject a = Instantiate(portalSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
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

    public void InteractiveRemoved(int x, int y)
    {
        
    }

    public void Use(int x, int y)
    {
        if (currentTool == 0) RemoveTile(x, y);
        else AddTile(x, y);
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
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
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
