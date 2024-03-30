using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapEditor : MonoBehaviour
{
    public TileBase basicTile;
    public TileBase clear;
    public List<Tilemap> tilemaps;
    public int currentTilemap;
    public GameObject ButtonPrefab;
    public int column;
    public int row;
    public bool add; //false remove

    private void Start()
    {
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < column; j++)
            {
                GameObject gO = Instantiate(ButtonPrefab, transform);
                gO.GetComponent<MapEditorButton>().x = j;
                gO.GetComponent<MapEditorButton>().y = row - i;
            }
        }
    }

    private void Update()
    {

    }

    public void AddTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), basicTile);
    }

    public void RemoveTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), clear);
    }

    public void Use(int x, int y)
    {
        Debug.Log(x + " " + y);
        if (add) AddTile(x, y);
        else RemoveTile(x, y);
    }
}
