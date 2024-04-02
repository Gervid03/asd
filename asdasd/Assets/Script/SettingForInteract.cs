using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingForInteract : MonoBehaviour
{
    public int x, y;
    public Text coordinates;
    public int index;
    public Image color;
    public int indexColorInteract;
    public Image colorInteract;
    public int timer;
    public int portalIndex;
    public bool activate;
    public void Set(int x1, int y1, int i, Color c)
    {
        coordinates.text = "(" + x1 + ", " + y1 + ")";
        index = i;
        color.color = c;
    }

    public void SetInteractColor()
    {
        ColorPalette cp = FindFirstObjectByType<ColorPalette>();
        if(cp != null)
        {
            indexColorInteract = cp.selectedButton.index;
            colorInteract.color = cp.selectedButton.color;
        }
    }

    public void SetTimer(string t)
    {
        timer = int.Parse(t);
    }
    
    public void SetPortalIndex(string pt)
    {
        portalIndex = int.Parse(pt);
    }

    public void SetActivate(bool a)
    {
        activate = a;
    }

}
