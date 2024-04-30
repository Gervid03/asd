using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorDisplayButton : MonoBehaviour
{
    public int index;
    public Color color;
    public GameObject toggle;

    private void Start()
    {
        ColorPalette.deleteColor += Suicide;
        ColorPalette.modifyColor += Modify;
    }

    public void OnClick()
    {
        FindFirstObjectByType<MapEditor>().ChangeColor(index);

        ColorPalette colorPalette = FindFirstObjectByType<ColorPalette>();
        colorPalette.selectedButton = this;
        colorPalette.UpdateColorCarousel();
    }

    public void Suicide(int recipient)
    {
        if (recipient == index) //if the message is for me
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        ColorPalette.deleteColor -= Suicide;
        ColorPalette.modifyColor -= Modify;
    }

    public void Modify(int recipient, Color newColor)
    {
        if (recipient == index) //if the message is for me
        {
            ColorTweaker colorTweaker = FindFirstObjectByType<ColorTweaker>();
            color = colorTweaker.color;
            this.GetComponent<Image>().color = colorTweaker.color;
            toggle.GetComponent<SetDefaultState>().colorDisplay.color = colorTweaker.color;
        }
    }
}
