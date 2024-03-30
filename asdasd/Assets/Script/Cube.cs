using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int colorIndex;
    // Start is called before the first frame update
    void Start()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeACube(this);
    }

    private void Update()
    {
        Teleport();
    }

    public void DontBeActive()
    {
        
    }

    public void Teleport()
    {
        if (Input.GetAxisRaw("CubeTP") > 0)
        {

        }
    }
}
