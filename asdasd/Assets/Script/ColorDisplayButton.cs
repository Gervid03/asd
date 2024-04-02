using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorDisplayButton : MonoBehaviour
{
    public int index;
    public Color color;
    public void OnClick()
    {
        FindFirstObjectByType<MapEditor>().ChangeColor(index);

        ColorPalette colorPalette = GetComponentInParent<ColorPalette>();
        colorPalette.selectedButton = this;
    }
}
