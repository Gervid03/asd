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
    public bool isButtonsForCube;
    public bool isPortal;
    public bool isLever;
    public bool isButton;
    public bool isButtonTimerCube;
    public Toggle toggle;
    public Sprite warning;

    private void Awake()
    {
        ColorPalette.modifyColor += Modify;
        ColorPalette.deleteColor += Suicide;
    }

    public void Suicide(int recipient)
    {
        if (recipient == index)
        {
            Destroy(this.gameObject);
        }
        if (recipient == indexColorInteract)
        {
            colorInteract.sprite = warning;
            colorInteract.color = Color.white;
        }
    }

    public void Modify(int recipient, Color newColor)
    {
        if (recipient == index)
        {
            color.color = newColor;
        }
        if (recipient == indexColorInteract)
        {
            colorInteract.color = newColor;
        }
    }

    public void Set(int x1, int y1, int i, Color c, int interactColor, bool active = false)
    {
        coordinates.text = "(" + x1 + ", " + y1 + ")";
        x = x1;
        y = y1;
        index = i;
        color.color = c;
        if (FindFirstObjectByType<MapEditor>().tilemaps.at(interactColor) != null && interactColor != -1 && colorInteract != null)
        {
            colorInteract.sprite = null;
            colorInteract.color = FindFirstObjectByType<MapEditor>().tilemaps.at(interactColor).color;
        }
        
        indexColorInteract = interactColor;
        if (toggle != null)
        {
            toggle.SetIsOnWithoutNotify(active);
        }
    }

    public void SetInteractColor()
    {
        ColorPalette cp = FindFirstObjectByType<ColorPalette>();
        if(cp != null)
        {
            colorInteract.sprite = null;
            indexColorInteract = cp.selectedButton.index;
            colorInteract.color = cp.selectedButton.color;
        }
    }

    public void SetTimer(string t)
    {
        if (t == null) return;
        timer = int.Parse(t);
    }

    public void SetTimerAtLoading(int t)
    {
        timer = t;
        GetComponentInChildren<InputField>().text = t + "";
    }

    public void SetPortalAtLoading(int t)
    {
        portalIndex = t;
        GetComponentInChildren<InputField>().text = t + "";
    }

    public void SetPortalIndex(string pt)
    {
        if(pt == null) return;
        portalIndex = int.Parse(pt);
    }

    public void SetActivate(bool a)
    {
        activate = a;
    }

    public void CommitSuicide()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        ColorPalette.modifyColor -= Modify;
        ColorPalette.deleteColor -= Suicide;
        FindAnyObjectByType<ColorPalette>().AdjustHeight();
    }
}
