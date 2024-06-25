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
    public bool pressedCarouselCycle;
    public bool pressedMenu;
    private Drag floater;
    public int mapX, mapY; //current map's coordinates
    public string mapName; //map's name
    Mappack mappack;

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

    public class Level
    {
        public int x, y;
        public string name;
        public List<int>[] missing;
        public Level(string name = null, int x = int.MaxValue, int y = int.MaxValue)
        {
            this.name = name;
            this.x = x;
            this.y = y;

            missing = new List<int>[4];
            for (int side = 0; side < 4; side++)
                missing[side] = new List<int>();
        }

        public enum Side
        {
            left = 0,
            right = 1,
            up = 2,
            down = 3
        }

        public void NewMissing(Side side, int idx) { NewMissing(((int)side), idx); }
        public void NewMissing(int side, int idx)
        {
            for (int i = 0; i < missing[side].Count; i++) if (missing[side][i] == idx) return; //check if it exists already
            missing[side].Add(idx);
        }

        public void RestoreMissing(Side side, int idx) { RestoreMissing(((int)side), idx); }
        public void RestoreMissing(int side, int idx)
        {
            missing[side].Remove(idx);
        }
    }

    public class Mappack
    {
        private Dictionary<(int, int), string> coordToName; //get name by coordinates
        public Dictionary<string, Level> levelInfo; //by name

        public Mappack(Level[] levels)
        {
            coordToName = new Dictionary<(int, int), string>();
            levelInfo = new Dictionary<string, Level>();

            for (int i = 0; i < levels.Length; i++) NewMap(levels[i]);
        }

        public string GetName(int x, int y)
        {
            string value;
            coordToName.TryGetValue((x, y), out value);
            return value;
        }

        public void NewMap(Level level, bool getMissingFromNeighbours = true)
        {
            if (coordToName.ContainsKey((level.x, level.y)) || levelInfo.ContainsKey(level.name))
            {
                Debug.Log("Duplicate coordinates or name");
                return;
            }

            coordToName.Add((level.x, level.y), level.name);
            levelInfo.Add(level.name, level);

            if (getMissingFromNeighbours)
            {//Get missing values from (potential) neighbours
                string neighbour;
                if (coordToName.TryGetValue((level.x, level.y - 1), out neighbour))
                    levelInfo[level.name].missing[((int)Level.Side.down)] = levelInfo[neighbour].missing[((int)Level.Side.up)];
                if (coordToName.TryGetValue((level.x, level.y + 1), out neighbour))
                    levelInfo[level.name].missing[((int)Level.Side.up)] = levelInfo[neighbour].missing[((int)Level.Side.down)];
                if (coordToName.TryGetValue((level.x - 1, level.y), out neighbour))
                    levelInfo[level.name].missing[((int)Level.Side.left)] = levelInfo[neighbour].missing[((int)Level.Side.right)];
                if (coordToName.TryGetValue((level.x + 1, level.y), out neighbour))
                    levelInfo[level.name].missing[((int)Level.Side.right)] = levelInfo[neighbour].missing[((int)Level.Side.left)];
            }
            else
            {//Give missing values to (potential) neighbours
                string neighbour;
                if (coordToName.TryGetValue((level.x, level.y - 1), out neighbour))
                    levelInfo[neighbour].missing[((int)Level.Side.up)] = levelInfo[level.name].missing[((int)Level.Side.down)];
                if (coordToName.TryGetValue((level.x, level.y + 1), out neighbour))
                    levelInfo[neighbour].missing[((int)Level.Side.down)] = levelInfo[level.name].missing[((int)Level.Side.up)];
                if (coordToName.TryGetValue((level.x - 1, level.y), out neighbour))
                    levelInfo[neighbour].missing[((int)Level.Side.right)] = levelInfo[level.name].missing[((int)Level.Side.left)];
                if (coordToName.TryGetValue((level.x + 1, level.y), out neighbour))
                    levelInfo[neighbour].missing[((int)Level.Side.left)] = levelInfo[level.name].missing[((int)Level.Side.right)];
            }

        }
        public void DelMap(string name)
        {
            coordToName.Remove((levelInfo[name].x, levelInfo[name].y));
            levelInfo.Remove(name);
        }
    }
    
    public FileInfo[] GetMapList()
    {
        string path = Application.dataPath + "/maps"; //Application.persitentDataPath
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
            //if (map.Name[0] == '!') continue;
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
        dropdown.SetValueWithoutNotify(dropdownIndex);
    }

    private void Awake()
    {
        //tilemaps.makeItNotNull(new List<int>(), new List<Tilemap>(), new List<bool>());
        countInversePair = 0;
        calculatedCellWith = (xTopRight - xBottomLeft) / columns;
        calculatedCellHeight = (yTopRight - xBottomLeft) / rows;
        dropdown = FindFirstObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        MapDropdownUpdate();
        floater = FindFirstObjectByType<Drag>();
    }

    private void Update()
    {
        HandleClick();
        ShortCuts();
    }

    public void ShortCuts()
    {
        if (!menu.activeSelf)
        {
            if (Input.GetAxisRaw("Remove") == 1) currentTool = 0;
            if (Input.GetAxisRaw("AddBasic") == 1) currentTool = 1;
            if (Input.GetAxisRaw("AddButton") == 1) currentTool = 2;
            if (Input.GetAxisRaw("AddButtonForCube") == 1) currentTool = 3;
            if (Input.GetAxisRaw("AddLever") == 1) currentTool = 4;
            if (Input.GetAxisRaw("AddPortal") == 1) currentTool = 5;
            if (Input.GetAxisRaw("AddGate") == 1) currentTool = 6;
            if (Input.GetAxisRaw("AddTimerCube") == 1) currentTool = 7;
            if (Input.GetAxisRaw("AddStart") == 1) currentTool = 8;
            if (Input.GetAxisRaw("AddFinish") == 1) currentTool = 9;
        
            if (Input.GetAxisRaw("CarouselCycle") != 0)
            {
                if (!pressedCarouselCycle)
                {
                    pressedCarouselCycle = true;
                    if (Input.GetAxisRaw("CarouselCycle") > 0) FindFirstObjectByType<ColorPalette>().SelectedColorIncrement();
                    else FindFirstObjectByType<ColorPalette>().SelectedColorDecrement();
                }
            }
            else pressedCarouselCycle = false;
        }

        if (Input.GetAxisRaw("Menu") == 1)
        {
            if (!pressedMenu)
            {
                pressedMenu = true;
                if (!menu.activeSelf) OpenMenu();
                else CloseMenu();
            }
        }
        else pressedMenu = false;

    }

    public void SetTool(int index)
    {
        currentTool = index; 
    }

    public bool MouseOnFloater()
    {
        return (floater.target.position.x < Input.mousePosition.x && Input.mousePosition.x < floater.target.position.x + 500) && (floater.target.position.y < Input.mousePosition.y && Input.mousePosition.y < floater.target.position.y + 130);
    }

    public int MouseXonGrid() { return Mathf.FloorToInt((Input.mousePosition.x - xBottomLeft) / calculatedCellWith); }
    public int MouseYonGrid() { return Mathf.FloorToInt((Input.mousePosition.y - yBottomLeft) / calculatedCellHeight); }

    public void HandleClick()
    {
        if (!Input.GetMouseButton(0) || MouseOnFloater() || menu.activeSelf) return; //haven't clicked or click is not for editing

        if (Input.mousePosition.x > xBottomLeft && Input.mousePosition.x < xTopRight && Input.mousePosition.y > yBottomLeft && Input.mousePosition.y < yTopRight)
        {//clicked inside editor
            Use(MouseXonGrid(), MouseYonGrid());
        }
        else
        {
            if (currentTool != 0 && currentTool != 1)
            {
                Debug.Log("No interactives on the border!");//TODO popup? sound? highlighting?
            }

            //check if borders are clicked:
            if (Input.mousePosition.x < xBottomLeft) //down
            {
                if (currentTool == 0) mappack.levelInfo[mapName].NewMissing(Level.Side.down, MouseYonGrid());
                else mappack.levelInfo[mapName].RestoreMissing(Level.Side.down, MouseYonGrid()+1);
            }
            if (Input.mousePosition.x > xTopRight) //up
            {
                if (currentTool == 0) mappack.levelInfo[mapName].NewMissing(Level.Side.up, MouseYonGrid());
                else mappack.levelInfo[mapName].RestoreMissing(Level.Side.up, MouseYonGrid()+1);
            }
            if (Input.mousePosition.y < yBottomLeft) //left
            {
                if (currentTool == 0) mappack.levelInfo[mapName].NewMissing(Level.Side.left, MouseXonGrid()+1);
                else mappack.levelInfo[mapName].RestoreMissing(Level.Side.left, MouseXonGrid()+1);
            }
            if (Input.mousePosition.x < xBottomLeft) //right
            {
                if (currentTool == 0) mappack.levelInfo[mapName].NewMissing(Level.Side.right, MouseXonGrid()+1);
                else mappack.levelInfo[mapName].RestoreMissing(Level.Side.right, MouseXonGrid()+1);
            }
        }
    }

    public void AddTile(int x, int y)
    {
        if(currentTool > tools.Count) currentTool = 1;
        RemoveAllTileAtThisPositon(x, y);
        if (tilemaps.at(currentTilemap) != null)
        {
            if (currentTool == 1 && currentTilemap == 0) tilemaps.at(currentTilemap).SetTile(new Vector3Int(x, y, 0), tools[0].tile);
            else tilemaps.at(currentTilemap).SetTile(new Vector3Int(x, y, 0), tools[currentTool].tile); 
        }

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
        if(x != startPosition.x || y != startPosition.y) if(tilemaps.at(currentTilemap).GetTile(new Vector3Int(x, y, 0)) == tools[8].tile) RemoveAllTileAtThisPositon(startPosition.x, startPosition.y);
        startPosition.x = x; 
        startPosition.y = y;
        if (startPosition == endPosition) endPosition = new Vector2Int(-10, -10);
    }

    public void AddEndPosition(int x, int y)
    {
        if (x != endPosition.x || y != endPosition.y) RemoveAllTileAtThisPositon(endPosition.x, endPosition.y);
        endPosition.x = x;
        endPosition.y = y;
        if (startPosition == endPosition) startPosition = new Vector2Int(-1, -1);
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        floater.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        floater.gameObject.SetActive(true);
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
        if(endPosition.x == x && endPosition.y == y)
        {
            endPosition = new Vector2Int(-10, -10);
        }
        if (startPosition.x == x && startPosition.y == y)
        {
            startPosition = new Vector2Int(-1, -1);
            //tilemaps.at(0).SetTile(new Vector3Int(0, 0, 0), tools[8].tile);
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

    public int AddColor(Color32 color, int index = -1)
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

    public void ModifyColor(int index, Color32 color)
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

        map.endx = endPosition.x;
        map.endy = endPosition.y;
        map.startx = startPosition.x;
        map.starty = startPosition.y;
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
                    if (tilemaps.at(indices[k]).GetTile(new Vector3Int(i, j, 0)) == tools[1].tile || tilemaps.at(indices[k]).GetTile(new Vector3Int(i, j, 0)) == tools[0].tile)//for white
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
    }
}
