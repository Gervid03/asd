using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{ 
    public List<Color> colors;
    public List<int> inverzColor; //0 if there is no inverz
    public List<WallObjects> wallObjects;
    public List<Buttons> buttons;
    public List<Lever> levers;
    public List<Cube> cubes;

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

    public void SubscribeToBeACube(Cube cube)
    {
        //it gathers all the cubes, so they can be later found
        cubes.Add(cube);
    }

    public void SubscribeToBeALever(Lever lever)
    {
        //it gathers all the button, so they can be later found
        levers.Add(lever);
    }

    public void SetColorActive(int index, bool inverzed = false)
    {
        if (inverzColor[index] != 0 && !inverzed) SetColorDeactive(inverzColor[index], true);
        //makes the color with the index visible
        for (int i = 0; i < wallObjects.Count; i++) {
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
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i].colorIndex == index)
            {
                cubes[i].BeActive();
            }
        }
        for (int i = 0; i < levers.Count; i++)
        {
            if (levers[i].colorIndex == index)
            {
                levers[i].BeActive();
            }
            if (levers[i].interactWithColor == index)
            {
                levers[i].spriteRenderer.sprite = levers[i].stateActivated;
                levers[i].activateTheColor = true;
            }
        }
    }

    public void SetColorDeactive(int index, bool inverzed = false)
    {
        if (inverzColor[index] != 0 && !inverzed) SetColorActive(inverzColor[index], true); 
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
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i].colorIndex == index)
            {
                cubes[i].DontBeActive();
            }
        }
        for (int i = 0; i < levers.Count; i++)
        {
            if (levers[i].colorIndex == index)
            {
                levers[i].DontBeActive();
            }
            if (levers[i].interactWithColor == index)
            {
                levers[i].spriteRenderer.sprite = levers[i].stateDeactivated;
                levers[i].activateTheColor = false;
            }
        }
    }
}
