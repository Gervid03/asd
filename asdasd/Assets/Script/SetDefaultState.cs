using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefaultState : MonoBehaviour
{
    public int colorIndex;
    public MapEditor mapEditor;

    private void Start()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
    }

    public void OnClick()
    {

    }
}
