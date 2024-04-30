using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
using TMPro;


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
    public int currentTool; //0 - remove, 1 - add basic tile, 2 - add button, 3 - add buttonforcube, 4 - add lever, 5 - add portal, 6 - add gate, 7 - add buttontimercube, 8 - startposition, 9 - endposition
    public List<tool> tools;
    public GameObject menu;
    public GameObject showMenuButton;
    public List<SettingForInteract> infos;
    public GameObject buttonSettingsPrefab;
    public GameObject buttonForCubeSettingsPrefab;
    public GameObject leverSettingsPrefab;
    public GameObject buttonTimerCubeSettingsPrefab;
    public GameObject portalSettingsPrefab; 
    public Transform settingParentTr;
    public List<int> inverseColor;
    public GameObject inversePair, inversePairParent;
    public List<GameObject> inversePairs;
    public int countInversePair; //because we dont remove from list
    public Vector2Int startPosition;
    public Vector2Int endPosition;
    public TMP_Dropdown dropdown;
    public GameObject leftButtons;

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
        List<bool> visibleAtBeginning;
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

        public bool atIsVisible(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    return visibleAtBeginning[i];
                }
            }
            return true;
        }

        public void remove(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    tilemaps.RemoveAt(i);
                    indexes.RemoveAt(i);
                    visibleAtBeginning.RemoveAt(i);
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
            if (visibleAtBeginning != null) visibleAtBeginning.Add(true);
            else Debug.Log("visible doesnt exist");
            last = Mathf.Max(last, index + 1);
        }

        public int lastIndex()
        {
            return last;
        }

        public List<int> getIndexes()
        {
            return indexes;
        }

        public void makeItNotNull(List<int> a, List<Tilemap> b, List<bool> c)
        {
            indexes = a;
            tilemaps = b;
            visibleAtBeginning = c;
        }

        public void changeVisibleAtBeginning(int index, bool to)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    visibleAtBeginning[i] = to;
                }
            }
        }
    }
    
    public FileInfo[] GetMapList()
    {
        string path = Application.persistentDataPath;
        DirectoryInfo dir = new DirectoryInfo(path);
        return dir.GetFiles("*.map");
    }

    public void MapDropdownUpdate(string currentOption = "")
    {
        dropdown.ClearOptions();

        FileInfo[] maps = GetMapList();
        
        List<string> options = new List<string>();
        foreach (FileInfo map in maps)
        {
            options.Add(map.Name.Substring(0, map.Name.Length-7));
        }
        dropdown.AddOptions(options);

        int dropdownIndex = 0;
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == currentOption)
            {
                dropdownIndex = i;
                break;
            }
        }
        dropdown.value = dropdownIndex;
    }

    private void Awake()
    {
        //tilemaps.makeItNotNull(new List<int>(), new List<Tilemap>(), new List<bool>());
        countInversePair = 0;
        calculatedCellWith = (xTopRight - xBottomLeft) / columns;
        calculatedCellHeight = (yTopRight - xBottomLeft) / rows;
        dropdown = FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        MapDropdownUpdate();
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
        if(currentTool != 0 && currentTool != 1 && currentTool != 6 && currentTool != 8 && currentTool != 9)
        {
            InteractiveAdded(x, y);
        }
        if (tilemaps.at(currentTilemap) == null)
        {
            Debug.Log("gonosz, katasztrófális szánalmas függvény!");
            return;
        }
        if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[8].tile) AddStartPosition(x, y);
        if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[9].tile) AddEndPosition(x, y);
    }

    public void InteractiveAdded(int x, int y)
    {
        //open menu, add settings
        OpenMenu();
        if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[2].tile) AddButton(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[3].tile) AddButtonForCube(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[4].tile) AddLever(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[5].tile) AddPortal(x, y);
        else if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[7].tile) AddButtonTimerCube(x, y);
    }

    public void AddStartPosition(int x, int y)
    {
        if(x != startPosition.x || y != startPosition.y) RemoveAllTileAtThisPositon(startPosition.x, startPosition.y);
        startPosition.x = x; 
        startPosition.y = y;
        if (startPosition == endPosition) endPosition = new Vector2Int(-10, -10);
    }

    public void AddEndPosition(int x, int y)
    {
        if (x != endPosition.x || y != endPosition.y) RemoveAllTileAtThisPositon(endPosition.x, endPosition.y);
        endPosition.x = x;
        endPosition.y = y;
        if (startPosition == endPosition) startPosition = new Vector2Int(0, 0);
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        showMenuButton.SetActive(false);
        leftButtons.SetActive(false);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        showMenuButton.SetActive(true);
        leftButtons.SetActive(true);
    }

    public void AddButton(int x, int y)
    {
        GameObject a = Instantiate(buttonSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color, -1);
        a.GetComponent<SettingForInteract>().isButton = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddButtonForCube(int x, int y)
    {
        GameObject a = Instantiate(buttonForCubeSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color, -1);
        a.GetComponent<SettingForInteract>().isButtonsForCube = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddLever(int x, int y)
    {
        GameObject a = Instantiate(leverSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color, -1);
        a.GetComponent<SettingForInteract>().isLever = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddButtonTimerCube(int x, int y)
    {
        GameObject a = Instantiate(buttonTimerCubeSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color, -1);
        a.GetComponent<SettingForInteract>().isButtonTimerCube = true;
        infos.Add(a.GetComponent<SettingForInteract>());
    }

    public void AddPortal(int x, int y)
    {
        GameObject a = Instantiate(portalSettingsPrefab, settingParentTr);
        a.GetComponent<SettingForInteract>().Set(x, y, currentTilemap, tilemaps.at(currentTilemap).color, -1);
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
            if (infos[i].x == x && infos[i].y == y && infos[i].index == currentTilemap)
            {
                infos[i].CommitSuicide();
                infos.RemoveAt(i);
                break;
            }
        }
    }

    public void Use(int x, int y)
    {
        if (currentTool == 0) RemoveAllTileAtThisPositon(x, y);
        else AddTile(x, y);
    }

    public void ChangeColor(int index)
    {
        if (tilemaps.at(index) == null)
        {
            if(tilemaps.count() > 0) index = tilemaps.getIndexes()[0];
            //Debug.LogError(index + " index még nem létezik, javítsd meg");
        }
        else currentTilemap = index;
    }

    public int AddColor(Color color, int index = -1)
    {
        int a;
        if (index == -1) a = tilemaps.lastIndex();
        else a = index;

        GameObject tm = Instantiate(tilemapPrefab, parentTilemap);
        
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
        map.gate = new int[columns][];
        for (int i = 0; i < columns; i++)
        {
            map.colorIndex[i] = new int[rows];
            map.gate[i] = new int[rows];
            for (int j = 0; j < rows; j++) 
            {
                map.colorIndex[i][j] = -1;
                map.gate[i][j] = -1;
                List<int> indices = tilemaps.getIndexes();
                for (int k = 0; k < indices.Count; k++)
                {
                    if (tilemaps.at(indices[k]).GetTile(new Vector3Int(i, j, 0)) == tools[1].tile)
                    {
                        map.colorIndex[i][j] = indices[k];
                    }
                    if (tilemaps.at(indices[k]).GetTile(new Vector3Int(i, j, 0)) == tools[6].tile)
                    {
                        map.gate[i][j] = indices[k];
                    }
                }
            }
        }

        map.colors = new MapData.ColorForSave[tilemaps.count()];
        map.activeAtStart = new MapData.ActiveAtStart[tilemaps.count()];
        List<int> indexes = tilemaps.getIndexes();
        for (int k = 0; k < tilemaps.count(); k++)
        {
            map.colors[k].Set(tilemaps.at(indexes[k]).color, indexes[k]);
            map.activeAtStart[k].isActive = tilemaps.atIsVisible(indexes[k]);
            map.activeAtStart[k].index = indexes[k];
        }

        int buttonForCubeCount = 0;
        int portalCount = 0;
        int leverCount = 0;
        int buttonCount = 0;
        int buttonTimerCubeCount = 0;
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
            else if (infos[i].isButton)
            {
                buttonCount++;
            }
            else if (infos[i].isButtonTimerCube)
            {
                buttonTimerCubeCount++;
            }
        }

        map.buttonForCubes = new MapData.ButtonForCube[buttonForCubeCount];
        map.portals = new MapData.Portal[portalCount];
        map.lever = new MapData.Lever[leverCount];
        map.buttons = new MapData.Button[buttonCount];
        map.buttonTimerCubes = new MapData.ButtonTimerCube[buttonTimerCubeCount];
        int c = 0;
        int p = 0;
        int l = 0;
        int b = 0;
        int btc = 0;
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
                portal.interactiveColor = infos[i].portalIndex;
                Debug.Log(infos[i].indexColorInteract);
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
            else if (infos[i].isButton)
            {
                MapData.Button button = new MapData.Button();
                button.color = infos[i].index;
                button.x = infos[i].x;
                button.y = infos[i].y;
                button.interactiveColor = infos[i].indexColorInteract;
                button.activateAtBeingActive = infos[i].activate;
                map.buttons[b++] = button;
            }
            else if (infos[i].isButtonTimerCube)
            {
                MapData.ButtonTimerCube buttonTimerCube = new MapData.ButtonTimerCube();
                buttonTimerCube.color = infos[i].index;
                buttonTimerCube.x = infos[i].x;
                buttonTimerCube.y = infos[i].y;
                buttonTimerCube.interactiveColor = infos[i].indexColorInteract;
                buttonTimerCube.timer = infos[i].timer;
                map.buttonTimerCubes[btc++] = buttonTimerCube;
            }
        }

        int validPairs = 0;
        for(int i = 0; i < inversePairs.Count; i++)
        {
            if (inversePairs[i].GetComponent<Suicide>().b1.GetComponent<InverseButton>().index != -1 && inversePairs[i].GetComponent<Suicide>().b2.GetComponent<InverseButton>().index != -1) validPairs++;
        }
        map.inversePairs = new MapData.Inverse[validPairs];
        int v = 0;
        for(int i = 0; i < inversePairs.Count; i++)
        {
            if (inversePairs[i].GetComponent<Suicide>().b1.GetComponent<InverseButton>().index != -1 && inversePairs[i].GetComponent<Suicide>().b2.GetComponent<InverseButton>().index != -1)
            {
                MapData.Inverse j = new MapData.Inverse();
                j.index1 = inversePairs[i].GetComponent<Suicide>().b1.GetComponent<InverseButton>().index;
                j.index2 = inversePairs[i].GetComponent<Suicide>().b2.GetComponent<InverseButton>().index;
                map.inversePairs[v++] = j;
            }
        }

        map.endx = endPosition.x;
        map.endy = endPosition.y;
        map.startx = startPosition.x;
        map.starty = startPosition.y;
    }
}
