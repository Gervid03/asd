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

    public void SaveMap()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
        FindFirstObjectByType<MapEditor>().GetInfos(this);
        SaveLoadMaps.SaveMap(this);
        mapEditor.MapDropdownUpdate();
    }

    public void SetIndex(string i)
    {
        index = i;
    }

    public void UpdateSelectedToLoad(int index2)
    {
        dropdown = FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        SetIndex(dropdown.options[index2].text);
    }

    public void VaultAndLoad()
    {
        FindFirstObjectByType<Vault>().mapToLoad = index;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadIntoEditor()
    {
        int i, j;
        mapEditor = FindAnyObjectByType<MapEditor>();
        colorPalette = FindAnyObjectByType<ColorPalette>();
        dropdown = FindAnyObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);

        if (index == "")
        {
            index = dropdown.options[0].text;
        }

        MapData data = SaveLoadMaps.LoadMap(index);

        if(data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        colorPalette.KillAllTheChildren();
        for (i = 0; i < data.colors.Length; i++)
        {
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
            mapEditor.infos[mapEditor.infos.Count - 1].indexColorInteract = data.buttons[i].interactiveColor;
        }
        mapEditor.currentTool = 3; //buttonForCube
        for (i = 0; i < data.buttonForCubes.Length; i++)
        {
            mapEditor.currentTilemap = data.buttonForCubes[i].color;
            mapEditor.AddTile(data.buttonForCubes[i].x, data.buttonForCubes[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].indexColorInteract = data.buttonForCubes[i].interactiveColor;
        }
        mapEditor.currentTool = 4; //lever
        for (i = 0; i < data.lever.Length; i++)
        {
            mapEditor.currentTilemap = data.lever[i].color;
            mapEditor.AddTile(data.lever[i].x, data.lever[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].indexColorInteract = data.lever[i].interactiveColor;
        }
        mapEditor.currentTool = 5; //portal
        for (i = 0; i < data.portals.Length; i++)
        {
            mapEditor.currentTilemap = data.portals[i].color;
            mapEditor.AddTile(data.portals[i].x, data.portals[i].y);
            mapEditor.infos[mapEditor.infos.Count - 1].portalIndex = data.portals[i].interactiveColor;
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
            mapEditor.infos[mapEditor.infos.Count - 1].indexColorInteract = data.buttonTimerCubes[i].interactiveColor;
            mapEditor.infos[mapEditor.infos.Count - 1].timer = data.buttonTimerCubes[i].timer;
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
    }

    public void LoadMap()
    {
        MapData data = SaveLoadMaps.LoadMap(index);

        if(data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        FindFirstObjectByType<WallManager>().colors.clear();
        for(int i = 0; i < data.colors.Length; i++)
        {
            FindFirstObjectByType<WallManager>().colors.add(data.colors[i].c(), data.colors[i].index);
        }

        //set the informations
        for(int i = 0; i < data.colors.Length; i++)
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
                        t.SetTile(new Vector3Int(k, j, 0), tileBase);
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
                    }
                }
            }
        }

        for(int i = 0; i < data.buttonForCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonForCubePrefab, thingParent);
            ButtonsForCube bb = b.GetComponent<ButtonsForCube>();
            bb.CreateNew(data.buttonForCubes[i].color, data.buttonForCubes[i].interactiveColor, data.buttonForCubes[i].x, data.buttonForCubes[i].y);
        }
        for (int i = 0; i < data.portals.Length; i++)
        {
            GameObject b = Instantiate(portalPrefab, thingParent);
            Portal bb = b.GetComponent<Portal>();
            bb.CreateNew(data.portals[i].color, data.portals[i].interactiveColor, data.portals[i].x, data.portals[i].y);
        }
        for (int i = 0; i < data.lever.Length; i++)
        {
            GameObject b = Instantiate(leverPrefab, thingParent);
            Lever bb = b.GetComponent<Lever>();
            bb.CreateNew(data.lever[i].color, data.lever[i].interactiveColor, data.lever[i].x, data.lever[i].y);
        }
        for (int i = 0; i < data.buttons.Length; i++)
        {
            GameObject b = Instantiate(buttonPrefab, thingParent);
            Buttons bb = b.GetComponent<Buttons>();
            bb.CreateNew(data.buttons[i].color, data.buttons[i].interactiveColor, data.buttons[i].x, data.buttons[i].y, data.buttons[i].activateAtBeingActive);
        }
        for (int i = 0; i < data.buttonTimerCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonTimerCubePrefab, thingParent);
            ButtonTimerCube bb = b.GetComponent<ButtonTimerCube>();
            bb.CreateNew(data.buttonTimerCubes[i].color, data.buttonTimerCubes[i].interactiveColor, data.buttonTimerCubes[i].x, data.buttonTimerCubes[i].y, data.buttonTimerCubes[i].timer);
        }


        for(int i = 0; i < data.inversePairs.Length; i++)
        {
            FindFirstObjectByType<WallManager>().inversColor.add(data.inversePairs[i].index1, data.inversePairs[i].index2);
            Debug.Log(data.inversePairs[i].index1 + " " + data.inversePairs[i].index2);
        }

        for(int i = 0; i < data.activeAtStart.Length; i++)
        {
            if (data.activeAtStart[i].isActive) FindFirstObjectByType<WallManager>().SetColorActive(data.activeAtStart[i].index);
            else FindFirstObjectByType<WallManager>().SetColorDeactive(data.activeAtStart[i].index);
        }

        FindFirstObjectByType<WallManager>().activeAtStart = data.activeAtStart;

        FindFirstObjectByType<Player>().gameObject.GetComponent<movement>().SetPosition(data.startx, data.starty);
        FindFirstObjectByType<WallManager>().endThing.SetPosition(data.endx, data.endy);
    }
}
