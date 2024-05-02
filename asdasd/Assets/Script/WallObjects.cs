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
        Color32 a = tilemap.color;
        a.a = 255;
        tilemap.color = a;
        wallCollider.enabled = true;
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex) return;
        //becomes invisible
        Color32 a = tilemap.color;
        a.a = 0;
        tilemap.color = a;
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
