using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class HistoryManager : MonoBehaviour
{
    public Stacks stacks;
    private bool heldDown;
    private PopUpHandler popUpHandler;

    void Awake()
    {
        stacks = new Stacks();
        popUpHandler = FindFirstObjectByType<PopUpHandler>();
    }

    void Update()
    {
        if (!popUpHandler.popupActive)
        {
            if (Input.GetKeyDown(KeyCode.Y) && !heldDown) //switched z and y!
            {
                heldDown = true;
                stacks.PopUndo();
            }
            else if (Input.GetKeyDown(KeyCode.Z) && !heldDown)
            {
                heldDown = true;
                stacks.PopRedo();
            }
            else
            {
                heldDown = false;
            }
        }
    }

    public class Stacks
    {
        private List<Change> undo;
        private List<Change> redo;
        private const int maxSize = 1024; //when reached the bottom half of the stack is deleted

        public Stacks()
        {
            undo = new List<Change>(maxSize);
            redo = new List<Change>(maxSize);
        }

        private void HalveStack(ref List<Change> s)
        {
            s = s.GetRange(s.Count / 2, s.Count - (s.Count / 2));
        }

        public void Push(Change change, bool keepRedo = false)
        {
            if (undo.Count - 2 >= maxSize) //-2 for safety (:
            {
                HalveStack(ref undo);
            }

            if (!keepRedo)
            {
                redo.Clear();
            }

            Debug.Log("pushed to Undo: " + change.GetType());

            undo.Add(change);
        }

        public void PopUndo()
        {
            if (undo.Count == 0)
            {
                Debug.Log("undo stack is empty!");
                return;
            }

            undo[^1].Undo();
            PushRedo(undo[^1]);
            undo.RemoveAt(undo.Count - 1);
        }

        public void PopRedo()
        {
            if (redo.Count == 0)
            {
                Debug.Log("redo stack is emtpy");
                return;
            }

            redo[^1].Redo();
            Push(redo[^1], true);
            redo.RemoveAt(redo.Count - 1);
        }

        private void PushRedo(Change change)
        {
            Debug.Log("pushed to Redo: " + change.GetType());

            if (redo.Count - 1 >= maxSize) //-1 for safety (:
            {
                HalveStack(ref redo);
            }

            redo.Add(change);
        }
    }
}

public class BlockData
{
    public int x, y;
    public int colorIndex, interactColorIndex;
    public int type;
    public bool activate;
    public int timer, portalIndex;
    public TileBase tile;
    ColorPalette colorPalette;
    MapEditor mapEditor;

    public BlockData(SettingForInteract setting)
    {
        colorPalette = MonoBehaviour.FindFirstObjectByType<ColorPalette>();
        mapEditor = MonoBehaviour.FindFirstObjectByType<MapEditor>();

        x = setting.x;
        y = setting.y;
        colorIndex = setting.index;
        interactColorIndex = setting.indexColorInteract;
        timer = setting.timer;
        portalIndex = setting.portalIndex;
        tile = null;

        if (setting.isButton) type = 2;
        else if (setting.isButtonsForCube) type = 3;
        else if (setting.isLever) type = 4;
        else if (setting.isPortal) type = 5;
        else if (setting.isButtonTimerCube) type = 7;
    }
    public BlockData(int x, int y, int colorIndex, TileBase tile)
    {
        colorPalette = MonoBehaviour.FindFirstObjectByType<ColorPalette>();
        mapEditor = MonoBehaviour.FindFirstObjectByType<MapEditor>();

        this.x = x;
        this.y = y;
        this.colorIndex = colorIndex;
        this.tile = tile;

        if (tile == mapEditor.clear)
        {
            type = -1;
        }
        else if (tile == mapEditor.tools[0].tile || tile == mapEditor.tools[1].tile)
        {
            type = 1;
        }
        else if (tile == mapEditor.tools[6].tile)
        {//gate
            type = 6;
        }
        else if (tile == mapEditor.tools[8].tile)
        {
            type = 8;
        }
        else if (tile == mapEditor.tools[9].tile)
        {
            type = 9;
        }
        else Debug.Log("Tile problems");
    }

