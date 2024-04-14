using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GateLight : MonoBehaviour
{
    public Light2D light2D;

    public void Create(int color, int x, int y)
    {
        light2D.color = FindFirstObjectByType<WallManager>().GetColor(color);
        SetPosition(x, y);
    }

    public void SetPosition(int x, int y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }
}
