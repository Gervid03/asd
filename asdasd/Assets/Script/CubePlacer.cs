using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    public GameObject timerCubePrefab;
    public TimerCube createdTimerCube;
    public List<TimerCubeData> timerCubes;
    public bool canPlaceAnother;

    [System.Serializable]
    public struct TimerCubeData{
        public int colorIndex;
        public float timer;
    }

    private void Awake()
    {
        WallManager.disableColor += RemoveTimerCube;
    }

    public void RemoveTimerCube(int colorIndex)
    {
        for(int i = 0; i < timerCubes.Count; i++)
        {
            if (timerCubes[i].colorIndex == colorIndex)// && FindFirstObjectByType<WallManager>().colors.atVisible(colorIndex))
            {
                timerCubes.RemoveAt(i);
                break;
            }
        }
        UpdateCurrentCube();
    }

    public void AddTimerCube(int color, float timer)
    {
        RemoveTimerCube(color);
        TimerCubeData tcd;
        tcd.colorIndex = color;
        tcd.timer = timer;
        timerCubes.Add(tcd);
        UpdateCurrentCube();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerCubes.Count > 0 && Input.GetAxisRaw("PlaceTimerCube") > 0 && canPlaceAnother)
        {
            CreateTimerCube();
            UpdateCurrentCube();
            canPlaceAnother = false;
        }
        if(!canPlaceAnother && Input.GetAxisRaw("PlaceTimerCube") == 0) canPlaceAnother = true;
    }
    public void CreateTimerCube()
    {
        GameObject timerCube = Instantiate(timerCubePrefab, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0), default);
        createdTimerCube = timerCube.GetComponent<TimerCube>();
        createdTimerCube.colorIndex = timerCubes[0].colorIndex;
        createdTimerCube.lifeTime = timerCubes[0].timer;
        timerCubes.RemoveAt(0);
        createdTimerCube.Set();
        UpdateCurrentCube();
    }

    public void UpdateCurrentCube()
    {
        if (timerCubes.Count > 0) FindFirstObjectByType<movement>().SetNewTimerCubeColor(timerCubes[0].colorIndex);
        else FindFirstObjectByType<movement>().NoMoreTimerCubes();
    }
}
