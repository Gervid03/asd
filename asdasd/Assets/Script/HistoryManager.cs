using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    public Stacks stacks;

    void Start()
    {
        stacks = new Stacks();
    }

    private void Update()
    {
        if (Input.GetKeyDown("z") && Input.GetKeyDown(KeyCode.LeftControl))
        {
            stacks.PopUndo();
        }
        if (Input.GetKeyDown("y") && Input.GetKeyDown(KeyCode.LeftControl))
        {
            stacks.PopRedo();
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
            colorPalette.FindButton(color);
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
            colorPalette.FindButton(color);
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
            inversePair.GetComponent<Suicide>().CommitSucide();
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
            inversePair.GetComponent<Suicide>().CommitSucide();
        }

        public override void Undo()
        {
            mapEditor.CreateInversePair();
            inversePair = mapEditor.inversePairs[^1]; //nem vagyok ebben biztos

            colorPalette.selectedButton = colorPalette.FindButton(left);
            inversePair.GetComponent<Suicide>().b1.Clicked();
            colorPalette.selectedButton = colorPalette.FindButton(right);
            inversePair.GetComponent<Suicide>().b2.Clicked();

            colorPalette.selectedButton = colorPalette.colors[0];
        }
    }

    public class IPMod : Change
    {
        Color32 leftBefore, rightBefore;
        Color32 leftAfter, rightAfter;
        public IPMod(byte rleftbefore, byte gleftbefore, byte bleftbefore, byte rrightbefore, byte grightbefore, byte brightbefore,
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
        public IPMod(Color32 leftbefore, Color32 rightbefore, Color32 leftafter, Color32 rightafter)
        {
            this.leftBefore = leftbefore;
            this.rightBefore = rightbefore;
            this.leftAfter = leftafter;
            this.rightAfter = rightafter;
        }

        public override void Redo()
        {
            foreach (GameObject IP in mapEditor.inversePairs)
            {
                if (IP.GetComponent<Suicide>().b1.GetComponent<Image>().color == leftBefore)
                {
                    colorPalette.selectedButton = colorPalette.FindButton(leftAfter);
                    IP.GetComponent<Suicide>().b1.Clicked();
                    colorPalette.selectedButton = colorPalette.FindButton(rightAfter);
                    IP.GetComponent<Suicide>().b2.Clicked();

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
                if (IP.GetComponent<Suicide>().b1.GetComponent<Image>().color == leftAfter)
                {
                    colorPalette.selectedButton = colorPalette.FindButton(leftBefore);
                    IP.GetComponent<Suicide>().b1.Clicked();
                    colorPalette.selectedButton = colorPalette.FindButton(rightBefore);
                    IP.GetComponent<Suicide>().b2.Clicked();

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
