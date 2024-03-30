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
        FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    private void Update()
    {
        Teleport();
    }

    public void BeActive()
    {

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
