using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData 
{
    public int index;
    public int[][] colorIndex; //to which color belongs this wall
    public ColorForSave[] colors; //color of the indexes
    //start, end positions needed
    public Portal[] portals; 
    public Lever[] lever;
    public Button[] buttons;
    public ButtonForCube[] buttonForCubes;
    public ButtonTimerCube[] buttonTimerCubes;
    public bool[] activeAtStart; //is the index active at the beginning
    public int row;
    public int column;

    [System.Serializable]
    public struct ColorForSave
    {
        public int index;
        public float r, g, b;
        public void Set(Color c, int i)
        {
            r = c.r;
            g = c.g;
            b = c.b;
            index = i;
        }

        public Color c()
        {
            return new Color(r, g, b);
        }
    }

    [System.Serializable]
    public struct Button
    {
        public int color;
        public int x, y;
        public int interactiveColor;
        public bool activateAtBeingActive; //if true, then it activates the color when it is pressed
    }

    [System.Serializable]
    public struct ButtonForCube
    {
        public int color;
        public int x, y;
        public int interactiveColor;
    }

    [System.Serializable]
    public struct ButtonTimerCube
    {
        public int color;
        public int x, y;
        public int interactiveColor;
        public int timer;
    }

    [System.Serializable]
    public struct Portal
    {
        public int color;
        public int x, y;
        public int interactiveColor;
    }

    [System.Serializable]
    public struct Lever
    {
        public int color;
        public int x, y;
        public int interactiveColor;
    }

    public MapData(Map map)
    {
        //get all the data from a map
        index = map.index;
        row = map.row;
        column = map.column;
        colorIndex = map.colorIndex;
        colors = map.colors;
        portals = map.portals;
        lever = map.lever;
        buttons = map.buttons;
        buttonForCubes = map.buttonForCubes;
        buttonTimerCubes = map.buttonTimerCubes;
        activeAtStart = map.activeAtStart;
    }
}