    public void ToSetting(SettingForInteract settingToUpdate)
    {
        settingToUpdate.activate = activate;
        settingToUpdate.index = colorIndex;
        settingToUpdate.indexColorInteract = interactColorIndex;
        settingToUpdate.isButton = type == 2;
        settingToUpdate.isButtonsForCube = type == 3;
        settingToUpdate.isLever = type == 4;
        settingToUpdate.isPortal = type == 5;
        settingToUpdate.isButtonTimerCube = type == 7;
        settingToUpdate.timer = timer;
        settingToUpdate.portalIndex = portalIndex;
        settingToUpdate.x = x;
        settingToUpdate.y = y;
        settingToUpdate.coordinates.text = "(" + x + ", " + y + ")";

        List<int> indices = mapEditor.tilemaps.getIndexes();
        for (int i = 0; i < indices.Count; i++)
        {
            if (colorIndex == indices[i])
            {
                settingToUpdate.color.color = colorPalette.colors[i].color;
                if (settingToUpdate.colorInteract != null) settingToUpdate.colorInteract.color = colorPalette.colors[i].color;
                settingToUpdate.Set(x, y, colorIndex, colorPalette.colors[colorIndex].color, interactColorIndex, activate);
                break;
            }
        }
        //maybe there's no need for all this, but safety first!
    }
}


public abstract class Change //TODO border changes
{
    protected MapEditor mapEditor;
    protected ColorPalette colorPalette;
    protected Map map;
    protected ColorTweaker colorTweaker;

    protected Change()
    {
        mapEditor = MonoBehaviour.FindFirstObjectByType<MapEditor>();
        colorPalette = MonoBehaviour.FindFirstObjectByType<ColorPalette>();
        map = MonoBehaviour.FindFirstObjectByType<Map>();
        colorTweaker = MonoBehaviour.FindFirstObjectByType<ColorTweaker>(FindObjectsInactive.Include);

        if (mapEditor == null || colorPalette == null || map == null || colorTweaker == null)
        {
            Debug.Log("smh is null!");
        }
    }

    public abstract void Undo();

    public abstract void Redo();
    


    public class AddTile : Change
    {
        BlockData before, after;

        public AddTile(BlockData before, BlockData after)
        {
            this.before = before;
            this.after = after;
        }

        public override void Redo()
        {
            mapEditor.Use(after.x, after.y, after.type, after.colorIndex, true);
            if (after.type == 2 || after.type == 3 || after.type == 4 || after.type == 5 || after.type == 7) //interactive
            {
                after.ToSetting(mapEditor.infos[^1]);
            }
        }

        public override void Undo()
        {
            if (before.type == -1)
            {
                mapEditor.RemoveAllTileAtThisPositon(before.x, before.y);
                return;
            }

            mapEditor.Use(before.x, before.y, before.type, before.colorIndex, true);
            if (before.type == 2 || before.type == 3 || before.type == 4 || before.type == 5 || before.type == 7) //interactive
            {
                before.ToSetting(mapEditor.infos[^1]);
            }
        }
    }

    public class RemoveTile : Change
    {
        BlockData block;

        public RemoveTile(BlockData block)
        {
            this.block = block;
        }

        public override void Redo()
        {
            mapEditor.RemoveAllTileAtThisPositon(block.x, block.y);
        }

        public override void Undo()
        {
            mapEditor.Use(block.x, block.y, block.type, block.colorIndex, true);
            if (block.type == 2 || block.type == 3 || block.type == 4 || block.type == 5 || block.type == 7) //interactive
            {
                block.ToSetting(mapEditor.infos[^1]);
            }
        }
    }

    public class AddColor : Change
    {
        private Color32 color;

        public AddColor(byte r, byte g, byte b)
        {
            color.r = r;
            color.g = g;
            color.b = b;
        }
        public AddColor(Color32 color)
        {
            this.color = color;
        }

        public override void Redo()
        {
            colorPalette.CreateColor(color, -1, true);
        }

        public override void Undo()
        {
            colorPalette.selectedButton = colorPalette.FindButton(color);
            colorPalette.DeleteSelectedColor(true); //null and 0 are handled in there
        }
    }

    //TODO removing a color removes the tilemap too!
    public class RemoveColor : Change
    {
        private Color32 color;
        private Tilemap tilemap;

        public RemoveColor(byte r, byte g, byte b, Tilemap tilemap)
        {
            color.r = r;
            color.g = g;
            color.b = b;
            this.tilemap = tilemap;
        }
        public RemoveColor(Color32 color, Tilemap tilemap)
        {
            this.color = color;
            this.tilemap = tilemap;
        }

