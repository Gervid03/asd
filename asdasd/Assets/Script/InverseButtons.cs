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
    public Sprite warning;
    public Sprite wall;

    private void Awake()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
        colorPalette = FindAnyObjectByType<ColorPalette>();
        index = -1;
        this.gameObject.GetComponent<Image>().sprite = warning;
    }

    private void Start()
    {
        ColorPalette.modifyColor += Modify;
        ColorPalette.deleteColor += Suicide;
    }

    private void OnDestroy()
    {
        ColorPalette.modifyColor -= Modify;
        ColorPalette.deleteColor -= Suicide;
        FindAnyObjectByType<ColorPalette>().AdjustHeight();
    }

    public void Modify(int recipient, Color newColor)
    {
        if (recipient == index) //if the message is for me
        {
            this.GetComponent<Image>().color = FindFirstObjectByType<ColorTweaker>().color;
        }
    }

    public void Suicide(int recipient)
    {
        if (recipient == index) //if the message is for me
        {
            GetComponentInParent<Suicide>().CommitSucide();
        }
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

        this.GetComponent<Image>().sprite = wall;

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
        Debug.Log("Color already has an inverse!"); //TODO popup
    }
}
