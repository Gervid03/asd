using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Progress
{
    [System.Serializable]
    public struct CubeStruct
    {
        public int index;
        public float x, y;
    }

    [System.Serializable]
    public struct TimerCubeStruct
    {
        public bool placed;
        public int index;
        public float time, timeLimit, x, y;
    }

    [System.Serializable]
    public struct C
    {
        public int index;
        public bool visible, defaultState;
        public byte r, g, b;

        public Color32 c()
        {
            return new Color32(r, g, b, 255);
        }
    }

    public float x, y;
    public int roomX, roomY;
    public CubeStruct cube;
    public TimerCubeStruct[] timerCubes;
    public C[] colors;

    public Progress(ProgressGatherer pg)
    {
        x = pg.x;
        y = pg.y;
        roomX = pg.roomX;
        roomY = pg.roomY;
        cube = pg.cube;
        timerCubes = pg.timerCubes;
        colors = pg.colors;
    }
}
