using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GateLight : MonoBehaviour
{
    public int colorIndex;
    public Light2D light2D;

    private void Awake()
    {
        WallManager.disableColor += DontBeActive;
        WallManager.activateColor += BeActive;
    }

    public void Create(int color, int x, int y)
    {
        colorIndex = color;
        light2D.color = FindFirstObjectByType<WallManager>().GetColor(color);
        SetPosition(x, y);
    }

    public void SetPosition(int x, int y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex) return;
        gameObject.SetActive(false);
    }

    public void BeActive(int c)
    {
        if (c != colorIndex) return;
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        WallManager.disableColor -= DontBeActive;
        WallManager.activateColor -= BeActive;
    }
}
