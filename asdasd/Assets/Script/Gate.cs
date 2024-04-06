using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int colorIndex;
    public int deleteColor;
    public GameObject character;
    public WallManager wallManager;

    private void Start()
    {
        character = FindAnyObjectByType<Player>().gameObject;
    }

    void Update()
    {
        if (this.GetComponent<Collider2D>().IsTouching(character.GetComponent<Collider2D>()))
        {
            if (deleteColor == -1)
            {
                //Deactivate every color;
            }
            else
            {
                wallManager.SetColorDeactive(deleteColor);
            }
        }
    }
}
