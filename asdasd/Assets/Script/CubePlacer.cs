using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    public static bool hasTimerCube;
    public static int timerCubeColor;
    public GameObject timerCubePrefab;
    public TimerCube createdTimerCube;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (hasTimerCube && Input.GetAxisRaw("PlaceTimerCube") > 0)
        {
            CreateTimerCube();
            hasTimerCube = false;
        }
    }
    public void CreateTimerCube()
    {
        GameObject timerCube = Instantiate(timerCubePrefab, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0), default);
        createdTimerCube = timerCube.GetComponent<TimerCube>();
        createdTimerCube.colorIndex = timerCubeColor;
        createdTimerCube.Set();
    }
}
