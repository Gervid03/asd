using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallManager : MonoBehaviour
{ 
    public ColorList colors;
    public List<int> inverzColor; //0 if there is no inverz
    public List<WallObjects> wallObjects;
    public List<Buttons> buttons;
    public List<ButtonsForCube> buttonForCubes;
    public List<Lever> levers;
    public List<Cube> cubes;
    public List<TimerCube> timerCubes;
    public List<Portal> portals;

    public struct ColorList
    {
        List<int> indexes;
        List<Color> colors;
        
        public Color at(int index)
        {
            for(int i = 0; i < indexes.Count; i++)
            {
                if(index == indexes[i])
                {
                    return colors[i];
                }
            }
            return Color.black;
        }

        public void remove(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    colors.RemoveAt(i);
                    indexes.RemoveAt(i);
                    return;
                }
            }
        }

        public void clear()
        {
            indexes.Clear();
            colors.Clear();
        }

        public void add(Color t, int index)
        {
            colors.Add(t);
            indexes.Add(index);
        }

        public void makeItNotNull(List<int> a, List<Color> b)
        {
            indexes = a;
            colors = b;
        }
    }

    private void Start()
    {
        colors.makeItNotNull(new List<int>(), new List<Color>());
    }

    public Color GetColor(int index)
    {
        return colors.at(index);
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

    public void SubscribeToBeATimerCube(TimerCube timerCube)
    {
        //it gathers all the cubes, so they can be later found
        timerCubes.Add(timerCube);
    }

    public void SubscribeToBeAButtonForCube(ButtonsForCube bfc)
    {
        //it gathers all the cubes, so they can be later found
        buttonForCubes.Add(bfc);
    }

    public void SubscribeToBeALever(Lever lever)
    {
        //it gathers all the button, so they can be later found
        levers.Add(lever);
    }

    public void SubscribeToBeAPortal(Portal portal)
    {
        //it gathers all the button, so they can be later found
        portals.Add(portal);
    }

    public void SetColorActive(int index, bool inverzed = false)
    {
        if (inverzColor.Count > index && inverzColor[index] != -1 && !inverzed) SetColorDeactive(inverzColor[index], true);
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
        for (int i = 0; i < buttonForCubes.Count; i++)
        {
            if (buttonForCubes[i].colorIndex == index)
            {
                buttonForCubes[i].BeActive();
            }
        }
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i].colorIndex == index)
            {
                cubes[i].BeActive();
            }
        }
        for (int i = 0; i < portals.Count; i++)
        {
            if (portals[i].colorIndex == index)
            {
                portals[i].BeActive();
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
        if (inverzColor.Count > index && inverzColor[index] != -1 && !inverzed) SetColorActive(inverzColor[index], true); 
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
        for (int i = 0; i < buttonForCubes.Count; i++)
        {
            if (buttonForCubes[i].colorIndex == index)
            {
                buttonForCubes[i].DontBeActive();
            }
        }
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i].colorIndex == index)
            {
                cubes[i].DontBeActive();
            }
        }
        for (int i = 0; i < portals.Count; i++)
        {
            if (portals[i].colorIndex == index)
            {
                portals[i].DontBeActive();
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

    public void DestroyAllCube()
    {
        for(int i = 0;i < cubes.Count; i++)
        {
            cubes[i].DontBeActive();
        }
        cubes.Clear();
    }

    public void DestroyTimerCube(int color)
    {
        for (int i = 0; i < timerCubes.Count; i++)
        {
            if (timerCubes[i].colorIndex == color)
            {
                timerCubes[i].DontBeActive();
                i--;
            }
        }
    }
}
