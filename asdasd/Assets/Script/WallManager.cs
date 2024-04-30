using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class WallManager : MonoBehaviour
{ 
    public ColorList colors;
    public InversePair inversColor; //-1 if there is no inverz
    public List<WallObjects> wallObjects;
    public List<Buttons> buttons;
    public List<ButtonsForCube> buttonForCubes;
    public List<Lever> levers;
    public List<Cube> cubes;
    public List<TimerCube> timerCubes;
    public List<Portal> portals;
    public List<Gate> gates;
    public List<GateLight> gateLights;
    public MapData.ActiveAtStart[] activeAtStart;
    public EndThing endThing;
    public Gradient portalColors;
    public Transform decoParent;
    public List<DecoDemon> decoDemons;
    public Tilemap decoDemonTilemap;
    public RuleTile decoDemonBase;
    public int column, row;
    public Tilemap outsideWallTilemap;
    public static event Action<int> disableColor;

    [System.Serializable]
    public struct ColorList
    {
        List<int> indexes;
        List<Color> colors;
        List<bool> visible;

        public Color at(int index)
        {
            for(int i = 0; i < indexes.Count; i++)
            {
                if(index == indexes[i])
                {
                    return colors[i];
                }
            }
            return Color.white;
        }

        public bool atVisible(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    return visible[i];
                }
            }
            return true;
        }

        public void setVisible(int index, bool to)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    visible[i] = to;
                }
            }
        }

        public void remove(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    colors.RemoveAt(i);
                    indexes.RemoveAt(i);
                    visible.RemoveAt(i);
                    return;
                }
            }
        }

        public void clear()
        {
            indexes.Clear();
            colors.Clear();
            visible.Clear();
        }

        public void add(Color t, int index, bool vis = true)
        {
            colors.Add(t);
            indexes.Add(index);
            visible.Add(vis);
        }

        public void makeItNotNull(List<int> a, List<Color> b)
        {
            indexes = a;
            colors = b;
            visible = new List<bool>();
        }

        public List<int> getIndexes()
        {
            return indexes;
        }
    }

    [System.Serializable]
    public struct InversePair
    {
        List<pair> pairs;

        public int at(int index)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                if (index == pairs[i].a)
                {
                    return pairs[i].b;
                }
                if (index == pairs[i].b)
                {
                    return pairs[i].a;
                }
            }
            return -1;
        }

        public void add(int index1, int index2)
        {
            pair p;
            p.a = index1;
            p.b = index2;
            pairs.Add(p);
        }

        public int count()
        {
            return pairs.Count;
        }

        public void makeItNotNull(List<pair> a)
        {
            pairs = a;
        }
    }

    public struct pair
    {
        public int a, b;
    }


    [System.Serializable]
    public struct DecoDemon
    {
        public Sprite sprite;
        public GameObject prefab;

        public void Create(int x, int y)
        {
            GameObject g = Instantiate(prefab, FindFirstObjectByType<WallManager>().decoParent);
            Map m = FindFirstObjectByType<Map>();
            g.GetComponent<Transform>().position = new Vector3(m.tileX + x, m.tileY + y, 0);
        }
    };

    void Awake()
    {
        colors.makeItNotNull(new List<int>(), new List<Color>());
        inversColor.makeItNotNull(new List<pair>());
        SetDecoDemons();
    }

    public void ResetThings()
    {
        wallObjects.Clear();
        buttons.Clear();
        buttonForCubes.Clear();
        levers.Clear();
        cubes.Clear();
        timerCubes.Clear();
        portals.Clear();
        gates.Clear();
        gateLights.Clear();
    }

    public void SetDecoDemons()
    {
        int xRandom = UnityEngine.Random.Range(-50, 50);
        int yRandom = UnityEngine.Random.Range(-50, 50);
        for (int i = 0; i < column; i++)
        {
            for(int j = 0; j < row; j++)
            {
                decoDemonTilemap.SetTile(new Vector3Int(i + xRandom, j + yRandom, 0), decoDemonBase);
                for(int k = 0; k < decoDemons.Count; k++)
                {
                    if (decoDemons[k].sprite == decoDemonTilemap.GetSprite(new Vector3Int(i + xRandom, j + yRandom, 0))){
                        decoDemons[k].Create(i, j);
                    }
                }
            }
        }
    }

    public Color GetColor(int index)
    {
        return colors.at(index);
    }

    public Color GetPortalColor(float index)
    {
        return portalColors.Evaluate((float)index / 100);
    }

    public void SubscribeToBeGate(Gate gate)
    {
        gates.Add(gate);
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

    public void SubscribeToBeAGateLight(GateLight gateLight)
    {
        gateLights.Add(gateLight);
    }

    public void SetColorActive(int index, bool inverzed = false)
    {
        if (inversColor.at(index) != -1 && !inverzed) SetColorDeactive(inversColor.at(index), true);
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
        for (int i = 0; i < gates.Count; i++)
        {
            if (gates[i].colorIndex == index)
            {
                gates[i].BeActive();
            }
        }
        for (int i = 0; i < gateLights.Count; i++)
        {
            if (gateLights[i].colorIndex == index)
            {
                gateLights[i].light2D.gameObject.SetActive(true);
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
                transform.localScale = new Vector2(1, transform.localScale.y);
                levers[i].activateTheColor = true;
            }
        }
        colors.setVisible(index, true);
    }

    public void SetColorDeactive(int index, bool inverzed = false)
    {
        disableColor?.Invoke(index);
        if (inversColor.at(index) != -1 && !inverzed) SetColorActive(inversColor.at(index), true); 
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
        for (int i = 0; i < gates.Count; i++)
        {
            if (gates[i].colorIndex == index)
            {
                gates[i].DontBeActive(0);
            }
        }
        for (int i = 0; i < gateLights.Count; i++)
        {
            if (gateLights[i].colorIndex == index)
            {
                gateLights[i].light2D.gameObject.SetActive(false);
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
                transform.localScale = new Vector2(-1, transform.localScale.y);
                levers[i].activateTheColor = false;
            }
        }
        colors.setVisible(index, false);
    }

    public void DestroyAllCube()
    {
        Cube[] c = FindObjectsByType<Cube>(default);
        for(int i = 0; i < c.Length; i++)
        {
            c[i].DontBeActive();
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

    public void SetDefaultState()
    {
        for (int i = 0; i < activeAtStart.Length; i++)
        {
            if (activeAtStart[i].isActive) SetColorActive(activeAtStart[i].index);
            else SetColorDeactive(activeAtStart[i].index);
        }
    }
}
