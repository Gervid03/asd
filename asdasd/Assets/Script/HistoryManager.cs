using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class HistoryManager : MonoBehaviour
{
    public Stacks stacks;
    private bool heldDown;

    void Awake()
    {
        stacks = new Stacks();
    }

    void Update()
    {
        if (Input.GetKeyDown("y") && !heldDown) //switched z and y!
        {
            heldDown = true;
            stacks.PopUndo();
        }
        else if (Input.GetKeyDown("z") && !heldDown)
        {
            heldDown = true;
            stacks.PopRedo();
        }
        else
        {
            heldDown = false;
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

    public BlockData(SettingForInteract setting)
    {
        colorPalette = MonoBehaviour.FindFirstObjectByType<ColorPalette>();

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
        this.x = x;
        this.y = y;
        this.colorIndex = colorIndex;
        this.tile = tile;

        MapEditor me = MonoBehaviour.FindFirstObjectByType<MapEditor>();
        if (tile == me.clear)
        {
            type = -1;
        }
        else if (tile == me.tools[0].tile || tile == me.tools[1].tile)
        {
            type = 1;
        }
        else if (tile == me.tools[6].tile)
        {//gate
            type = 6;
        }
        else if (tile == me.tools[8].tile)
        {
            type = 8;
        }
        else if (tile == me.tools[9].tile)
        {
            type = 9;
        }
        else Debug.Log("Tile problems");
    }

    public SettingForInteract ToSetting()
    {
        SettingForInteract setting = new SettingForInteract();
        setting.activate = activate;
        setting.index = colorIndex;
        setting.indexColorInteract = interactColorIndex;
        setting.color.color = colorPalette.colors[colorIndex].color;
        setting.colorInteract.color = colorPalette.colors[interactColorIndex].color;
        setting.isButton = type == 2;
        setting.isButtonsForCube = type == 3;
        setting.isLever = type == 4;
        setting.isPortal = type == 5;
        setting.isButtonTimerCube = type == 7;
        setting.timer = timer;
        setting.portalIndex = portalIndex;
        setting.x = x;
        setting.y = y;
        setting.coordinates.text = "(" + x + ", " + y + ")";
        setting.Set(x, y, colorIndex, colorPalette.colors[colorIndex].color, interactColorIndex, activate);
        //maybe there's no need for all this, but safety first!
        
        return setting;
    }
}


public abstract class Change
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
            mapEditor.Use(after.x, after.y, after.type);
            if (after.type == 2 || after.type == 3 || after.type == 4 || after.type == 5 || after.type == 7) //interactive
            {
                mapEditor.infos[^1] = after.ToSetting();
            }
        }

        public override void Undo()
        {
            if (before.type == -1)
            {
                mapEditor.RemoveAllTileAtThisPositon(before.x, before.y);
                return;
            }

            mapEditor.Use(before.x, before.y, before.type);
            if (before.type == 2 || before.type == 3 || before.type == 4 || before.type == 5 || before.type == 7) //interactive
            {
                mapEditor.infos[^1] = before.ToSetting();
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
            mapEditor.Use(block.x, block.y, block.type);
            if (block.type == 2 || block.type == 3 || block.type == 4 || block.type == 5 || block.type == 7) //interactive
            {
                mapEditor.infos[^1] = block.ToSetting();
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
            colorPalette.DeleteSelectedColor(); //null and 0 are handled in there
        }
    }

    //TODO removing a color removes the tilemap too!
    public class RemoveColor : Change
    {
        private Color32 color;

        public RemoveColor(byte r, byte g, byte b)
        {
            color.r = r;
            color.g = g;
            color.b = b;
        }
        public RemoveColor(Color32 color)
        {
            this.color = color;
        }

        public override void Redo()
        {
            colorPalette.FindButton(color);
            colorPalette.DeleteSelectedColor(); //null and 0 are handled in there
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
            colorPalette.OverwriteSelectedColor();
        }

        public override void Undo()
        {
            colorPalette.selectedButton = colorPalette.FindButton(after);
            colorPalette.ModifySelectedColor();
            colorTweaker.color = before;
            colorPalette.OverwriteSelectedColor();
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
            mapEditor.CreateInversePair();
            inversePair = mapEditor.inversePairs[^1]; //TODO nem vagyok ebben biztos
        }

        public override void Undo()
        {
            inversePair.GetComponent<InversePair>().CommitSucide();
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
            inversePair.GetComponent<InversePair>().CommitSucide();
        }

        public override void Undo()
        {
            mapEditor.CreateInversePair();
            inversePair = mapEditor.inversePairs[^1]; //nem vagyok ebben biztos

            colorPalette.selectedButton = colorPalette.FindButton(left);
            inversePair.GetComponent<InversePair>().b1.Clicked();
            colorPalette.selectedButton = colorPalette.FindButton(right);
            inversePair.GetComponent<InversePair>().b2.Clicked();

            colorPalette.selectedButton = colorPalette.colors[0];
        }
    }

    public class ModInversePair : Change
    {
        Color32 leftBefore, rightBefore;
        Color32 leftAfter, rightAfter;
        public ModInversePair(Color32 leftbefore, Color32 rightbefore, Color32 leftafter, Color32 rightafter)
        {
            this.leftBefore = leftbefore;
            this.rightBefore = rightbefore;
            this.leftAfter = leftafter;
            this.rightAfter = rightafter;
        }
        public ModInversePair(byte rleftbefore, byte gleftbefore, byte bleftbefore, byte rrightbefore, byte grightbefore, byte brightbefore,
                     byte rleftafter, byte gleftafter, byte bleftafter, byte rrightafter, byte grightafter, byte brightafter)
        {
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
                if (IP.GetComponent<InversePair>().b1.GetComponent<Image>().color == leftBefore)
                {
                    colorPalette.selectedButton = colorPalette.FindButton(leftAfter);
                    IP.GetComponent<InversePair>().b1.Clicked();
                    colorPalette.selectedButton = colorPalette.FindButton(rightAfter);
                    IP.GetComponent<InversePair>().b2.Clicked();

                    colorPalette.selectedButton = colorPalette.colors[0]; //reset to white
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
                    colorPalette.selectedButton = colorPalette.FindButton(leftBefore);
                    IP.GetComponent<InversePair>().b1.Clicked();
                    colorPalette.selectedButton = colorPalette.FindButton(rightBefore);
                    IP.GetComponent<InversePair>().b2.Clicked();

                    colorPalette.selectedButton = colorPalette.colors[0]; //reset to white
                    return;
                }
            }

            Debug.Log("Couldn't find the inversePair");
        }

    }

    public class DefaultStateMod : Change
    {
        Color32 color;
        bool turnedOn;

        public DefaultStateMod(byte r, byte g, byte b, bool turnedOn)
        {
            color.r = r;
            color.g = g;
            color.b = b;
            this.turnedOn = turnedOn;
        }
        public DefaultStateMod(Color32 color, bool turnedOn)
        {
            this.color = color;
            this.turnedOn = turnedOn;
        }

        public override void Redo()
        {
            colorPalette.FindButton(color).toggle.GetComponent<Toggle>().isOn = turnedOn;
        }

        public override void Undo()
        {
            colorPalette.FindButton(color).toggle.GetComponent<Toggle>().isOn = !turnedOn;
        }
    }
}
