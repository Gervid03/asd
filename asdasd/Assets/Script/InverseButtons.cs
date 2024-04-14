using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InverseButton : MonoBehaviour
{
    public int index;
    public InverseButton other;
    public MapEditor mapEditor;
    public ColorPalette colorPalette;

    private void Awake()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
        colorPalette = FindAnyObjectByType<ColorPalette>();
        index = -1;
        this.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
    }

    public void Clicked()
    {
        if (mapEditor.inverseColor[colorPalette.selectedButton.index] != -1 || other.index == colorPalette.selectedButton.index)
        {
            ColorAlreadyHasAnInverse();
            return;
        }
        if (index != -1)
        {
            mapEditor.inverseColor[index] = -1;
        }
        index = colorPalette.selectedButton.index;
        this.GetComponent<Image>().color = colorPalette.selectedButton.color;

        mapEditor.inverseColor[index] = other.index;

        if (other.index != -1)
        {
            mapEditor.inverseColor[other.index] = index;
        }
    }

    public void ColorAlreadyHasAnInverse()
    {
        Debug.Log("Color already has an inverse!");
    }
}
