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
    public Mappack mappack;
    public int? typedX, typedY; //from the input fields
    public GameObject GoButton, CreateButton, CreateInputField;
    public string typedName;
    public string mappackSaveName;
    public TMP_Dropdown mappackDropdown;
    public string mappackToLoad;
    public TMP_Text currentMapInfo;
    public Tilemap outsideWallTilemap;
    public bool mouseOnArrow; //stop handleclick()
    public PopUpHandler popUpHandler;
    public PopUp.AddNewMap newMapPopUp;
    HistoryManager history;
    ColorPalette colorPalette;

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

        public void changeVisibleAtBeginning(int index, bool to, bool noHistory = false)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    //register Default State modification
                    if (!noHistory) FindFirstObjectByType<HistoryManager>().stacks.Push(
                        new Change.ModDefaultState(FindFirstObjectByType<ColorPalette>().colors[i].color, to));
                    visibleAtBeginning[i] = to;
                }
            }
        }
    }

    [System.Serializable]
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

        //public enum Side
        //{
        //    left = 0,
        //    right = 1,
        //    up = 2,
        //    down = 3
        //}

        //public void NewMissing(Side side, int idx) { NewMissing(((int)side), idx); }
        public void NewMissing(int side, int idx)
        {
            for (int i = 0; i < missing[side].Count; i++) if (missing[side][i] == idx) return; //check if it exists already
            missing[side].Add(idx);
            //Debug.Log("new missing on side " + side + ": " + idx);
        }

        //public void RestoreMissing(Side side, int idx) { RestoreMissing(((int)side), idx); }
        public void RestoreMissing(int side, int idx)
        {
            missing[side].Remove(idx);
            //Debug.Log("restored missing on side " + side + ": " + idx);
        }

        //public void MissingFromTool(Side side, int idx, int tool) { MissingFromTool(((int)side), idx, tool); }
        public void MissingFromTool(int side, int idx, int tool)
        {
            if (tool == 1) RestoreMissing(side, idx);
            else NewMissing(side, idx);
        }
    }

    [System.Serializable]
    public class Mappack
    {
        public string ID; //mappack's name
        public Dictionary<(int, int), string> coordToName; //get name by coordinates
        public Dictionary<string, Level> levelInfo; //by name

        public Mappack(MappackData data)
        {
            ID = data.ID;
            coordToName = new Dictionary<(int, int), string>();
            levelInfo = new Dictionary<string, Level>();

            for (int i = 0; i < data.levels.Length; i++) NewMap(data.levels[i], null);
        }
        public Mappack(string id, Level[] levels, bool uniformMissings = false)
        {
            ID = id;
            coordToName = new Dictionary<(int, int), string>();
            levelInfo = new Dictionary<string, Level>();

            for (int i = 0; i < levels.Length; i++) NewMap(levels[i], uniformMissings ? true : null);
        }

        public void OverwriteMap(int x, int y, string newName)
        {
            string oldName = coordToName[(x, y)];
            coordToName[(x, y)] = newName;

            levelInfo[newName] = levelInfo[oldName];

            levelInfo.Remove(oldName);

            levelInfo[newName].name = newName;
        }

        public void NewMap(Level level, bool? getMissingFromNeighbours = true)
        {
            if (coordToName.ContainsKey((level.x, level.y)) || levelInfo.ContainsKey(level.name))
            {
                Debug.Log("Duplicate coordinates or name");
                return;
            }

            coordToName.Add((level.x, level.y), level.name);
            levelInfo.Add(level.name, level);

            if (getMissingFromNeighbours == true)
            {//Get missing values from (potential) neighbours
                string neighbour;
                if (coordToName.TryGetValue((level.x, level.y - 1), out neighbour))
                    levelInfo[level.name].missing[3] = levelInfo[neighbour].missing[2];
                if (coordToName.TryGetValue((level.x, level.y + 1), out neighbour))
                    levelInfo[level.name].missing[2] = levelInfo[neighbour].missing[3];
                if (coordToName.TryGetValue((level.x - 1, level.y), out neighbour))
                    levelInfo[level.name].missing[0] = levelInfo[neighbour].missing[1];
                if (coordToName.TryGetValue((level.x + 1, level.y), out neighbour))
                    levelInfo[level.name].missing[1] = levelInfo[neighbour].missing[0];
            }
            else if (getMissingFromNeighbours == false)
            {//Give missing values to (potential) neighbours
                string neighbour;
                if (coordToName.TryGetValue((level.x, level.y - 1), out neighbour))
                    levelInfo[neighbour].missing[2] = levelInfo[level.name].missing[3];
                if (coordToName.TryGetValue((level.x, level.y + 1), out neighbour))
                    levelInfo[neighbour].missing[3] = levelInfo[level.name].missing[2];
                if (coordToName.TryGetValue((level.x - 1, level.y), out neighbour))
                    levelInfo[neighbour].missing[1] = levelInfo[level.name].missing[0];
                if (coordToName.TryGetValue((level.x + 1, level.y), out neighbour))
                    levelInfo[neighbour].missing[0] = levelInfo[level.name].missing[1];
            }

        }
        public void DelMap(string name)
        {
            coordToName.Remove((levelInfo[name].x, levelInfo[name].y));
            levelInfo.Remove(name);
        }

        [System.Serializable]
        public class MappackData
        {
            public string ID;
            public Level[] levels;
            public MappackData(Mappack mappack)
            {
                this.ID = mappack.ID;

                levels = new Level[mappack.levelInfo.Count];
                int i = 0;
                foreach (Level level in mappack.levelInfo.Values)
                {
                    levels[i++] = level;
                }
            }
        }
    }

    public void SaveMappackByButton()
    {
        if (mappackSaveName == null || mappackSaveName == "")
        {
            Debug.Log("saveName is null");
            return;
        }
        mappack.ID = mappackSaveName;
        SaveLoadMaps.SaveMappack(mappack);
        MappackDropdownUpdate(mappackSaveName);
    }

    public void LoadMappackByButton()
    {
        if (mappackToLoad == null || mappackToLoad == "")
        {
            Debug.Log("mappack to load is null");
            return;
        }
        mappack = new Mappack(SaveLoadMaps.LoadMappack(mappackToLoad));
        MappackDropdownUpdate(mappackToLoad);
        checkIfTypedExists();

        string firstMapFound = "";
        foreach (string temp2 in mappack.coordToName.Values) //get the first key from the dict
        {
            firstMapFound = temp2;
            break;
        }

        FindFirstObjectByType<Map>().ResetEditor();

        if (firstMapFound != "") GoToMap(mappack.levelInfo[firstMapFound].x, mappack.levelInfo[firstMapFound].y); //load a map
        else
        {
            Debug.Log("loaded mappack doesn't have maps!");
        }
        UpdateCurrentMapInfo();
    }

    public void SetMappackSaveName(string text)
    {
        if (text.Length > 0) mappackSaveName = text;
        else mappackSaveName = null;
    }

    //called by creating arrows
    public void CreateLeftMap() => newMapPopUp.Set(mapX - 1, mapY, true);
    public void CreateRightMap() => newMapPopUp.Set(mapX + 1, mapY, true);
    public void CreateUpMap() => newMapPopUp.Set(mapX, mapY + 1, true);
    public void CreateDownMap() => newMapPopUp.Set(mapX, mapY - 1, true);

    //called by normal arrows
    public void GoToLeftMap() => GoToMap(mapX - 1, mapY);
    public void GoToRightMap() => GoToMap(mapX + 1, mapY);
    public void GoToDownMap() => GoToMap(mapX, mapY - 1);
    public void GoToUpMap() => GoToMap(mapX, mapY + 1);

    public void GoToMapByButton()
    {
        if (typedX == null || typedY == null)
        { 
            Debug.Log("invalid coordinates");
            return;
        }
        GoToMap(typedX.Value, typedY.Value);
    }
    public void GoToMap(int x, int y)
    {
        if (mappack.coordToName.ContainsKey((x, y))) //map exists
        {
            mapName = mappack.coordToName[(x, y)];
            mapX = x;
            mapY = y;
            Debug.Log(mapName);

            FindFirstObjectByType<Map>().LoadIntoEditor(mapName);
            UpdateCurrentMapInfo();
        }
    }

    public void CreateMapByButton()
    {
        if (typedName != null && typedX != null && typedY != null)
        {
            Debug.Log("created: " + typedName);
            mappack.NewMap(new Level(typedName, typedX.Value, typedY.Value));
            checkIfTypedExists(); //spoiler alert: it does exists (:
        }
        else Debug.Log("smh is null");
    }

    public void SetTypedNameByInputField(string text)
    {
        if (text.Length > 0) typedName = text.Trim();
        else typedName = null;
    }

    public void SetXByInputField(string text)
    {
        if (!(text.Length > 0)) typedX = null;
        else
        {
            typedX = int.Parse(text);
            checkIfTypedExists();
        }
    }
    public void SetYByInputField(string text)
    {
        if (!(text.Length > 0)) typedY = null;
        else
        {
            typedY = int.Parse(text);
            checkIfTypedExists();
        }
    }
    public void checkIfTypedExists()
    {
        if (typedX == null || typedY == null)
        {
            GoButton.GetComponent<Button>().interactable = false;
            CreateButton.GetComponent<Button>().interactable = false;
            CreateInputField.GetComponent<TMP_InputField>().interactable = false;
        }
        else if (mappack.coordToName.ContainsKey((typedX.Value, typedY.Value)))
        {
            GoButton.GetComponent<Button>().interactable = true;
            CreateButton.GetComponent<Button>().interactable = false;
            CreateInputField.GetComponent<TMP_InputField>().interactable = false;
        }
        else
        {
            GoButton.GetComponent<Button>().interactable = false;
            CreateButton.GetComponent<Button>().interactable = true;
            CreateInputField.GetComponent<TMP_InputField>().interactable = true;
        }
    }

    public FileInfo[] GetMappackList()
    {
        string path = Application.streamingAssetsPath + "/mappacks";
        DirectoryInfo dir = new DirectoryInfo(path);
        return dir.GetFiles("*.mappack");
    }

    public void MappackDropdownUpdate(string currentOption = "")
    {
        mappackDropdown.ClearOptions();

        FileInfo[] mappacks = GetMappackList();

        List<string> options = new List<string>();
        foreach (FileInfo mappack in mappacks)
        {
            if (mappack.Name[0] == '!') continue; //TODO uncomment this line
            options.Add(mappack.Name.Substring(0, mappack.Name.Length - 8));
        }
        mappackDropdown.AddOptions(options);

        mappackToLoad = null;
        int dropdownIndex = 0;
        for (int i = 0; i < mappackDropdown.options.Count; i++)
        {
            if (mappackDropdown.options[i].text == currentOption)
            {
                mappackToLoad = currentOption;
                dropdownIndex = i;
                break;
            }
        }
        if (mappackToLoad == null) mappackToLoad = mappackDropdown.options[0].text;
        mappackDropdown.SetValueWithoutNotify(dropdownIndex);
    }

    public void SetSelectedMappackToLoad(int nameIndex)
    {
        mappackToLoad = mappackDropdown.options[nameIndex].text;
    }

    public void UpdateCurrentMapInfo()
    {
        if (mapName != null) currentMapInfo.text = "Currently editing: " + mapName + " on (" + mapX + "; " + mapY + ")";
        else currentMapInfo.text = "Currently editing: -";

        Arrow[] arrows = FindObjectsByType<Arrow>(FindObjectsSortMode.None);
        for (int i = 0; i < 4; i++)
        {
            bool mapExists = false;
            if (arrows[i].normalArrow.name == "LeftArrow")
                mapExists = mappack.coordToName.ContainsKey((mapX - 1, mapY));
            else if (arrows[i].normalArrow.name == "RightArrow")
                mapExists = mappack.coordToName.ContainsKey((mapX + 1, mapY));
            else if (arrows[i].normalArrow.name == "UpArrow")
                mapExists = mappack.coordToName.ContainsKey((mapX, mapY + 1));
            else if (arrows[i].normalArrow.name == "DownArrow")
                mapExists = mappack.coordToName.ContainsKey((mapX, mapY - 1));

            arrows[i].SetIndication(!mapExists);
        }
    }

    public void MapDropdownUpdate(string currentOption = "")
    {
        dropdown.ClearOptions();

        FileInfo[] maps = SaveLoadMaps.GetMapList();
        
        List<string> options = new List<string>();
        foreach (FileInfo map in maps)
        {
            if (map.Name[0] == '!') continue;
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
        MapDropdownUpdate();
        MappackDropdownUpdate();
        floater = FindFirstObjectByType<Drag>();
        typedX = null;
        typedY = null;
        mappackSaveName = null;
        popUpHandler = FindFirstObjectByType<PopUpHandler>();
        colorPalette = FindFirstObjectByType<ColorPalette>();
        history = FindFirstObjectByType<HistoryManager>();

        if (popUpHandler == null || colorPalette == null || history == null) Debug.Log("smh is null");

        if (mappack.levelInfo == null) mappack = new Mappack("default", new Level[] {new Level("-", 0, 0)});
        mapName = "-";
        mapX = 0;
        mapY = 0;
        UpdateCurrentMapInfo();
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

            if (Input.GetKeyDown(KeyCode.R))
            {
                Map map = FindFirstObjectByType<Map>();
                GetInfos(map);

                string holder = map.index;
                map.index = "!temp";
                SaveLoadMaps.SaveMap(map);
                map.index = holder;

                SceneLoader.LoadTestScene();
            }
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

    public bool MouseOnFloater() => (floater.target.position.x < Input.mousePosition.x && Input.mousePosition.x < floater.target.position.x + 500) && (floater.target.position.y < Input.mousePosition.y && Input.mousePosition.y < floater.target.position.y + 130);
    public int MouseXonGrid() => Mathf.FloorToInt((Input.mousePosition.x - xBottomLeft) / calculatedCellWith);
    public int MouseYonGrid() => Mathf.FloorToInt((Input.mousePosition.y - yBottomLeft) / calculatedCellHeight);

    public void HandleClick()
    {
        if (!Input.GetMouseButton(0) || MouseOnFloater() || menu.activeSelf || mouseOnArrow || popUpHandler.popupActive) return; //haven't clicked or click is not for editing

        if (Input.mousePosition.x > xBottomLeft && Input.mousePosition.x < xTopRight && Input.mousePosition.y > yBottomLeft && Input.mousePosition.y < yTopRight)
        {//clicked inside editor
            Use(MouseXonGrid(), MouseYonGrid());
        }
        else
        {//clicked on the border
            if (currentTool != 0 && currentTool != 1)
            {
                Debug.Log("No interactives on the border!");//TODO popup? sound? highlighting?
            }

            string neighbour; //handle the neighbours with shared borders
            //check if borders are clicked:
            if (Input.mousePosition.y < yBottomLeft) //down
            {
                mappack.levelInfo[mapName].MissingFromTool(3, MouseXonGrid(), currentTool);
                outsideWallTilemap.SetTile(new Vector3Int(MouseXonGrid(), -1, 0), currentTool == 1 ? basicTile : clear);
                if (mappack.coordToName.TryGetValue((mappack.levelInfo[mapName].x, mappack.levelInfo[mapName].y - 1), out neighbour))
                    mappack.levelInfo[neighbour].MissingFromTool(2, MouseXonGrid(), currentTool);
            }
            if (Input.mousePosition.y > yTopRight) //up
            {
                mappack.levelInfo[mapName].MissingFromTool(2, MouseXonGrid(), currentTool);
                outsideWallTilemap.SetTile(new Vector3Int(MouseXonGrid(), 16, 0), currentTool == 1 ? basicTile : clear);
                if (mappack.coordToName.TryGetValue((mappack.levelInfo[mapName].x, mappack.levelInfo[mapName].y + 1), out neighbour))
                    mappack.levelInfo[neighbour].MissingFromTool(3, MouseXonGrid(), currentTool);
            }
            if (Input.mousePosition.x < xBottomLeft) //left
            {
                mappack.levelInfo[mapName].MissingFromTool(0, MouseYonGrid(), currentTool);
                outsideWallTilemap.SetTile(new Vector3Int(-1, MouseYonGrid(), 0), currentTool == 1 ? basicTile : clear);
                if (mappack.coordToName.TryGetValue((mappack.levelInfo[mapName].x - 1, mappack.levelInfo[mapName].y), out neighbour))
                    mappack.levelInfo[neighbour].MissingFromTool(1, MouseYonGrid(), currentTool);
            }
            if (Input.mousePosition.x > xTopRight) //right
            {
                mappack.levelInfo[mapName].MissingFromTool(1, MouseYonGrid(), currentTool);
                outsideWallTilemap.SetTile(new Vector3Int(30, MouseYonGrid(), 0), currentTool == 1 ? basicTile : clear);
                if (mappack.coordToName.TryGetValue((mappack.levelInfo[mapName].x + 1, mappack.levelInfo[mapName].y), out neighbour))
                    mappack.levelInfo[neighbour].MissingFromTool(0, MouseYonGrid(), currentTool);
            }
        }
    }

    public BlockData GetTileAt(int x, int y)
    {
        for (int i = 0; i < infos.Count; i++) //check interactives
        {
            if (infos[i].x == x && infos[i].y == y)
            {
                return new BlockData(infos[i]);
            }
        }

        List<int> indices = tilemaps.getIndexes();
        for (int i = 0; i < indices.Count; i++) //check blocks
        {
            if (!(tilemaps.at(indices[i]).GetTile(new Vector3Int(x, y, 0)) == null || clear == tilemaps.at(indices[i]).GetTile(new Vector3Int(x, y, 0))))
            {
                return new BlockData(x, y, colorPalette.FindButton(tilemaps.at(indices[i]).color).index,
                                           tilemaps.at(indices[i]).GetTile(new Vector3Int(x, y, 0)));
            }
        }

        return new BlockData(x, y, -1, clear); //clear (-1 bc it doesn't have a color (it shouldn't be tested by this value!))
    }

    public void AddTile(int x, int y, int tool = -1, int tilemapIndex = -2, bool noHistory = false)
    {
        if(currentTool > tools.Count) currentTool = 1; //if selected tool is invalid set it to 1
        if (tool == -1) tool = currentTool; //if no tool specified, use the selected one
        if (tilemapIndex == -2) tilemapIndex = currentTilemap;

        BlockData before = GetTileAt(x, y);
        BlockData after = null;

        RemoveAllTileAtThisPositon(x, y);
        if (tilemaps.at(tilemapIndex) != null)
        {
            if (tool == 1 && tilemapIndex == 0)
            {
                tilemaps.at(tilemapIndex).SetTile(new Vector3Int(x, y, 0), tools[0].tile);

            }
            else tilemaps.at(tilemapIndex).SetTile(new Vector3Int(x, y, 0), tools[tool].tile); 

        }

        else Debug.Log(tilemapIndex + " doesnt exist");
        if(tool != 0 && tool != 1 && tool != 6 && tool != 8 && tool != 9)
        {
            InteractiveAdded(x, y, tilemapIndex);
            after = new BlockData(infos[^1]); //if interactive
        }
        else
        {
            after = GetTileAt(x, y); //if non-interactive
        }

        if (!noHistory && (after.type != before.type || before.colorIndex != after.colorIndex))
            history.stacks.Push(new Change.AddTile(before, after)); //register new block

        if (tilemaps.at(tilemapIndex) == null)
        {
            Debug.Log("gonosz, katasztrófális szánalmas függvény!");
            return;
        }
        if(tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[8].tile) AddStartPosition(x, y);
        if(tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[9].tile) AddEndPosition(x, y);
    }

    public void InteractiveAdded(int x, int y, int tilemapIndex = -1)
    {
        if (tilemapIndex == -1) tilemapIndex = currentTilemap;

        //open menu, add settings
        //TODO smaller popups? 
        if (tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[2].tile) AddButton(x, y);
        else if (tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[3].tile) AddButtonForCube(x, y);
        else if (tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[4].tile) AddLever(x, y);
        else if (tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[5].tile) AddPortal(x, y);
        else if (tilemaps.at(tilemapIndex).GetTile(new Vector3Int(x, y, 0)) == tools[7].tile) AddButtonTimerCube(x, y);
        else Debug.Log("not good" + GetTileAt(x, y));
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

    public void Use(int x, int y, int tool = -1, int tilemapIndex = -2, bool noHistory = false)
    {
        if (tilemapIndex == -2) tilemapIndex = currentTilemap;
        if (tool == -1) tool = currentTool;
        if (tool == 0)
        {
            BlockData before = GetTileAt(x, y);
            if (!noHistory && before.type != -1) history.stacks.Push(new Change.RemoveTile(before)); //register deletion
            RemoveAllTileAtThisPositon(x, y);
        }
        else AddTile(x, y, tool, tilemapIndex, noHistory);
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
            if (pair == null)
            {
                Debug.Log("pair is null"); //probably bc they get destroyed somewhere, but stay in the list
                continue;
            }
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

    public void CreateInversePair(bool noHistory = false)
    {
        countInversePair++;
        inversePairs.Add(Instantiate(inversePair, inversePairParent.transform));
        inversePairParent.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 80 + countInversePair * 120);

        if (!noHistory) history.stacks.Push(new Change.AddInversePair(inversePairs[^1])); //register new Inverse Pair
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
            if (inversePairs[i].GetComponent<InversePair>().b1.GetComponent<InverseButton>().index != -1 && inversePairs[i].GetComponent<InversePair>().b2.GetComponent<InverseButton>().index != -1) validPairs++;
        }
        map.inversePairs = new MapData.Inverse[validPairs];
        int v = 0;
        for(int i = 0; i < inversePairs.Count; i++)
        {
            if (inversePairs[i].GetComponent<InversePair>().b1.GetComponent<InverseButton>().index != -1 && inversePairs[i].GetComponent<InversePair>().b2.GetComponent<InverseButton>().index != -1)
            {
                MapData.Inverse j = new MapData.Inverse();
                j.index1 = inversePairs[i].GetComponent<InversePair>().b1.GetComponent<InverseButton>().index;
                j.index2 = inversePairs[i].GetComponent<InversePair>().b2.GetComponent<InverseButton>().index;
                map.inversePairs[v++] = j;
            }
        }
    }
}
