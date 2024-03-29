using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{ 
    public List<Color> colors;
    public List<WallObjects> wallObjects;
    public List<Buttons> buttons;

    public Color GetColor(int index)
    {
        return colors[index];
    }

    public void SubscribeToBeAWallObject(WallObjects wallObject)
    {
        //it gathers all the wallobjects, so they can be later found
        wallObjects.Add(wallObject);
    }

    public void SubscribeToBeAButton(Buttons button)
    {
        //it gathers all the button, so they can be later found
        buttons.Add(button);
    }

    public void SetColorActive(int index)
    {
        //makes the color with the index visible
        for(int i = 0; i < wallObjects.Count; i++) {
            if (wallObjects[i].colorIndex == index)
            {
                wallObjects[i].BeActive();
            }
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].colorIndex == index)
            {
                buttons[i].BeActive();
            }
        }
    }

    public void SetColorDeactive(int index)
    {
        //makes the color with the index invisible
        for (int i = 0; i < wallObjects.Count; i++)
        {
            if (wallObjects[i].colorIndex == index)
            {
                wallObjects[i].DontBeActive();
            }
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].colorIndex == index)
            {
                buttons[i].DontBeActive();
            }
        }
    }
}
