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
        }
    };

    private void Start()
    {
        Vault vault = FindFirstObjectByType<Vault>();
        if (vault == null) return;
        if (vault.intent == Vault.Intent.loadMapToLoad)
        {
            Debug.Log("Loading in: " + vault.mapToLoad);
            index = vault.mapToLoad;
            LoadIntoEditor();
        }
    }

    public void SaveMap()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
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
        dropdown = FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        SetIndex(dropdown.options[index2].text);
    }

    public void UpdateSelectedDropdownOption(string name)
    {
        dropdown = FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        for(int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == name)
            {
                dropdown.value = i;
            }
        }
    }

    public void VaultAndLoad()
    {
        FindFirstObjectByType<Vault>().Load();
        
    }

    public void LoadIntoEditorInit()
    {
        Vault vault = FindFirstObjectByType<Vault>();
        vault.mapToLoad = index;
        vault.intent = Vault.Intent.loadMapToLoad;
        Debug.Log("Vaulting: " + vault.mapToLoad + ", " + vault.intent.ToString());

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload the scene
    }

    public void LoadIntoEditor()
    {
        int i, j;
        mapEditor = FindAnyObjectByType<MapEditor>();
        colorPalette = FindAnyObjectByType<ColorPalette>();
        dropdown = FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        Vault vault = FindAnyObjectByType<Vault>();

        if (index == "")
        {
            index = dropdown.options[0].text;
        }

        for (i = 0; i < dropdown.options.Count; i++)
        {
            if (index == "temp")
            {
                if (dropdown.options[i].text == vault.mapUnderConstruction)
                {
                    dropdown.value = i;
                    saveName.text = vault.mapUnderConstruction;
                    break;
                }
            }
            else
            {
                if (dropdown.options[i].text == index)
                {
                    dropdown.value = i;
                    saveName.text = vault.mapToLoad;
                    break;
                }
            }
        }

        MapData data = SaveLoadMaps.LoadMap(index);

        if (data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        colorPalette.KillAllTheChildren();
        for (i = 0; i < data.colors.Length; i++)
        {
            if (data.colors[i].r == 1 && data.colors[i].g == 1 && data.colors[i].b == 1) continue; //feheret nem bantjuk!
            colorPalette.CreateColor(data.colors[i].c(), data.colors[i].index);
        }

        mapEditor.columns = data.column;
        mapEditor.rows = data.row;

        mapEditor.currentTool = 1;

        for (i = 0; i < data.colorIndex.Length; i++)
        {
            for (j = 0; j < data.colorIndex[i].Length; j++)
            {
                if (data.colorIndex[i][j] == -1)
                {
                    continue;
                }

                mapEditor.currentTilemap = data.colorIndex[i][j];

                mapEditor.AddTile(i, j);
            }
        }

        mapEditor.currentTool = 2; //button
        for (i = 0; i < data.buttons.Length; i++)
        {
            mapEditor.currentTilemap = data.buttons[i].color;
            mapEditor.AddTile(data.buttons[i].x, data.buttons[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.buttons[i].x, data.buttons[i].y, data.buttons[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.buttons[i].color).color, data.buttons[i].interactiveColor, data.buttons[i].activateAtBeingActive);
        }
        mapEditor.currentTool = 3; //buttonForCube
        for (i = 0; i < data.buttonForCubes.Length; i++)
        {
            mapEditor.currentTilemap = data.buttonForCubes[i].color;
            mapEditor.AddTile(data.buttonForCubes[i].x, data.buttonForCubes[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.buttonForCubes[i].x, data.buttonForCubes[i].y, data.buttonForCubes[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.buttonForCubes[i].color).color, data.buttonForCubes[i].interactiveColor);
        }
        mapEditor.currentTool = 4; //lever
        for (i = 0; i < data.lever.Length; i++)
        {
            mapEditor.currentTilemap = data.lever[i].color;
            mapEditor.AddTile(data.lever[i].x, data.lever[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.lever[i].x, data.lever[i].y, data.lever[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.lever[i].color).color, data.lever[i].interactiveColor);
        }
        mapEditor.currentTool = 5; //portal
        for (i = 0; i < data.portals.Length; i++)
        {
            //Debug.Log(data.portals[i].x + " asd " + data.portals[i].y + " asd " + data.portals[i].color + ' ' + data.portals[i].interactiveColor);
            mapEditor.currentTilemap = data.portals[i].color;
            mapEditor.AddTile(data.portals[i].x, data.portals[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.portals[i].x, data.portals[i].y, data.portals[i].color, mapEditor.tilemaps.at(data.portals[i].color).color, data.portals[i].interactiveColor);
            mapEditor.infos[mapEditor.infos.Count - 1].SetPortalAtLoading(data.portals[i].interactiveColor);
        }
        mapEditor.currentTool = 6; //gate
        for (i = 0; i < data.gate.Length; i++)
        {
            for (j = 0; j < data.gate[i].Length; j++)
            {
                if (data.gate[i][j] == -1)
                {
                    continue;
                }

                mapEditor.currentTilemap = data.gate[i][j];

                mapEditor.AddTile(i, j);
            }
        }
        mapEditor.currentTool = 7; //buttonForTimerCube
        for (i = 0; i < data.buttonTimerCubes.Length; i++)
        {
            mapEditor.currentTilemap = data.buttonTimerCubes[i].color;
            mapEditor.AddTile(data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].Set(data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y, data.buttonTimerCubes[i].color, FindFirstObjectByType<MapEditor>().tilemaps.at(data.buttonTimerCubes[i].color).color, data.buttonTimerCubes[i].interactiveColor);
            mapEditor.infos[mapEditor.infos.Count - 1].SetTimerAtLoading(data.buttonTimerCubes[i].timer);
        }

        for (i = 0; i < data.inversePairs.Length; i++)
        {
            mapEditor.CreateInversePair();
            InverseButton[] buttons = mapEditor.inversePairs[mapEditor.inversePairs.Count - 1].GetComponentsInChildren<InverseButton>();

            buttons[0].index = data.inversePairs[i].index1;
            buttons[0].GetComponent<Image>().color = mapEditor.tilemaps.at(data.inversePairs[i].index1).color;
            buttons[1].index = data.inversePairs[i].index2;
            buttons[1].GetComponent<Image>().color = mapEditor.tilemaps.at(data.inversePairs[i].index2).color;
        }

        for (i = 0; i < data.activeAtStart.Length; i++)
        {
            mapEditor.tilemaps.changeVisibleAtBeginning(data.activeAtStart[i].index, data.activeAtStart[i].isActive);
        }

        mapEditor.currentTool = 8;
        mapEditor.AddTile(data.startx, data.starty);

        mapEditor.currentTool = 9;
        mapEditor.AddTile(data.endx, data.endy);

        for (i = 0; i < data.activeAtStart.Length; i++)
        {
            mapEditor.tilemaps.changeVisibleAtBeginning(data.activeAtStart[i].index, data.activeAtStart[i].isActive);
        }

        for (i = 0; i < data.activeAtStart.Length; i++)
        {
            for (j = 0; j < colorPalette.colors.Count; j++)
            {
                if (colorPalette.colors[j].index == data.activeAtStart[i].index)
                {
                    if (colorPalette.colors[j].toggle.GetComponentInChildren<Toggle>() == null) Debug.Log(i + " " + j);
                    colorPalette.colors[j].toggle.GetComponentInChildren<Toggle>().SetIsOnWithoutNotify(data.activeAtStart[i].isActive);
                    break;
                }
            }
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

    public void LoadMap(bool isStart = true)
    {
        MapData data = SaveLoadMaps.LoadMap(index);

        if (data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        KillAllTheChildren(thingParent);
        KillAllTheChildren(FindFirstObjectByType<WallManager>().decoParent);
        KillAllTheChildren(tilemapParent);
        FindFirstObjectByType<WallManager>().ResetThings();

        hasTile = new bool[data.column][];
        hasWhiteWall = new bool[data.column][];
        for (int i = 0; i < data.column; i++)
        {
            hasTile[i] = new bool[data.row];
            hasWhiteWall[i] = new bool[data.row];
        }

        int whiteIndex = 0;

        //FindFirstObjectByType<WallManager>().colors.clear();
        for (int i = 0; i < data.colors.Length; i++)
        {
            bool vis = true;
            for(int j = 0; j < data.activeAtStart.Length; j++)
            {
                if (data.activeAtStart[j].index == data.colors[i].index) vis = data.activeAtStart[j].isActive;
            }
            FindFirstObjectByType<WallManager>().colors.add(data.colors[i].c(), data.colors[i].index, vis);
            if (data.colors[i].c() == Color.white) whiteIndex = data.colors[i].index;
        }

        //set the informations
        for (int i = 0; i < data.colors.Length; i++)
        {
            GameObject a = Instantiate(tilemapPrefab, tilemapParent);
            a.GetComponent<WallObjects>().Create(i);
            Tilemap t = a.GetComponent<Tilemap>();
            for (int j = 0; j < data.row; j++)
            {
                for (int k = 0; k < data.column; k++)
                {
                    if (data.colorIndex[k][j] == i)
                    {
                        if (FindFirstObjectByType<WallManager>().colors.at(i) == Color.white) t.SetTile(new Vector3Int(k, j, 0), tileBase);
                        else t.SetTile(new Vector3Int(k, j, 0), tileColorBase);

                        if (i == whiteIndex) hasTile[k][j] = true;
                        if (i == whiteIndex) hasWhiteWall[k][j] = true;
                    }
                }
            }

            GameObject gate = Instantiate(gatePrefab, tilemapParent);
            gate.GetComponent<Gate>().CreateNew(i);
            Tilemap gateTilemap = gate.GetComponent<Tilemap>();
            for (int j = 0; j < data.row; j++)
            {
                for (int k = 0; k < data.column; k++)
                {
                    if (data.gate[k][j] == i)
                    {
                        gateTilemap.SetTile(new Vector3Int(k, j, 0), gateBase);
                        GameObject b = Instantiate(gateLightPrefab, thingParent);
                        b.GetComponent<GateLight>().Create(i, k, j);

                        hasTile[k][j] = true;
                    }
                }
            }
        }

        for (int i = 0; i < data.buttonForCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonForCubePrefab, thingParent);
            ButtonsForCube bb = b.GetComponent<ButtonsForCube>();
            bb.CreateNew(data.buttonForCubes[i].color, data.buttonForCubes[i].interactiveColor, data.buttonForCubes[i].x, data.buttonForCubes[i].y);

            if (data.buttonForCubes[i].color == whiteIndex) hasTile[data.buttonForCubes[i].x][data.buttonForCubes[i].y] = true;
        }
        for (int i = 0; i < data.portals.Length; i++)
        {
            GameObject b = Instantiate(portalPrefab, thingParent);
            Portal bb = b.GetComponent<Portal>();
            bb.CreateNew(data.portals[i].color, data.portals[i].interactiveColor, data.portals[i].x, data.portals[i].y);

            if (data.portals[i].color == whiteIndex) hasTile[data.portals[i].x][data.portals[i].y] = true;
        }
        for (int i = 0; i < data.lever.Length; i++)
        {
            GameObject b = Instantiate(leverPrefab, thingParent);
            Lever bb = b.GetComponent<Lever>();
            bb.CreateNew(data.lever[i].color, data.lever[i].interactiveColor, data.lever[i].x, data.lever[i].y);

            if (data.lever[i].color == whiteIndex) hasTile[data.lever[i].x][data.lever[i].y] = true;
        }
        for (int i = 0; i < data.buttons.Length; i++)
        {
            GameObject b = Instantiate(buttonPrefab, thingParent);
            Buttons bb = b.GetComponent<Buttons>();
            bb.CreateNew(data.buttons[i].color, data.buttons[i].interactiveColor, data.buttons[i].x, data.buttons[i].y, data.buttons[i].activateAtBeingActive);

            if (data.buttons[i].color == whiteIndex) hasTile[data.buttons[i].x][data.buttons[i].y] = true;
        }
        for (int i = 0; i < data.buttonTimerCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonTimerCubePrefab, thingParent);
            ButtonTimerCube bb = b.GetComponent<ButtonTimerCube>();
            bb.CreateNew(data.buttonTimerCubes[i].color, data.buttonTimerCubes[i].interactiveColor, data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y, data.buttonTimerCubes[i].timer);

            if (data.buttonTimerCubes[i].color == whiteIndex) hasTile[data.buttonTimerCubes[i].x][data.buttonTimerCubes[i].y] = true;
        }

        for (int i = 0; i < data.inversePairs.Length; i++)
        {
            FindFirstObjectByType<WallManager>().inversColor.add(data.inversePairs[i].index1, data.inversePairs[i].index2);
            Debug.Log(data.inversePairs[i].index1 + " " + data.inversePairs[i].index2);
        }


        if (isStart) { 
            FindFirstObjectByType<WallManager>().activeAtStart = data.activeAtStart;
            FindFirstObjectByType<WallManager>().SetDefaultState();
        }

        else FindFirstObjectByType<WallManager>().SetCurrentState();

        if(isStart) FindFirstObjectByType<Player>().gameObject.GetComponent<movement>().SetPosition(data.startx, data.starty);
        FindFirstObjectByType<WallManager>().endThing.SetPosition(data.endx, data.endy);
        hasTile[data.endx][data.endy] = true;
        FindFirstObjectByType<WallManager>().SetDecoDemons();
        SetDeco();
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
}
