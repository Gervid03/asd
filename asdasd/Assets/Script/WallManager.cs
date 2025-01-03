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
    public ActiveAtBeginning activeAtStart;
    public EndThing endThing;
    public Gradient portalColors;
    public Transform decoParent;
    public List<DecoDemon> decoDemons;
    public Tilemap decoDemonTilemap;
    public RuleTile decoDemonBase;
    public int column, row;
    public Tilemap outsideWallTilemap;
    public int[][] wallPositions;
    public static event Action<int> disableColor;
    public static event Action<int> activateColor;
    public NPC_data.Language language;

    [System.Serializable]
    public struct ColorList
    {
        public List<int> indexes;
        public List<Color32> colors;
        public List<bool> visible;

        public Color32 at(int index)
        {
            for(int i = 0; i < indexes.Count; i++)
            {
                if(index == indexes[i])
                {
                    return colors[i];
                }
            }
            return new Color32(255, 255, 255, 255);
        }

        public int searchColor(Color32 color)
        {
            WallManager wallManager = FindFirstObjectByType<WallManager>();
            for (int i = 0; i < colors.Count; i++)
            {
                if (wallManager.SameColor(color, colors[i]))
                {
                    return indexes[i];
                }
            }
            return -1;
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
            makeItNotNull(new List<int>(), new List<Color32>());
        }

        public void add(Color32 t, int index, bool vis = true)
        {
            if (indexes.Contains(index)) return;
            colors.Add(t);
            indexes.Add(index);
            visible.Add(vis);
        }

        public void makeItNotNull(List<int> a, List<Color32> b)
        {
            indexes = a;
            colors = b;
            visible = new List<bool>();
        }

        public List<int> getIndexes()
        {
            return indexes;
        }

        public bool exist(int index)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (index == indexes[i])
                {
                    return true;
                }
            }
            return false;
        }

        public int maxIndex()
        {
            int maxi = -1;
            for (int i = 0; i < indexes.Count; i++)
            {
                maxi = Mathf.Max(maxi, indexes[i]);
            }
            return maxi;
        }
    }

    
    [System.Serializable]
    public struct ActiveAtBeginning
    {
        public List<int> index;
        public List<bool> active;

        public void add(int ind, bool act)
        {
            if(index == null)
            {
                index = new List<int>();
                active = new List<bool>();
            }
            if (index.Contains(ind)) return;
            index.Add(ind);
            active.Add(act);
        }

        public List<int> getIndexes()
        {
            return index;
        }

        public bool at(int ind)
        {
            for(int i = 0; i < index.Count; i++)
            {
                if(ind == index[i]) return active[i];
            }
            return false;
        }

        public void remove(int ind)
        {
            if (index == null) return;
            for(int i = 0; i < index.Count; i++)
            {
                if (index[i] == ind)
                {
                    index.RemoveAt(i);
                    active.RemoveAt(i);
                }
            }
        }

        public void clear()
        {
            index.Clear();
            active.Clear();
        }
    }

    [System.Serializable]
    public struct InversePair
    {
        public List<pair> pairs;

        public void clear()
        {
            pairs = new List<pair>();
        }

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

    [System.Serializable]
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
        colors.makeItNotNull(new List<int>(), new List<Color32>());
        inversColor.makeItNotNull(new List<pair>());
        SetDecoDemons();
    }

    private void Update()
    {
        //if (Input.GetKeyDown("o")){
        //    Debug.Log("o");
        //    FindFirstObjectByType<PopUpHandler>().ResetProgress();
        //    for (int i = 0; i < wallPositions.Length; i++)
        //    {
        //        string line = "";
        //        for (int j = 0; j < wallPositions[i].Length; j++)
        //        {
        //            line += (char)(wallPositions[i][j] + '0')  + " ";
        //        }
        //        Debug.LogWarning(line);
        //    }
        //    //FindFirstObjectByType<PopUpHandler>().ResetProgress();
        //}
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
        if(FindFirstObjectByType<Cube>() != null) FindFirstObjectByType<Cube>().transform.position = new Vector3(-100, -100, 0);
        for(int i = 0; i < colors.getIndexes().Count; i++)
        {
            DestroyTimerCube(colors.getIndexes()[i]);
        }

        NPC[] t = FindObjectsByType<NPC>(default);
        foreach (NPC k in t)
        {
            Destroy(k.gameObject);
        }
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

    public Color32 GetColor(int index)
    {
        return colors.at(index);
    }

    public Color32 GetPortalColor(float index)
    {
        return portalColors.Evaluate((float)index / 100);
    }

    /*
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
    */

    #region setColor(De)Active

    public void SetColorActive(int index, bool inverzed = false)
    {
        if (SameColor(colors.at(index), new Color32(255, 255, 255, 255))) return;
        activateColor?.Invoke(index);
        if (inversColor.at(index) != -1 && !inverzed) SetColorDeactive(inversColor.at(index), true);
        colors.setVisible(index, true);
        /*//makes the color with the index visible
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
                //gates[i].BeActive();
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
        */
    }


    public void SetColorDeactive(int index, bool inverzed = false)
    {
        if (SameColor(colors.at(index), new Color32(255, 255, 255, 255))) return;
        disableColor?.Invoke(index);
        if (inversColor.at(index) != -1 && !inverzed) SetColorActive(inversColor.at(index), true);
        colors.setVisible(index, false);
        /*//makes the color with the index invisible
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
                //gates[i].DontBeActive(0);
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
        */
    }
    #endregion


    public void DestroyAllCube()
    {
        Cube[] c = FindObjectsByType<Cube>(default);
        for(int i = 0; i < c.Length; i++)
        {
            c[i].ForceDestroy(c[i].colorIndex);
        }
        cubes.Clear();
    }

    public void DestroyTimerCube(int color)
    {
        TimerCube[] tc = FindObjectsByType<TimerCube>(default); 
        for (int i = 0; i < tc.Length; i++)
        {
            if (tc[i].colorIndex == color)
            {
                tc[i].ForceDestroy(tc[i].colorIndex);
            }
        }
    }

    public void SetDefaultState()
    {
        for (int i = 0; i < activeAtStart.getIndexes().Count; i++)
        {
            if (activeAtStart.at(activeAtStart.getIndexes()[i])) SetColorActive(activeAtStart.getIndexes()[i]);
            else SetColorDeactive(activeAtStart.getIndexes()[i]);

            if (!activeAtStart.at(activeAtStart.getIndexes()[i]))
            {
                Cube[] c = FindObjectsByType<Cube>(default);
                for (int j = 0; j < c.Length; j++)
                {
                    if (c[j].colorIndex == activeAtStart.getIndexes()[i])
                    {
                        c[j].ForceDestroy(c[j].colorIndex);
                    }
                }
                DestroyTimerCube(activeAtStart.getIndexes()[i]);
                FindFirstObjectByType<CubePlacer>().RemoveTimerCube(activeAtStart.getIndexes()[i]);
            }
            colors.setVisible(activeAtStart.getIndexes()[i], activeAtStart.at(activeAtStart.getIndexes()[i]));
        }
    }

    public void SetCurrentState()
    {
        List<int> ind = colors.getIndexes();
        for(int i = 0; i < ind.Count; i++)
        {
            if (colors.atVisible(ind[i])) SetColorActive(ind[i]);
            else SetColorDeactive(ind[i]);
        }
    }

    public bool SameColor(Color32 a, Color32 b)
    {
        return (a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a);
    }

    public void SaveCurrentProgress()
    {
        FindFirstObjectByType<ProgressGatherer>().GetInfos();
        SaveLoadMaps.SaveProgress(FindFirstObjectByType<ProgressGatherer>());
        Debug.Log("saved");
    }

    public void CheckIfThePositionIsInTheWall(int x, int y)
    {
        if (x < 0 || y < 0 || x >= wallPositions.Length || y >= wallPositions[x].Length) return;
        if (colors.atVisible(wallPositions[x][y]) && wallPositions[x][y] != -1)
        {
            //TODO later make it not a comment
            FindFirstObjectByType<Map>().LoadFromProgress();
            Debug.Log("Stuck in the wall");
        }
    }
}
