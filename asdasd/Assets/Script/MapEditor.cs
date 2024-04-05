using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


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
    public GameObject showMenuButton;
    public List<SettingForInteract> infos;
    public GameObject buttonSettingsPrefab;
    public GameObject buttonForCubeSettingsPrefab;
    public GameObject leverSettingsPrefab;
    public GameObject portalSettingsPrefab;
    public Transform settingParentTr;
    public List<int> inverseColor;
    public GameObject inversePair, inversePairParent;
    public List<GameObject> inversePairs;
    public int countInversePair; //because we dont remove from list

    [System.Serializable]
    public struct tool
    {
        public string name;
        public TileBase tile;
    }

    private void Start()
    {
        countInversePair = 0;
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
        OpenMenu();
        if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[2].tile) AddButton(x, y);
        else if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[3].tile) AddButtonForCube(x, y);
        else if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[4].tile) AddLever(x, y);
        else if(tilemaps[currentTilemap].GetTile(new Vector3Int(x, y, 0)) == tools[5].tile) AddPortal(x, y);
    }



    public void OpenMenu()
    {
        menu.SetActive(true);
        showMenuButton.SetActive(true);
    }

    public void AddButton(int x, int y)
    {
        GameObject a = Instantiate(buttonSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddButtonForCube(int x, int y)
    {
        GameObject a = Instantiate(buttonForCubeSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
        a.GetComponent<SettingForInteract>().isButtonsForCube = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddLever(int x, int y)
    {
        GameObject a = Instantiate(leverSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
        a.GetComponent<SettingForInteract>().isLever = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddPortal(int x, int y)
    {
        GameObject a = Instantiate(portalSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps[currentTilemap].color);
        a.GetComponent<SettingForInteract>().isPortal = true;
        infos.Add(a.GetComponent<SettingForInteract>());
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
        InteractiveRemoved(x, y);
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), clear);
    }

    public void InteractiveRemoved(int x, int y)
    {
        for(int i = 0; i < infos.Count; i++)
        {
            if (infos[i].x == x && infos[i].y == y)
            {
                infos[i].CommitSuicide();
                infos.RemoveAt(i);
                break;
            }
        }
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
            Debug.LogWarning(index + " index m�g nem l�tezik, �gy egy m�sikat haszn�lunk");
        }
        else currentTilemap = index;
    }

    public int AddColor(Color color)
    {
        GameObject tm = Instantiate(tilemapPrefab, parentTilemap);
        tilemaps.Add(tm.GetComponent<Tilemap>());
        tm.GetComponent<Tilemap>().color = color;
        inverseColor.Add(-1);
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

        foreach (GameObject pair in inversePairs)
        {
            Component[] buttons = pair.GetComponentsInChildren<InverseButton>();
            foreach (InverseButton b in buttons)
            {
                if (b.index == index)
                {
                    b.GetComponent<Image>().color = color;
                }
            }
        }
    }

    public void CreateInversePair()
    {
        countInversePair++;
        inversePairs.Add(Instantiate(inversePair, inversePairParent.transform));
        inversePairParent.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 80 + countInversePair * 120);
    }
    
    public void GetInfos(Map map)
    {
        map.row = rows;
        map.column = columns;
        map.colorIndex = new int[columns][];
        for (int i = 0; i < columns; i++)
        {
            map.colorIndex[i] = new int[rows];
            for (int j = 0; j < rows; j++) 
            {
                Debug.Log(i + " " + j);
                map.colorIndex[i][j] = -1;
                for (int k = 0; k < tilemaps.Count; k++)
                {
                    if (tilemaps[k].GetTile(new Vector3Int(i, j, 0)) == tools[1].tile)
                    {
                        map.colorIndex[i][j] = k;
                        //Debug.Log(i + " " + (rows - j - 1));
                    }
                }
            }
        }

        map.colors = new MapData.ColorForSave[tilemaps.Count];

        for (int k = 0; k < tilemaps.Count; k++)
        {
            map.colors[k].Set(tilemaps[k].color);
        }

        int buttonForCubeCount = 0;
        int portalCount = 0;
        for (int i = 0; i < infos.Count; i++)
        {
            if (infos[i].isButtonsForCube)
            {
                buttonForCubeCount++;
            }
            else if (infos[i].isPortal)
            {
                portalCount++;
            }
        }

        map.buttonForCubes = new MapData.ButtonForCube[buttonForCubeCount];
        int c = 0;
        int p = 0;
        for (int i = 0; i < infos.Count; i++)
        {
            if (infos[i].isButtonsForCube)
            {
                MapData.ButtonForCube bfc = new MapData.ButtonForCube();
                bfc.color = infos[i].index;
                bfc.x = infos[i].x;
                bfc.y = infos[i].y;
                bfc.interactiveColor = infos[i].indexColorInteract;
                map.buttonForCubes[c++] = bfc;
            }
            else if (infos[i].isPortal)
            {
                MapData.Portal portal = new MapData.Portal();
                portal.color = infos[i].index;
                portal.x = infos[i].x;
                portal.y = infos[i].y;
                portal.interactiveColor = infos[i].indexColorInteract;
                map.portals[p++] = portal;
            }
        }
        Debug.Log(buttonForCubeCount);
    }
}
