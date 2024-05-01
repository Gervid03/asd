using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndThing : MonoBehaviour
{
    public Transform tr;

    public void SetPosition(int x, int y)
    {
        Map m = FindFirstObjectByType<Map>();
        tr.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            Debug.LogError("Level Finished");
        }
    }
}
