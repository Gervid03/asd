using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    public Stacks stacks;

    void Start()
    {
        stacks = new Stacks();
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

        public void PushUndo(Change change, bool keepRedo = false)
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
            PushUndo(redo[^1], true);
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

    public abstract class Change
    {
        protected MapEditor mapEditor;
        protected ColorPalette colorPalette;
        protected Map map;
        protected Stacks stacks;
        protected ColorTweaker colorTweaker;

        protected Change()
        {
            mapEditor = FindFirstObjectByType<MapEditor>();
            colorPalette = FindFirstObjectByType<ColorPalette>();
            map = FindFirstObjectByType<Map>();
            stacks = FindFirstObjectByType<HistoryManager>().stacks;
            colorTweaker = FindFirstObjectByType<ColorTweaker>();

            if (mapEditor == null || colorPalette == null || map == null || stacks == null || colorTweaker == null)
            {
                Debug.Log("smh is null!");
            }
        }

        public abstract void Undo();

        public abstract void Redo();
    }


    public class ColorAdd : Change
    {
        private Color32 color;

        public ColorAdd(byte r, byte g, byte b)
        {
            color.r = r;
            color.g = g;
            color.b = b;
        }
        public ColorAdd(Color32 color)
        {
            this.color = color;
        }

        public override void Redo()
        {
            colorPalette.CreateColor(color);
        }

        public override void Undo()
        {
            foreach (Transform child in colorPalette.colorPaletteParent)
            {
                if (colorPalette.SameColor(child.GetComponent<ColorDisplayButton>().color, color))
                {
                    colorPalette.selectedButton = child.GetComponent<ColorDisplayButton>();
                    break;
                }
            }
            colorPalette.DeleteSelectedColor(); //null and 0 are handled in there
        }
    }
    //TODO removing a color removes the tilemap too!
    public class ColorRemove : Change
    {
        private Color32 color;

        public ColorRemove(byte r, byte g, byte b)
        {
            color.r = r;
            color.g = g;
            color.b = b;
        }
        public ColorRemove(Color32 color)
        {
            this.color = color;
        }

        public override void Redo()
        {
            foreach (Transform child in colorPalette.colorPaletteParent)
            {
                if (colorPalette.SameColor(child.GetComponent<ColorDisplayButton>().color, color))
                {
                    colorPalette.selectedButton = child.GetComponent<ColorDisplayButton>();
                    break;
                }
            }
            colorPalette.DeleteSelectedColor(); //null and 0 are handled in there
        }

        public override void Undo()
        {
            colorPalette.CreateColor(color);
        }
    }

    public class ColorMod : Change
    {
        Color32 before;
        Color32 after;
        public ColorMod(byte rbefore, byte gbefore, byte bbefore, byte rafter, byte gafter, byte bafter)
        {   
            before.r = rbefore;
            before.g = gbefore;
            before.b = bbefore;
            after.r = rafter;
            after.g = gafter;
            after.b = bafter;
        }
        public ColorMod(Color32 before, Color32 after)
        {
            this.before = before;
            this.after = after;
        }

        public override void Redo()
        {
            foreach (Transform child in colorPalette.colorPaletteParent)
            {
                if (colorPalette.SameColor(child.GetComponent<ColorDisplayButton>().color, before))
                {
                    colorPalette.selectedButton = child.GetComponent<ColorDisplayButton>();
                    break;
                }
            }
            colorPalette.ModifySelectedColor();
            colorTweaker.color = after; //TODO tidy these up
            colorPalette.OverwriteSelectedColor();
        }

        public override void Undo()
        {
            foreach (Transform child in colorPalette.colorPaletteParent)
            {
                if (colorPalette.SameColor(child.GetComponent<ColorDisplayButton>().color, after))
                {
                    colorPalette.selectedButton = child.GetComponent<ColorDisplayButton>();
                    break;
                }
            }
            colorPalette.ModifySelectedColor();
            colorTweaker.color = before;
            colorPalette.OverwriteSelectedColor();
        }
    }

    public class AddInversePair : Change
    {
        Color32 left;
        Color32 right;
        public AddInversePair(byte rleft, byte gleft, byte bleft, byte rright, byte gright, byte bright)
        {
            left.r = rleft;
            left.g = gleft;
            left.b = bleft;
            right.r = rright;
            right.g = gright;
            right.b = bright;
        }
        public AddInversePair(Color32 left, Color32 right)
        {
            this.left = left;
            this.right = right;
        }

        public override void Redo()
        {
            
        }

        public override void Undo()
        {
            
        }
    }

    public class IPRemove(int r, int g, int b, int r2, int g2, int b2)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.r2 = r2;
        this.g2 = g2;
        this.b2 = b2;
        type = 8;
    }

    public class IPMod(int r, int g, int b, int r2, int g2, int b2, int index)
    {
        this.r = r; //from color
        this.g = g;
        this.b = b;
        this.r2 = r2; //to color
        this.g2 = g2;
        this.b2 = b2;
        this.index = index; //first or second in the pair
        type = 9;
    }

    public class DefaultStateMod(int r, int g, int b, bool on)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.on = on;
        type = 10;
    }
}
