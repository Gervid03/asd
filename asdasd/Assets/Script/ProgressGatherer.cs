using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressGatherer : MonoBehaviour
{
    public float x, y;
    public int roomX, roomY;
    public Progress.CubeStruct cube;
    public Progress.TimerCubeStruct[] timerCubes;
    public Progress.C[] colors;

    public void GetInfos()
    {
        x = FindFirstObjectByType<Player>().gameObject.transform.position.x;
        y = FindFirstObjectByType<Player>().gameObject.transform.position.y;

        roomX = FindFirstObjectByType<MultipleLevel>().currentX;
        roomY = FindFirstObjectByType<MultipleLevel>().currentY;

        if (FindFirstObjectByType<Cube>() == null)
        {
            cube.index = -1;
        }
        else
        {
            cube.index = FindFirstObjectByType<Cube>().colorIndex;
            cube.x = FindFirstObjectByType<Cube>().transform.position.x;
            cube.y = FindFirstObjectByType<Cube>().transform.position.y;
        }

        WallManager wm = FindFirstObjectByType<WallManager>();
        colors = new Progress.C[wm.colors.indexes.Count];
        for (int i = 0; i < wm.colors.indexes.Count; i++)
        {
            colors[i].index = wm.colors.indexes[i];
            colors[i].r = wm.colors.colors[i].r;
            colors[i].g = wm.colors.colors[i].g;
            colors[i].b = wm.colors.colors[i].b;
            colors[i].visible = wm.colors.visible[i];
            colors[i].defaultState = wm.activeAtStart.at(i);
        }



        List<Progress.TimerCubeStruct> lstTimerCubes = new List<Progress.TimerCubeStruct>();

        foreach (TimerCube tc in FindObjectsByType<TimerCube>(default))
        {
            Progress.TimerCubeStruct temp = new Progress.TimerCubeStruct();

            temp.index = tc.colorIndex;
            temp.placed = true;
            temp.time = Time.time - tc.birthTime;
            temp.timeLimit = tc.lifeTime;
            temp.x = tc.gameObject.transform.position.x;
            temp.y = tc.gameObject.transform.position.y;

            lstTimerCubes.Add(temp);
        }

        foreach (CubePlacer.TimerCubeData tc in FindFirstObjectByType<CubePlacer>().timerCubes)
        {
            Progress.TimerCubeStruct temp = new Progress.TimerCubeStruct();

            temp.placed = false;
            temp.timeLimit = tc.timer;
            temp.index = tc.colorIndex;

            lstTimerCubes.Add(temp);
        }

        timerCubes = new Progress.TimerCubeStruct[lstTimerCubes.Count];

        for (int i = 0; i < timerCubes.Length; i++)
        {
            timerCubes[i] = lstTimerCubes[i];
        }
    }

    public ProgressGatherer()
    {
        x = -100; y = -100;
        roomX = 0; roomY = 0;
        cube = new Progress.CubeStruct();
        timerCubes = new Progress.TimerCubeStruct[0];
        colors = new Progress.C[0];
    }
}
