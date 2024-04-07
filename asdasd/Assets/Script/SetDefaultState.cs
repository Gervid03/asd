using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultState : MonoBehaviour
{
    public int colorIndex;
    public MapEditor mapEditor;
    public Image colorDisplay;

    private void Start()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
    }

    public void OnClick(bool toggle)
    {
        mapEditor.tilemaps.changeVisibleAtBeginning(colorIndex, toggle);
    }
}
