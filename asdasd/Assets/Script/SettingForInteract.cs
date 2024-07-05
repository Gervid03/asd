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
    public InputField inputField;

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
            colorInteract.color = new Color32(255, 255, 255, 255);
        }
    }

    public void Modify(int recipient, Color32 newColor)
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

    public void Set(int x1, int y1, int i, Color32 c, int interactColor, bool active = false)
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
            FindFirstObjectByType<MapEditor>().tilemaps.changeVisibleAtBeginning(interactColor, active, true);
        }
    }

    public void SetInteractColor(bool noHistory = false)
    {
        BlockData before = new BlockData(this);

        ColorPalette cp = FindFirstObjectByType<ColorPalette>();
        if(cp != null)
        {
            colorInteract.sprite = null;
            indexColorInteract = cp.selectedButton.index;
            colorInteract.color = cp.selectedButton.color;
        }

        if (!noHistory) FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModSetting(this, before, new BlockData(this)));
    }

    public void SetTimer(string t)
    {
        BlockData before = new BlockData(this);

        if (t == null) return;
        timer = int.Parse(t);

        FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModSetting(this, before, new BlockData(this)));
    }

    public void SetTimerAtLoading(int t, bool noHistory = false)
    {
        BlockData before = new BlockData(this);

        timer = t;
        GetComponentInChildren<InputField>().SetTextWithoutNotify(t + "");

        if (!noHistory) FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModSetting(this, before, new BlockData(this)));
    }

    public void SetPortalAtLoading(int t, bool noHistory = false)
    {
        BlockData before = new BlockData(this);

        portalIndex = t;
        GetComponentInChildren<InputField>().SetTextWithoutNotify(t + "");

        if (!noHistory) FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModSetting(this, before, new BlockData(this)));
    }

    public void SetPortalIndex(string pt)
    {
        BlockData before = new BlockData(this);
        
        if(pt == null) return;
        portalIndex = int.Parse(pt);

        FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModSetting(this, before, new BlockData(this)));
    }

    public void SetActivate(bool a, bool noHistory = false)
    {
        BlockData before = new BlockData(this);

        activate = a;

        if (!noHistory) FindFirstObjectByType<HistoryManager>().stacks.Push(new Change.ModSetting(this, before, new BlockData(this)));
    }

    public void CommitSuicide()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        ColorPalette.modifyColor -= Modify;
        ColorPalette.deleteColor -= Suicide;
    }
}
