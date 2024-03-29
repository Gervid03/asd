using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{ 
    public List<Color> colors;
    public List<WallObjects> wallObjects;

    public Color GetColor(int index)
    {
        return colors[index];
    }

    public void SubscribeToBeAWallObject(WallObjects wallObject)
    {
        //it gathers all the wallobjects, so they can be later found
        wallObjects.Add(wallObject);
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
    }
}
