using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapData;

public class Map : MonoBehaviour
{
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
    public int tileSize;
    public float tileX; //the minimum x
    public float tileY; //the minimum y
    public int row;
    public int column;

    public void SaveMap()
    {
        FindFirstObjectByType<MapEditor>().GetInfos(this);
        SaveLoadMaps.SaveMap(this);
    }

    public void SetIndex(string i)
    {
        index = i;
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


        for(int i = 0; i < data.activeAtStart.Length; i++)
        {
            if (data.activeAtStart[i].isActive) FindFirstObjectByType<WallManager>().SetColorActive(data.activeAtStart[i].index);
            else FindFirstObjectByType<WallManager>().SetColorDeactive(data.activeAtStart[i].index);
        }

        FindFirstObjectByType<WallManager>().activeAtStart = data.activeAtStart;
    }
}
