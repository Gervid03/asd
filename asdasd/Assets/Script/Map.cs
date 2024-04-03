using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapData;

public class Map : MonoBehaviour
{
    public int index;
    public int[][] colorIndex; //to which color belongs this wall
    public List<Color> colors; //color of the indexes
    //start, end positions needed
    public List<Portal> portals;
    public List<Lever> lever;
    public List<Button> buttons;
    public List<ButtonForCube> buttonForCubes;
    public List<bool> activeAtStart; //is the index active at the beginning
    public Transform tilemapParent;
    public Transform thingParent;
    public GameObject tilemapPrefab;
    public TileBase tileBase;

    public void SaveMap()
    {
        SaveLoadMaps.SaveMap(this);
    }

    public void LoadMap()
    {
        MapData data = SaveLoadMaps.LoadMap(index);

        FindFirstObjectByType<WallManager>().colors = data.colors;

        //set the informations
        for(int i = 0; i < data.colors.Count; i++)
        {
            GameObject a = Instantiate(tilemapPrefab, tilemapParent);
            a.GetComponent<WallObjects>().Create(i);
            Tilemap t = a.GetComponent<Tilemap>();
            for (int j = 0; j < data.row; i++)
            {
                for (int k = 0; k < data.column; i++)
                {
                    if (colorIndex[i][j] == i)
                    {
                        t.SetTile(new Vector3Int(i, j, 0), tileBase);
                    }
                }
            }
        }

        
    }
}
