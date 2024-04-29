using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleLevelBarrier : MonoBehaviour
{

    public type t;

    public enum type
    {
        none = 0,
        left = 1,
        right = 2,
        up = 3,
        down = 4
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Player>() != null)
        {
            MultipleLevel ml = FindFirstObjectByType<MultipleLevel>();
            if (t == type.up) ml.SwitchUp();
            if (t == type.down) ml.SwitchDown();
            if (t == type.left) ml.SwitchLeft();
            if (t == type.right) ml.SwitchRight();
        }
    }
}
