using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindFirstObjectByType<Map>().LoadMap("!temp");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r")) SceneLoader.LoadMapEditor();
    }
}
