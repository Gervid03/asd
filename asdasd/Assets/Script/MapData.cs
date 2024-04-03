using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData 
{
    public int index;
    public List<int> colorIndex; //to which color belongs this wall
    public List<Color> colors; //color of the indexes
    //start, end positions needed
    public List<Portal> portals; 
    public List<Lever> lever;
    public List<Button> buttons;
    public List<ButtonForCube> buttonForCubes;
    public List<bool> activeAtStart; //is the index active at the beginning
    public int row;
    public int column;

    public struct Button
    {
        public int color;
        public int x, y;
        public int interactiveColor;
        public bool activateAtBeingActive; //if true, then it activates the color when it is pressed
    }

    public struct ButtonForCube
    {
        public int color;
        public int x, y;
        public int interactiveColor;
        public int timer;
    }

    public struct Portal
    {
        public int color;
        public int x, y;
        public int interactiveColor;
    }

    public struct Lever
    {
        public int color;
        public int x, y;
        public int interactiveColor;
    }

    public MapData(Map map)
    {
        //get all the data from a map
    }
}