        public override void Redo()//TODO
        {
            colorPalette.FindButton(color);
            colorPalette.DeleteSelectedColor(true); //null and 0 are handled in there
        }

        public override void Undo()
        {
            colorPalette.CreateColor(color, -1, true);
        }
    }

    public class ModColor : Change
    {
        Color32 before;
        Color32 after;
        public ModColor(byte rbefore, byte gbefore, byte bbefore, byte rafter, byte gafter, byte bafter)
        {   
            before.r = rbefore;
            before.g = gbefore;
            before.b = bbefore;
            after.r = rafter;
            after.g = gafter;
            after.b = bafter;
        }
        public ModColor(Color32 before, Color32 after)
        {
            this.before = before;
            this.after = after;
        }

        public override void Redo()
        {
            colorPalette.selectedButton = colorPalette.FindButton(before);
            colorPalette.ModifySelectedColor();
            colorTweaker.color = after; //TODO tidy these up
            colorPalette.OverwriteSelectedColor(true);
        }

        public override void Undo()
        {
            colorPalette.selectedButton = colorPalette.FindButton(after);
            colorPalette.ModifySelectedColor();
            colorTweaker.color = before;
            colorPalette.OverwriteSelectedColor(true);
        }
    }

    public class AddInversePair : Change
    {
        GameObject inversePair;

        public AddInversePair(GameObject IP)
        {
            inversePair = IP;
        }

        public override void Redo()
        {
            mapEditor.CreateInversePair(true);
            inversePair = mapEditor.inversePairs[^1]; //TODO nem vagyok ebben biztos
        }

        public override void Undo()
        {
            inversePair.GetComponent<InversePair>().CommitSucide(true);
        }
    }

    public class RemoveInversePair : Change
    {
        GameObject inversePair;
        Color32 left, right;

        public RemoveInversePair(byte rleft, byte gleft, byte bleft, byte rright, byte gright, byte bright)
        {
            left.r = rleft;
            left.g = gleft;
            left.b = bleft;
            right.r = rright;
            right.g = gright;
            right.b = bright;
        }
        public RemoveInversePair(Color32 left, Color32 right)
        {
            this.left = left;
            this.right = right;
        }

        public override void Redo()
        {
            inversePair.GetComponent<InversePair>().CommitSucide(true);
        }

        public override void Undo()
        {
            mapEditor.CreateInversePair(true);
            inversePair = mapEditor.inversePairs[^1]; //nem vagyok ebben biztos

            inversePair.GetComponent<InversePair>().b1.ChangeColor(colorPalette.FindButton(left), true);

            inversePair.GetComponent<InversePair>().b2.ChangeColor(colorPalette.FindButton(right), true);
        }
    }

    public class ModInversePair : Change
    {
        bool leftBeforeColor, rightBeforeColor;
        Color32 leftBefore, rightBefore;
        Color32 leftAfter, rightAfter;
        public ModInversePair(bool leftBeforeColor, bool rightBeforeColor, Color32 leftbefore, Color32 leftafter, Color32 rightbefore, Color32 rightafter)
        {
            this.leftBeforeColor = leftBeforeColor;
            this.rightBeforeColor = rightBeforeColor;
            this.leftBefore = leftbefore;
            this.rightBefore = rightbefore;
            this.leftAfter = leftafter;
            this.rightAfter = rightafter;
        }
        public ModInversePair(bool leftBeforeColor, bool rightBeforeColor,
            byte rleftbefore, byte gleftbefore, byte bleftbefore, byte rleftafter, byte gleftafter, byte bleftafter,
            byte rrightbefore, byte grightbefore, byte brightbefore, byte rrightafter, byte grightafter, byte brightafter)
        {
            this.leftBeforeColor = leftBeforeColor;
            this.rightBeforeColor = rightBeforeColor;
            leftBefore.r = rleftbefore;
            leftBefore.g = gleftbefore;
            leftBefore.b = bleftbefore;
            rightBefore.r = rrightbefore;
            rightBefore.g = grightbefore;
            rightBefore.b = brightbefore;
            leftAfter.r = rleftafter;
            leftAfter.g = gleftafter;
            leftAfter.b = bleftafter;
            rightAfter.r = rrightafter;
            rightAfter.g = grightafter;
            rightAfter.b = brightafter;
        }

