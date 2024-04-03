using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObjects : MonoBehaviour
{
    public int colorIndex;
    public Tilemap tilemap;
    public Collider2D wallCollider;

    private void Awake()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        wallCollider = gameObject.GetComponent<TilemapCollider2D>();
    }

    private void Start()
    {
    }

    public void SetColor()
    {
        tilemap.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    public void SubscribeToBeAWallObject()
    {
        //informs the manager of the existence
        FindFirstObjectByType<WallManager>().SubscribeToBeAWallObject(this);
    }

    public void BeActive()
    {
        //becomes active and visible
        tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 255);
        wallCollider.enabled = true;
    }

    public void DontBeActive()
    {
        //becomes invisible
        tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0);
        wallCollider.enabled = false;
    }

    public void Create(int color)
    {
        colorIndex = color;
        SetColor();
        SubscribeToBeAWallObject();
    }
}
