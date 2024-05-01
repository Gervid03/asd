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
        if (colorIndex != -1)
        {
            wallCollider = this.gameObject.AddComponent<TilemapCollider2D>();
            wallCollider.usedByComposite = true;
        }
        tilemap = gameObject.GetComponent<Tilemap>();
        WallManager.disableColor += DontBeActive;
        WallManager.activateColor += BeActive;
    }

    public void SetColor()
    {
        tilemap.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    public void BeActive(int c)
    {
        if (c != colorIndex) return;
        //becomes active and visible
        tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 255);
        wallCollider.enabled = true;
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex) return;
        //becomes invisible
        tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0);
        wallCollider.enabled = false;
    }

    public void Create(int color)
    {
        colorIndex = color;
        SetColor();
    }

    private void OnDestroy()
    {
        WallManager.disableColor -= DontBeActive;
        WallManager.activateColor -= BeActive;
    }
}
