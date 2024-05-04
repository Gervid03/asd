using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Decos : MonoBehaviour
{
    public int x, y;

    private void Awake()
    {
        Map.deactivateDecos += Deactivate;
    }

    public void Deactivate(int x1, int y1){
        if(x == x1 && y == y1)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Map.deactivateDecos -= Deactivate;
    }
}
