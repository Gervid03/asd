using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapEditor : MonoBehaviour
{
    public TileBase basicTile;
    public List<Tilemap> tilemaps;
    public int currentTilemap;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) == true)
        {
            Use();
        }
    }

    public void AddTile(int x, int y)
    {
        tilemaps[currentTilemap].SetTile(new Vector3Int(x, y, 0), basicTile);
    }

    public void Use()
    {
        
    }
}
