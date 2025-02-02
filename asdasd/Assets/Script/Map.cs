using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapData;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static WallManager;
using System.ComponentModel;
using System.IO;
using UnityEngine.Rendering;
using JetBrains.Annotations;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class Map : MonoBehaviour
{
    public MapEditor mapEditor;
    public ColorPalette colorPalette;
    public string index;
    public int[][] colorIndex; //to which color belongs this wall
    public int[][] gate; //to which color belongs this gate
    public MapData.ColorForSave[] colors; //color of the indexes
    //start, end positions needed
    public MapData.Portal[] portals;
    public MapData.Lever[] lever;
    public MapData.Button[] buttons;
    public MapData.ButtonForCube[] buttonForCubes;
    public MapData.ButtonTimerCube[] buttonTimerCubes;
    public MapData.ActiveAtStart[] activeAtStart; //is the index active at the beginning
    public MapData.Inverse[] inversePairs;
    public Transform tilemapParent;
    public Transform thingParent;
    public GameObject tilemapPrefab;
    public TileBase tileBase;
    public TileBase tileColorBase;
    public TileBase gateBase;
    public GameObject buttonForCubePrefab;
    public GameObject portalPrefab;
    public GameObject leverPrefab;
    public GameObject buttonPrefab;
    public GameObject buttonTimerCubePrefab;
    public GameObject gatePrefab;
    public GameObject gateLightPrefab;
    public int tileSize;
    public float tileX; //the minimum x
    public float tileY; //the minimum y
    public int row;
    public int column;
    public int startx, starty, endx, endy;
    public TMP_Dropdown dropdown;
    public TMP_InputField saveName;
    public Tilemap decoTilemap;
    public RuleTile decoBase;
    public List<Deco> decos;
    public bool[][] hasTile;
    public bool[][] hasWhiteWall;
    public GameObject prefabOfNPC;
    public List<NPCList> datas;
    public Tile clear;
    public static event Action<int, int> deactivateDecos;
    public bool loadFromProgress;
    public GameObject cubePrefab;

    [System.Serializable]
    public struct Deco
    {
        public Sprite sprite;
        public GameObject prefab;

        public void Create(int x, int y)
        {
            GameObject g = Instantiate(prefab, FindFirstObjectByType<WallManager>().decoParent);
            Map m = FindFirstObjectByType<Map>();
            g.GetComponent<Transform>().position = new Vector3(m.tileX + x, m.tileY + y, 0);
            Decos d = g.AddComponent<Decos>();
            d.x = x; d.y = y;
        }
    };

    

    private void Start()
    {
        mapEditor = FindFirstObjectByType<MapEditor>();
        if (loadFromProgress)
        {
            LoadFromProgress();
        }
    }

    public void LoadFromProgress()
    {
        FindFirstObjectByType<WallManager>().ResetThings();
        if (FindFirstObjectByType<Cube>() != null) FindFirstObjectByType<Cube>().ForceDestroy(FindFirstObjectByType<Cube>().colorIndex);
        KillAllTheChildren(thingParent);
        KillAllTheChildren(FindFirstObjectByType<WallManager>().decoParent);
        Progress progress = SaveLoadMaps.LoadProgress();
        Debug.Log(progress.x + " " + progress.y);
        if (!(progress.x < -20 && progress.y < -20))
        {
            FindFirstObjectByType<MultipleLevel>().currentX = progress.roomX;
            FindFirstObjectByType<MultipleLevel>().currentY = progress.roomY;

            foreach (Progress.C c in progress.colors)
            {
                WallObjects[] wo = FindObjectsOfType<WallObjects>(includeInactive: true);
                Tilemap t = null;
                for (int j = 0; j < wo.Length; j++)
                {
                    if (wo[j].colorIndex == c.index)
                    {
                        t = wo[j].gameObject.GetComponent<Tilemap>();
                        Debug.Log(c.index + " " + wo[j].name);
                    }
                }

                if (t == null)
                {
                    Debug.Log(c.c() + " " + c.index + " " + c.visible + " " + c.defaultState);
                    FindFirstObjectByType<WallManager>().colors.add(c.c(), c.index, c.visible);
                    FindFirstObjectByType<WallManager>().activeAtStart.add(c.index, c.defaultState);
                    GameObject a = Instantiate(tilemapPrefab, tilemapParent);
                    a.GetComponent<WallObjects>().Create(c.index);
                }
            }


            FindFirstObjectByType<MultipleLevel>().LoadAMap();
            FindFirstObjectByType<CubePlacer>().gameObject.transform.position = new Vector2(progress.x, progress.y);

            CubePlacer cubePlacer = FindFirstObjectByType<CubePlacer>();
            foreach (Progress.TimerCubeStruct tc in progress.timerCubes)
            {
                if (tc.placed)
                {
                    GameObject newTimerCube = Instantiate(cubePlacer.timerCubePrefab);
                    newTimerCube.transform.position = new Vector2(tc.x, tc.y);
                    newTimerCube.GetComponent<TimerCube>().colorIndex = tc.index;
                    newTimerCube.GetComponent<TimerCube>().lifeTime = tc.timeLimit;
                    newTimerCube.GetComponent<TimerCube>().birthTime = Time.time - tc.time;
                    newTimerCube.GetComponent<TimerCube>().Set();
                }
                else
                {
                    CubePlacer.TimerCubeData temp;
                    temp.colorIndex = tc.index;
                    temp.timer = tc.timeLimit;
                    cubePlacer.timerCubes.Add(temp);
                }
            }
            if (progress.cube.index != -1)
            {
                GameObject cube = Instantiate(cubePrefab);
                cube.GetComponent<Cube>().colorIndex = progress.cube.index;
                cube.transform.position = new Vector2(progress.cube.x, progress.cube.y);
                cube.GetComponent<Cube>().Set();
            }
        }
        else Debug.Log("nem akar lefutni");
    }

    public void SaveMap()
    {
        mapEditor = FindFirstObjectByType<MapEditor>();
        FindFirstObjectByType<MapEditor>().GetInfos(this);
        SaveLoadMaps.SaveMap(this);
        mapEditor.MapDropdownUpdate(index);
    }

    public void SetIndex(string i)
    {
        index = i;
    }

    public void IndexUpdateSelectedToLoad(int index2)
    {
        SetIndex(mapEditor.dropdown.options[index2].text);
    }

    public void UpdateSelectedDropdownOption(string name)
    {
        for(int i = 0; i < mapEditor.dropdown.options.Count; i++)
        {
            if (mapEditor.dropdown.options[i].text == name)
            {
                mapEditor.dropdown.SetValueWithoutNotify(i);
            }
        }
    }

    public void ByButtonLoadIntoEditor()
    {
        LoadIntoEditor(index);
        colorPalette.AdjustHeight();
    }

    public void LoadIntoEditor(string mapToLoad)
    {
        ResetEditor();
        int i, j;
        mapEditor = FindFirstObjectByType<MapEditor>();
        colorPalette = FindFirstObjectByType<ColorPalette>();
        dropdown = FindFirstObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        
        FindFirstObjectByType<HistoryManager>().stacks = new HistoryManager.Stacks(); //clear history
        if (FindFirstObjectByType<HistoryManager>().stacks == null) Debug.Log("???");

        if (mapToLoad == "")
        {
            mapToLoad = dropdown.options[0].text;
        }

        bool overWritten = false;
        if (mapEditor.mapName != mapToLoad) //mappack needs to be overwritten at current coords
        {
            mapEditor.mappack.OverwriteMap(mapEditor.mapX, mapEditor.mapY, mapToLoad);
            mapEditor.mapName = mapToLoad;
            mapEditor.UpdateCurrentMapInfo();
            overWritten = true;
        }

        MapData data = SaveLoadMaps.LoadMap(mapToLoad);

        if (data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        colorPalette.KillAllTheChildren();
        for (i = 0; i < data.colors.Length; i++)
        {
            if (data.colors[i].r == 255 && data.colors[i].g == 255 && data.colors[i].b == 255) continue; //feheret nem bantjuk!
            colorPalette.CreateColor(data.colors[i].c(), data.colors[i].index, true);
        }

        mapEditor.columns = data.column;
        mapEditor.rows = data.row;

        mapEditor.AddTile(data.startx, data.starty, 8, -2, true);

        mapEditor.AddTile(data.endx, data.endy, 9, -2, true);

        for (i = 0; i < data.colorIndex.Length; i++)
        {
            for (j = 0; j < data.colorIndex[i].Length; j++)
            {
                if (data.colorIndex[i][j] == -1)
                {
                    continue;
                }

                mapEditor.currentTilemap = data.colorIndex[i][j];

                mapEditor.AddTile(i, j, 1, -2, true);
            }
        }

        for (i = 0; i < data.buttons.Length; i++) //button
        {
            mapEditor.currentTilemap = data.buttons[i].color;
            mapEditor.AddTile(data.buttons[i].x, data.buttons[i].y, 2, -2, true);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.buttons[i].x, data.buttons[i].y, data.buttons[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.buttons[i].color).color, data.buttons[i].interactiveColor, data.buttons[i].activateAtBeingActive);
        }
        for (i = 0; i < data.buttonForCubes.Length; i++) //buttonForCube
        {
            mapEditor.currentTilemap = data.buttonForCubes[i].color;
            mapEditor.AddTile(data.buttonForCubes[i].x, data.buttonForCubes[i].y, 3, -2, true);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.buttonForCubes[i].x, data.buttonForCubes[i].y, data.buttonForCubes[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.buttonForCubes[i].color).color, data.buttonForCubes[i].interactiveColor);
        }
        for (i = 0; i < data.lever.Length; i++) //lever
        {
            mapEditor.currentTilemap = data.lever[i].color;
            mapEditor.AddTile(data.lever[i].x, data.lever[i].y, 4, -2, true);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.lever[i].x, data.lever[i].y, data.lever[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.lever[i].color).color, data.lever[i].interactiveColor);
        }
        for (i = 0; i < data.portals.Length; i++) //portal
        {
            //Debug.Log(data.portals[i].x + " asd " + data.portals[i].y + " asd " + data.portals[i].color + ' ' + data.portals[i].interactiveColor);
            mapEditor.currentTilemap = data.portals[i].color;
            mapEditor.AddTile(data.portals[i].x, data.portals[i].y, 5, -2, true);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.portals[i].x, data.portals[i].y, data.portals[i].color, mapEditor.tilemaps.at(data.portals[i].color).color, data.portals[i].interactiveColor);
            mapEditor.infos[mapEditor.infos.Count - 1].SetPortalAtLoading(data.portals[i].interactiveColor, true);
        }
        for (i = 0; i < data.gate.Length; i++) //gate
        {
            for (j = 0; j < data.gate[i].Length; j++)
            {
                if (data.gate[i][j] == -1)
                {
                    continue;
                }

                mapEditor.currentTilemap = data.gate[i][j];

                mapEditor.AddTile(i, j, 6, -2, true);
            }
        }
        for (i = 0; i < data.buttonTimerCubes.Length; i++) //buttonForTimerCube
        {
            mapEditor.currentTilemap = data.buttonTimerCubes[i].color;
            mapEditor.AddTile(data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y, 7, -2, true);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y, data.buttonTimerCubes[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.buttonTimerCubes[i].color).color, data.buttonTimerCubes[i].interactiveColor);
            mapEditor.infos[mapEditor.infos.Count - 1].SetTimerAtLoading(data.buttonTimerCubes[i].timer, true);
        }

        for (i = 0; i < data.inversePairs.Length; i++)
        {
            mapEditor.CreateInversePair(true);
            InverseButton[] buttons = mapEditor.inversePairs[mapEditor.inversePairs.Count - 1].GetComponentsInChildren<InverseButton>();

            buttons[0].index = data.inversePairs[i].index1;
            buttons[0].GetComponent<Image>().sprite = buttons[0].wall;
            buttons[0].GetComponent<Image>().color = mapEditor.tilemaps.at(data.inversePairs[i].index1).color;

            buttons[1].index = data.inversePairs[i].index2;
            buttons[1].GetComponent<Image>().sprite = buttons[1].wall;
            buttons[1].GetComponent<Image>().color = mapEditor.tilemaps.at(data.inversePairs[i].index2).color;
        }

        for (i = 0; i < data.activeAtStart.Length; i++)
        {
            mapEditor.tilemaps.changeVisibleAtBeginning(data.activeAtStart[i].index, data.activeAtStart[i].isActive, true);
        }

        for (i = 0; i < data.activeAtStart.Length; i++)
        {
            for (j = 1; j < colorPalette.colors.Count; j++)//starts from 1!
            {
                if (colorPalette.colors[j].index == data.activeAtStart[i].index)
                {
                    if (colorPalette.colors[j].toggle.GetComponentInChildren<Toggle>() == null) Debug.Log("From Map a problem " + i + " " + j);
                    colorPalette.colors[j].toggle.GetComponentInChildren<Toggle>().SetIsOnWithoutNotify(data.activeAtStart[i].isActive);
                    mapEditor.tilemaps.changeVisibleAtBeginning(colorPalette.colors[j].index, data.activeAtStart[i].isActive, true);
                    break;
                }
            }
        }

        if (!overWritten)
        {
            List<int>[] missing = mapEditor.mappack.levelInfo[mapEditor.mapName].missing; //TODO not everymap is in a mappack //now kinda is (not necessarily good)

            for (i = 0; i < missing[2].Count; i++) //up
            {
                mapEditor.outsideWallTilemap.SetTile(new Vector3Int(missing[2][i], 16, 0), clear);
            }
            for (i = 0; i < missing[3].Count; i++) //down
            {
                mapEditor.outsideWallTilemap.SetTile(new Vector3Int(missing[3][i], -1, 0), clear);
            }
            for (i = 0; i < missing[0].Count; i++) //left
            {
                mapEditor.outsideWallTilemap.SetTile(new Vector3Int(-1, missing[0][i], 0), clear);
            }
            for (i = 0; i < missing[1].Count; i++) //right
            {
                mapEditor.outsideWallTilemap.SetTile(new Vector3Int(30, missing[1][i], 0), clear);
            }
        }

        mapEditor.currentTilemap = 0;
    }

    public void ResetEditor()
    {
        int i;
        mapEditor = FindFirstObjectByType<MapEditor>();
        colorPalette = FindFirstObjectByType<ColorPalette>();
        dropdown = FindFirstObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);

        //set start and end outside of the map
        mapEditor.AddTile(-10, -10, 8, -2, true);
        mapEditor.AddTile(-1, -1, 9, -2, true);

        foreach (int t in mapEditor.tilemaps.getIndexes()) //reset every tilemap
            for (i = 0; i < 32; i++)
                for (int j = 0; j < 16; j++)
                    //mapEditor.tilemaps.at(t).SetTile(new Vector3Int(i, j, 0), clear);
                    mapEditor.RemoveAllTileAtThisPositon(i, j);

        for (i = 0; i < mapEditor.inversePairs.Count; i++)
        {
            Destroy(mapEditor.inversePairs[i]); //reset inversePairs
            mapEditor.inversePairs = new List<GameObject>();
        }

        //restore entire outside wall
        for (i = 0; i < 31; i++) //up
        {
            mapEditor.outsideWallTilemap.SetTile(new Vector3Int(i, 16, 0), mapEditor.basicTile);
        }
        for (i = 0; i < 31; i++) //down
        {
            mapEditor.outsideWallTilemap.SetTile(new Vector3Int(i, -1, 0), mapEditor.basicTile);
        }
        for (i = -1; i < 17; i++) //left
        {
            mapEditor.outsideWallTilemap.SetTile(new Vector3Int(-1, i), mapEditor.basicTile);
        }
        for (i = -1; i < 17; i++) //right
        {
            mapEditor.outsideWallTilemap.SetTile(new Vector3Int(30, i), mapEditor.basicTile);
        }
    }

    public void KillAllTheChildren(Transform p)
    {
        foreach (Transform child in p)
        {
            if (child.gameObject.GetComponent<DontDestroyThisObject>() != null) continue;
            GameObject.Destroy(child.gameObject);
        }
    }

    public void KillAllTheTilemapChildren(Transform p, MapData d)
    {
        int i;
        foreach (Transform child in p)
        {
            for (i = 0; i < d.colors.Length; i++)
            {
                if (SameColor(d.colors[i].c(), child.gameObject.GetComponent<Tilemap>().color) || SameColor(new Color32(d.colors[i].r, d.colors[i].g, d.colors[i].b, 0), child.gameObject.GetComponent<Tilemap>().color))
                {
                    for (int j = 0; j < d.row; j++)
                    {
                        for (int k = 0; k < d.column; k++)
                        {
                            child.gameObject.GetComponent<Tilemap>().SetTile(new Vector3Int(k, j, 0), clear);
                        }
                    }
                    break;
                }
            }
            if (i != d.colors.Length || child.gameObject.GetComponent<DontDestroyThisObject>() != null) continue;
            GameObject.Destroy(child.gameObject);
        }
    }

    public void LoadMap(bool isStart) => LoadMap(null, isStart); //this overload still loads "index"
    public void LoadMap(string mapToLoad = null, bool isStart = true)
    {
        if (mapToLoad == null) mapToLoad = index;
        MapData data = SaveLoadMaps.LoadMap(mapToLoad);

        if (data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        Tilemap white = null;
        Tilemap[] tm = FindObjectsOfType<Tilemap>();
        for(int i = 0; i < tm.Length; i++)
        {
            if (SameColor(tm[i].color, new Color32(255, 255, 255, 255)) || SameColor(tm[i].color, new Color32(255, 255, 255, 0))) white = tm[i];
        }

        WallManager wm = FindFirstObjectByType<WallManager>();
        for (int j = 0; j < data.row; j++)
        {
            for (int k = 0; k < data.column; k++)
            {
                white.SetTile(new Vector3Int(k, j, 0), clear);
            }
        }

        FindFirstObjectByType<WallManager>().ResetThings();
        KillAllTheChildren(thingParent);
        KillAllTheChildren(FindFirstObjectByType<WallManager>().decoParent);
        KillAllTheTilemapChildren(tilemapParent, data);
            

        wm.wallPositions = new int[data.column][];
        hasTile = new bool[data.column][];
        hasWhiteWall = new bool[data.column][];
        for (int i = 0; i < data.column; i++)
        {
            hasTile[i] = new bool[data.row];
            hasWhiteWall[i] = new bool[data.row];
            wm.wallPositions[i] = new int[data.row];

            for (int j = 0; j < data.row; j++)
            {
                wm.wallPositions[i][j] = -1;
            }
        }

        int whiteIndex = 0;

        //add the colors, if needed change the index
        for (int i = 0; i < data.colors.Length; i++)
        {
            if (c(data, data.colors[i].index) == -1)
            {
                if(!wm.colors.exist(data.colors[i].index)) wm.colors.add(data.colors[i].c(), data.colors[i].index);
                else wm.colors.add(data.colors[i].c(), wm.colors.maxIndex() + 1);

                for (int j = 0; j < data.activeAtStart.Length; j++)
                {
                    if (data.activeAtStart[j].index == data.colors[i].index)
                    {
                        wm.activeAtStart.add(c(data, data.activeAtStart[j].index), data.activeAtStart[j].isActive);
                        wm.colors.setVisible(c(data, data.activeAtStart[j].index), data.activeAtStart[j].isActive);
                        break;
                    }
                }
            }
            if (SameColor(wm.colors.at(c(data, data.colors[i].index)), new Color32(255, 255, 255, 255))) whiteIndex = c(data, data.colors[i].index);
        }

        //set the informations
        for (int i = 0; i < data.colors.Length; i++)
        {
            WallObjects[] wo = FindObjectsOfType<WallObjects>(includeInactive: true);
            int ind = wm.colors.searchColor(data.colors[i].c());
            Tilemap t = null;
            for(int j = 0; j < wo.Length; j++)
            {
                if (wo[j].colorIndex == ind)
                {
                    t = wo[j].gameObject.GetComponent<Tilemap>();
                }
            }
            if(t == null)
            {
                GameObject a = Instantiate(tilemapPrefab, tilemapParent);
                t = a.GetComponent<Tilemap>();
                a.GetComponent<WallObjects>().Create(ind);
            }
            for (int j = 0; j < data.row; j++)
            {
                for (int k = 0; k < data.column; k++)
                {
                    if (data.colorIndex[k][j] == data.colors[i].index)
                    {
                        if (SameColor(FindFirstObjectByType<WallManager>().colors.at(ind), new Color32(255, 255, 255, 255))) t.SetTile(new Vector3Int(k, j, 0), tileBase);
                        else t.SetTile(new Vector3Int(k, j, 0), tileColorBase);

                        hasTile[k][j] = true;
                        if (ind == whiteIndex) hasWhiteWall[k][j] = true;
                        wm.wallPositions[k][j] = ind;
                    }
                }
            }

            Gate[] gates = FindObjectsOfType<Gate>(includeInactive: true);
            ind = wm.colors.searchColor(data.colors[i].c());
            Tilemap gateTilemap = null;
            for (int j = 0; j < gates.Length; j++)
            {
                if (gates[j].colorIndex == ind)
                {
                    gateTilemap = gates[j].gameObject.GetComponent<Tilemap>();
                }
            }
            if (gateTilemap == null)
            {
                GameObject gate = Instantiate(gatePrefab, tilemapParent);
                gate.GetComponent<Gate>().CreateNew(ind);
                gateTilemap = gate.GetComponent<Tilemap>();
            }
            for (int j = 0; j < data.row; j++)
            {
                for (int k = 0; k < data.column; k++)
                {
                    if (data.gate[k][j] == data.colors[i].index)
                    {
                        gateTilemap.SetTile(new Vector3Int(k, j, 0), gateBase);
                        GameObject b = Instantiate(gateLightPrefab, thingParent);
                        b.GetComponent<GateLight>().Create(ind, k, j);

                        hasTile[k][j] = true;
                    }
                }
            }
        }

        for (int i = 0; i < data.buttonForCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonForCubePrefab, thingParent);
            ButtonsForCube bb = b.GetComponent<ButtonsForCube>();
            bb.CreateNew(c(data, data.buttonForCubes[i].color), c(data, data.buttonForCubes[i].interactiveColor), data.buttonForCubes[i].x, data.buttonForCubes[i].y);

            /*if (c(data, data.buttonForCubes[i].color) == whiteIndex) */ hasTile[data.buttonForCubes[i].x][data.buttonForCubes[i].y] = true;
        }
        for (int i = 0; i < data.portals.Length; i++)
        {
            GameObject b = Instantiate(portalPrefab, thingParent);
            Portal bb = b.GetComponent<Portal>();
            bb.CreateNew(c(data, data.portals[i].color), data.portals[i].interactiveColor, data.portals[i].x, data.portals[i].y);

            /*if (c(data, data.portals[i].color) == whiteIndex) */ hasTile[data.portals[i].x][data.portals[i].y] = true;
        }
        for (int i = 0; i < data.lever.Length; i++)
        {
            GameObject b = Instantiate(leverPrefab, thingParent);
            Lever bb = b.GetComponent<Lever>();
            bb.CreateNew(c(data, data.lever[i].color), c(data, data.lever[i].interactiveColor), data.lever[i].x, data.lever[i].y);

            /*if (c(data, data.lever[i].color) == whiteIndex) */ hasTile[data.lever[i].x][data.lever[i].y] = true;
        }
        for (int i = 0; i < data.buttons.Length; i++)
        {
            GameObject b = Instantiate(buttonPrefab, thingParent);
            Buttons bb = b.GetComponent<Buttons>();
            bb.CreateNew(c(data, data.buttons[i].color), c(data, data.buttons[i].interactiveColor), data.buttons[i].x, data.buttons[i].y, data.buttons[i].activateAtBeingActive);

            /*if (c(data, data.buttons[i].color) == whiteIndex) */ hasTile[data.buttons[i].x][data.buttons[i].y] = true;
        }
        for (int i = 0; i < data.buttonTimerCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonTimerCubePrefab, thingParent);
            ButtonTimerCube bb = b.GetComponent<ButtonTimerCube>();
            bb.CreateNew(c(data, data.buttonTimerCubes[i].color), c(data, data.buttonTimerCubes[i].interactiveColor), data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y, data.buttonTimerCubes[i].timer);

            /*if (c(data, data.buttonTimerCubes[i].color) == whiteIndex)*/ hasTile[data.buttonTimerCubes[i].x][data.buttonTimerCubes[i].y] = true;
        }

        for (int i = 0; i < data.inversePairs.Length; i++)
        {
            FindFirstObjectByType<WallManager>().inversColor.add(c(data, data.inversePairs[i].index1), c(data, data.inversePairs[i].index2));
        }

        //if (isStart) { 
        //    FindFirstObjectByType<WallManager>().SetDefaultState();
        //}
        //else FindFirstObjectByType<WallManager>().SetCurrentState();

        if (isStart) FindFirstObjectByType<movement>().needUpdate = 2;
        else FindFirstObjectByType<movement>().needUpdate = 1;

        if(isStart) FindFirstObjectByType<CubePlacer>().gameObject.GetComponent<movement>().SetPosition(data.startx, data.starty);
        FindFirstObjectByType<WallManager>().endThing.SetPosition(data.endx, data.endy);
        if(data.endx >= 0 && data.endy >= 0) hasTile[data.endx][data.endy] = true;
        FindFirstObjectByType<WallManager>().SetDecoDemons();
        SetDeco();
        SearchForNPCs(mapToLoad);
    }

    public int c(MapData data, int colorIndex)
    {
        WallManager wm = FindFirstObjectByType<WallManager>();
        for(int i = 0; i < data.colors.Length; i++)
        {
            if (data.colors[i].index == colorIndex)
            {
                return wm.colors.searchColor(data.colors[i].c());
            }
        }
        return -1;
    }

    public void SetDeco()
    {
        int xRandom = UnityEngine.Random.Range(-50, 50);
        int yRandom = UnityEngine.Random.Range(-50, 50);
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                decoTilemap.SetTile(new Vector3Int(i + xRandom, j + yRandom, 0), decoBase);
                for (int k = 0; k < decos.Count; k++)
                {
                    if (decos[k].sprite == decoTilemap.GetSprite(new Vector3Int(i + xRandom, j + yRandom, 0)))
                    {
                        if (!hasTile[i][j])
                        {
                            if (j == 0 && FindFirstObjectByType<MultipleLevel>().CurrentLevel().missingDown != null && !FindFirstObjectByType<MultipleLevel>().CurrentLevel().missingDown.Contains(i))
                            {
                                decos[k].Create(i, j);

                            }
                            else if (j != 0 && hasWhiteWall[i][j - 1])
                            {
                                decos[k].Create(i, j);

                            }
                        }
                    }
                }
            }
        }
    }

    public void SearchForNPCs(string mapName)
    {
        for(int i = 0; i < datas.Count; i++)
        {
            for(int j = 0; j < datas[i].data.Count; j++)
            {
                if (datas[i].data[j].mapName == mapName)
                {
                    deactivateDecos?.Invoke(datas[i].data[j].x, datas[i].data[j].y);
                    datas[i].data[j].Summon();
                }
            }
        }
    }

    public bool SameColor(Color32 a, Color32 b)
    {
        return (a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a);
    }
}
