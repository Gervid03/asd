using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Lever : MonoBehaviour
{
    public int colorIndex;
    public Sprite stateActivated;
    public Sprite stateDeactivated;
    public int interactWithColor; //which color is activated or deactivated by this button
    public bool activateTheColor; //if false then deactivates that color, else activates that color
    public SpriteRenderer spriteRenderer;
    public Collider2D leverCollider;
    public Collider2D leverTriggerCollider;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        
    }

    public void SetColor()
    {
        spriteRenderer.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    public void SubscribeToBeALever()
    {
        //informs the manager of the existence
        FindFirstObjectByType<WallManager>().SubscribeToBeALever(this);
    }

    public void BeActive()
    {
        //becomes active and visible
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 255);
        leverCollider.enabled = true;
        leverTriggerCollider.enabled = true;
    }

    public void DontBeActive()
    {
        //becomes invisible
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        leverCollider.enabled = false;
        leverTriggerCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it's the player it gets activated
        if (collision.gameObject.GetComponent<movement>() != null)
        {
            Use();
        }
    }

    public void Use()
    {
        activateTheColor = !activateTheColor;
        if (activateTheColor)
        {
            spriteRenderer.sprite = stateActivated;
            FindFirstObjectByType<WallManager>().SetColorActive(interactWithColor);
        }
        else
        {
            spriteRenderer.sprite = stateDeactivated;
            FindFirstObjectByType<WallManager>().SetColorDeactive(interactWithColor);
        }
    }

    public void CreateNew(int color, int interactColor, float x, float y)
    {
        colorIndex = color;
        interactWithColor = interactColor;
        activateTheColor = true;
        SetPosition(x, y);
        SubscribeToBeALever();
        SetColor();
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }
}
