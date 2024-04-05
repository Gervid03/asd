using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapData;

public class Map : MonoBehaviour
{
    public int index;
    public int[][] colorIndex; //to which color belongs this wall
    public MapData.ColorForSave[] colors; //color of the indexes
    //start, end positions needed
    public MapData.Portal[] portals;
    public MapData.Lever[] lever;
    public MapData.Button[] buttons;
    public MapData.ButtonForCube[] buttonForCubes;
    public bool[] activeAtStart; //is the index active at the beginning
    public Transform tilemapParent;
    public Transform thingParent;
    public GameObject tilemapPrefab;
    public TileBase tileBase;
    public GameObject buttonForCubePrefab;
    public int tileSize;
    public int tileY; //the minimum y
    public int tileX; //the minimum x
    public int row;
    public int column;

    public void SaveMap()
    {
        FindFirstObjectByType<MapEditor>().GetInfos(this);
        SaveLoadMaps.SaveMap(this);
    }

    public void LoadMap()
    {
        MapData data = SaveLoadMaps.LoadMap(index);

        if(data == null)
        {
            Debug.LogWarning("vilagvege");
            return;
        }

        FindFirstObjectByType<WallManager>().colors.Clear();
        for(int i = 0; i < data.colors.Length; i++)
        {
            FindFirstObjectByType<WallManager>().colors.Add(data.colors[i].c());
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
        }

        /*
        for(int i = 0; i < buttonForCubes.Length; i++)
        {
            GameObject b = Instantiate(buttonForCubePrefab, thingParent);
            ButtonsForCube bb = b.GetComponent<ButtonsForCube>();
            bb.CreateNew(buttonForCubes[i].color, buttonForCubes[i].interactiveColor, buttonForCubes[i].x, buttonForCubes[i].y);
        }*/
    }
}
