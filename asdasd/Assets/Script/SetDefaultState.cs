using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultState : MonoBehaviour
{
    public int colorIndex;
    public Image colorDisplay;

    public void OnClick(bool toggle)
    {
        FindFirstObjectByType<MapEditor>().tilemaps.changeVisibleAtBeginning(colorIndex, toggle);
    }
}