        public override void Redo()
        {
            foreach (GameObject IP in mapEditor.inversePairs)
            {
                if (IP.GetComponent<InversePair>().b1.GetComponent<Image>().color == leftBefore &&
                    IP.GetComponent<InversePair>().b2.GetComponent<Image>().color == rightBefore)
                {
                    IP.GetComponent<InversePair>().b1.ChangeColor(colorPalette.FindButton(leftAfter), true);

                    IP.GetComponent<InversePair>().b2.ChangeColor(colorPalette.FindButton(rightAfter), true);

                    return;
                }
            }

            Debug.Log("Couldn't find the inversePair");
        }

        public override void Undo()
        {
            foreach (GameObject IP in mapEditor.inversePairs)
            {
                if (IP.GetComponent<InversePair>().b1.GetComponent<Image>().color == leftAfter)
                {
                    if (leftBeforeColor)
                    {
                        IP.GetComponent<InversePair>().b1.ChangeColor(colorPalette.FindButton(leftBefore), true);
                    }
                    else
                    {
                        InverseButton b = IP.GetComponent<InversePair>().b1;
                        b.GetComponent<Image>().sprite = b.warning;
                        b.GetComponent<Image>().color = Color.white;
                    }

                    if (rightBeforeColor)
                    {
                        IP.GetComponent<InversePair>().b2.ChangeColor(colorPalette.FindButton(rightBefore), true);
                    }
                    else
                    {
                        InverseButton b = IP.GetComponent<InversePair>().b2;
                        b.GetComponent<Image>().sprite = b.warning;
                        b.GetComponent<Image>().color = Color.white;
                    }

                    return;
                }
            }

            Debug.Log("Couldn't find the inversePair");
        }

    }

    public class ModDefaultState : Change
    {
        Color32 color;
        bool turnedOn;

        public ModDefaultState(byte r, byte g, byte b, bool turnedOn)
        {
            color.r = r;
            color.g = g;
            color.b = b;
            this.turnedOn = turnedOn;
        }
        public ModDefaultState(Color32 color, bool turnedOn)
        {
            this.color = color;
            this.turnedOn = turnedOn;
        }

        public override void Redo()
        {
            colorPalette.FindButton(color).toggle.GetComponentInChildren<Toggle>().SetIsOnWithoutNotify(turnedOn);
        }

        public override void Undo()
        {
            colorPalette.FindButton(color).toggle.GetComponentInChildren<Toggle>().SetIsOnWithoutNotify(!turnedOn);
        }
    }

    public class ModSetting : Change
    {
        SettingForInteract setting;
        BlockData before, after;

        public ModSetting(SettingForInteract setting, BlockData before, BlockData after)
        {
            this.setting = setting;
            this.before = before;
            this.after = after;
        }

        public override void Redo()
        {
            if (setting == null) //interactive placement undone and redone
            {
                //search for the interactive
                int i;
                for (i = 0; i < mapEditor.infos.Count; i++)
                {
                    if (mapEditor.infos[i].x == before.x && mapEditor.infos[i].y == before.y)
                    {
                        setting = mapEditor.infos[i];
                        break;
                    }
                }
                if (i == mapEditor.infos.Count) Debug.Log("search failed");
            }

            after.ToSetting(setting);
            if (setting.isButtonTimerCube) setting.inputField.SetTextWithoutNotify(setting.timer.ToString());
            if (setting.isPortal) setting.inputField.SetTextWithoutNotify(setting.portalIndex.ToString());
        }

        public override void Undo()
        {
            if (setting == null) //bc interactive placement undone and redone
            {
                //search for the interactive
                int i;
                for (i = 0; i < mapEditor.infos.Count; i++)
                {
                    if (mapEditor.infos[i].x == before.x && mapEditor.infos[i].y == before.y)
                    {
                        setting = mapEditor.infos[i];
                        break;
                    }
                }
                if (i == mapEditor.infos.Count) Debug.Log("search failed");
            }

            before.ToSetting(setting);
            if (setting.isButtonTimerCube) setting.inputField.SetTextWithoutNotify(setting.timer.ToString());
            if (setting.isPortal) setting.inputField.SetTextWithoutNotify(setting.portalIndex.ToString());
        }
    }
}
