using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefaultState : MonoBehaviour
{
    public int colorIndex;
    public MapEditor mapEditor;
    public GameObject colorDisplay;

    private void Start()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
    }

    public void OnClick(bool toggle)
    {
        mapEditor.tilemaps.changeVisibleAtBeginning(colorIndex, toggle);
    }
}
