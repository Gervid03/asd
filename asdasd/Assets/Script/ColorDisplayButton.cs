using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDisplayButton : MonoBehaviour
{
    public void OnClick()
    {
        GetComponentInParent<ColorPalette>().selectedButton = this.gameObject;
    }
}
