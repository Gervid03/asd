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
        mapEditor = FindFirstObjectByType<MapEditor>();
        colorPalette = FindFirstObjectByType<ColorPalette>();
        ColorPalette.modifyColor += Modify;
        ColorPalette.deleteColor += Suicide;
    }

    private void OnDestroy()
    {
        ColorPalette.modifyColor -= Modify;
        ColorPalette.deleteColor -= Suicide;
    }

    public void Modify(int recipient, Color32 newColor)
    {
        if (recipient == index) //if the message is for me
        {
            this.GetComponent<Image>().color = FindFirstObjectByType<ColorTweaker>(FindObjectsInactive.Include).color;
        }
    }

    public void Suicide(int recipient)
    {
        if (recipient == index) //if the message is for me
        {
            GetComponentInParent<InversePair>().CommitSucide();
        }
    }

    public void Clicked() => ChangeColor();
    public void ChangeColor(ColorDisplayButton selectedButtonIndex = null, bool noHistory = false)
    {
        if (selectedButtonIndex == null) selectedButtonIndex = colorPalette.selectedButton;
        if (selectedButtonIndex.index == 0) return;
        if (mapEditor.inverseColor[selectedButtonIndex.index] != -1 || other.index == selectedButtonIndex.index)
        {
            ColorAlreadyHasAnInverse();
            return;
        }
        if (index != -1)
        {
            mapEditor.inverseColor[index] = -1;
        }

        //register Inverse Pair modification
        if (!noHistory)
        {
            InversePair inversePair = this.gameObject.transform.parent.GetComponent<InversePair>();
            if (inversePair.b1 == this) //check which side is which
            {
                FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModInversePair(
                    inversePair.b1.GetComponent<Image>().sprite == inversePair.b1.wall,
                    inversePair.b2.GetComponent<Image>().sprite == inversePair.b2.wall,
                    GetComponent<Image>().color, selectedButtonIndex.color,
                    other.GetComponent<Image>().color, other.GetComponent<Image>().color));
            }
            else
            {
                FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModInversePair(
                    inversePair.b1.GetComponent<Image>().sprite == inversePair.b1.wall,
                    inversePair.b2.GetComponent<Image>().sprite == inversePair.b2.wall,
                    other.GetComponent<Image>().color, other.GetComponent<Image>().color,
                    GetComponent<Image>().color, selectedButtonIndex.color));
            }
        }

        this.GetComponent<Image>().sprite = wall;

        index = selectedButtonIndex.index;
        this.GetComponent<Image>().color = selectedButtonIndex.color;

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
