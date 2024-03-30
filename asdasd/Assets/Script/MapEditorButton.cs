using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorButton : MonoBehaviour
{
    public int x, y;

    public void Use()
    {
        FindFirstObjectByType<MapEditor>().Use(x, y);
    }
}
