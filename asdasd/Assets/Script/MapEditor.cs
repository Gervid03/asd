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
    public TilemapList tilemaps;
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

    [System.Serializable]
    public struct TilemapList
    {
        List<int> indexes;
        List<Tilemap> tilemaps;
        int last;

        public Tilemap at(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    return tilemaps[i];
                }
            }
            return null;
        }

        public void remove(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    tilemaps.RemoveAt(i);
                    indexes.RemoveAt(i);
                    return;
                }
            }
        }

        public int count()
        {
            return tilemaps.Count;
        }

        public void add(Tilemap t, int index)
        {
            if (tilemaps != null) tilemaps.Add(t);
            else Debug.Log("tilemaps doesnt exists");
            if (indexes != null) indexes.Add(index);
            else Debug.Log("indexes doesnt exist");
            last++;
        }

        public int lastIndex()
        {
            return last;
        }

        public List<int> getIndexes()
        {
            return indexes;
        }

        public void makeItNotNull(List<int> a, List<Tilemap> b)
        {
            indexes = a;
            tilemaps = b;
        }
    }

    private void Awake()
    {
        tilemaps.makeItNotNull(new List<int>(), new List<Tilemap>());
        countInversePair = 0;
        calculatedCellWith = (xTopRight - xBottomLeft) / columns;
        calculatedCellHeight = (yTopRight - xBottomLeft) / rows;
    }

    private void Update()
    {
        HandleClick();
        ShortCuts();
    }

    public void ShortCuts()
    {
        if(Input.GetAxisRaw("Remove") == 1) currentTool = 0;
        if(Input.GetAxisRaw("AddBasic") == 1) currentTool = 1;
        if(Input.GetAxisRaw("AddButton") == 1) currentTool = 2;
        if(Input.GetAxisRaw("AddButtonForCube") == 1) currentTool = 3;
        if(Input.GetAxisRaw("AddLever") == 1) currentTool = 4;
        if(Input.GetAxisRaw("AddPortal") == 1) currentTool = 5;
        if(Input.GetAxisRaw("AddGate") == 1) currentTool = 6;
        if(Input.GetAxisRaw("OpenMenu") == 1) OpenMenu();
        if(Input.GetAxisRaw("CloseMenu") == 1) CloseMenu();
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
        if(currentTool > tools.Count) currentTool = 1;
        RemoveAllTileAtThisPositon(x, y);
        if(tilemaps.at(currentTilemap) != null) tilemaps.at(currentTilemap).SetTile(new Vector3Int(x, y, 0), tools[currentTool].tile);
        else Debug.Log(currentTilemap + " doesnt exist");
        if(currentTool != 0 && currentTool != 1 && currentTool != 6)
        {
            InteractiveAdded(x, y);
        }
    }

    public void InteractiveAdded(int x, int y)
    {
        //open menu, add settings
        OpenMenu();
        if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[2].tile) AddButton(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[3].tile) AddButtonForCube(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[4].tile) AddLever(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[5].tile) AddPortal(x, y);
    }



    public void OpenMenu()
    {
        menu.SetActive(true);
        showMenuButton.SetActive(false);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        showMenuButton.SetActive(true);
    }

    public void AddButton(int x, int y)
    {
        GameObject a = Instantiate(buttonSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color);
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddButtonForCube(int x, int y)
    {
        GameObject a = Instantiate(buttonForCubeSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color);
        a.GetComponent<SettingForInteract>().isButtonsForCube = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddLever(int x, int y)
    {
        GameObject a = Instantiate(leverSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color);
        a.GetComponent<SettingForInteract>().isLever = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddPortal(int x, int y)
    {
        GameObject a = Instantiate(portalSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color);
        a.GetComponent<SettingForInteract>().isPortal = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void RemoveAllTileAtThisPositon(int x, int y)
    {
        int a = currentTilemap;
        List<int> indexes = tilemaps.getIndexes();
        for(int i = 0; i < indexes.Count; i++)
        {
            currentTilemap = indexes[i];
            RemoveTile(x, y);
        }
        currentTilemap = a;
    }

    public void RemoveTile(int x, int y)
    {
        InteractiveRemoved(x, y);
        tilemaps.at(currentTilemap).SetTile(new Vector3Int(x, y, 0), clear);
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
        if (index > tilemaps.count())
        {
            Debug.LogError(index + " index még nem létezik, javítsd meg");
        }
        else currentTilemap = index;
    }

    public int AddColor(Color color)
    {
        GameObject tm = Instantiate(tilemapPrefab, parentTilemap);
        int a = tilemaps.lastIndex();
        tilemaps.add(tm.GetComponent<Tilemap>(), a);
        tm.GetComponent<Tilemap>().color = color;
        inverseColor.Add(-1);
        return a;
    }

    public void RemoveColor(int index) {
        int a = currentTilemap;
        currentTilemap = index;
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
        tilemaps.at(index).color = color;

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
                for (int k = 0; k < tilemaps.count(); k++)
                {
                    if (tilemaps.at(k).GetTile(new Vector3Int(i, j, 0)) == tools[1].tile)
                    {
                        map.colorIndex[i][j] = k;
                        //Debug.Log(i + " " + (rows - j - 1));
                    }
                }
            }
        }

        map.colors = new MapData.ColorForSave[tilemaps.count()];
        List<int> indexes = tilemaps.getIndexes();
        for (int k = 0; k < tilemaps.count(); k++)
        {
            map.colors[k].Set(tilemaps.at(indexes[k]).color, indexes[k]);
        }

        int buttonForCubeCount = 0;
        int portalCount = 0;
        int leverCount = 0;
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
            else if (infos[i].isLever)
            {
                leverCount++;
            }
        }

        map.buttonForCubes = new MapData.ButtonForCube[buttonForCubeCount];
        map.portals = new MapData.Portal[portalCount];
        map.lever = new MapData.Lever[leverCount];
        int c = 0;
        int p = 0;
        int l = 0;
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
            else if (infos[i].isLever)
            {
                MapData.Lever lever = new MapData.Lever();
                lever.color = infos[i].index;
                lever.x = infos[i].x;
                lever.y = infos[i].y;
                lever.interactiveColor = infos[i].indexColorInteract;
                map.lever[l++] = lever;
            }
        }
    }
}
