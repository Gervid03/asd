using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Buttons : MonoBehaviour
{
    public int colorIndex;
    public bool state; //true it's activated, else it's not
    public int interactWithColor; //which color is activated or deactivated by this button
    public bool activateTheColor; //if false then deactivates that color, else activates that color
    public SpriteRenderer spriteRenderer;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SubscribeToBeButton();
    }

    public void SetColor()
    {
        spriteRenderer.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    public void SubscribeToBeButton()
    {
        //informs the manager of the existence
        FindFirstObjectByType<WallManager>().SubscribeToBeAButton(this);
    }

    public void BeActive()
    {
        //becomes active and visible
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 255);
        buttonCollider.enabled = true;
        buttonTriggerCollider.enabled = true;
    }

    public void DontBeActive()
    {
        //becomes invisible
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        buttonCollider.enabled = false;
        buttonTriggerCollider.enabled = false;
    }
}
