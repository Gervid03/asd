using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lever : MonoBehaviour
{
    public int colorIndex;
    public int interactWithColor; //which color is activated or deactivated by this button
    public bool activateTheColor; //if false then deactivates that color, else activates that color
    public SpriteRenderer spriteRenderer;
    public Light2D displayColor;
    public Light2D displayInteractiveColor;
    public Collider2D leverCollider;
    public Collider2D leverTriggerCollider;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetColor()
    {
        displayColor.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        spriteRenderer.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        displayInteractiveColor.color = FindFirstObjectByType<WallManager>().GetColor(interactWithColor);
    }

    public void SubscribeToBeALever()
    {
        //informs the manager of the existence
        FindFirstObjectByType<WallManager>().SubscribeToBeALever(this);
    }

    public void BeActive()
    {
        //becomes active and visible
        displayColor.gameObject.SetActive(true);
        displayInteractiveColor.gameObject.SetActive(true);
        leverCollider.enabled = true;
        leverTriggerCollider.enabled = true;
    }

    public void DontBeActive()
    {
        //becomes invisible
        displayColor.gameObject.SetActive(false);
        displayInteractiveColor.gameObject.SetActive(false);
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
            transform.localScale = new Vector2(1, transform.localScale.y);
            FindFirstObjectByType<WallManager>().SetColorActive(interactWithColor);
        }
        else
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
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
