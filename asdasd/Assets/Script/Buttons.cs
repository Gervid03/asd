using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Buttons : MonoBehaviour
{
    public int colorIndex;
    public int state; //how many collisions are there currently
    public Sprite stateActivated;
    public Sprite stateDeactivated;
    public int interactWithColor; //which color is activated or deactivated by this button
    public bool activateTheColor; //if false then deactivates that color, else activates that color
    public SpriteRenderer spriteRenderer;
    public Light2D displayColor;
    public Light2D displayInteractiveColor;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;

    public void CreateNew(int color, int interactColor, float x, float y, bool activate)
    {
        colorIndex = color;
        interactWithColor = interactColor;
        activateTheColor = activate;
        SetPosition(x, y);
        SetColor();
        SubscribeToBeButton();
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetColor()
    {
        displayColor.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        displayInteractiveColor.color = FindFirstObjectByType<WallManager>().GetColor(interactWithColor);
    }

    public void SubscribeToBeButton()
    {
        //informs the manager of the existence
        FindFirstObjectByType<WallManager>().SubscribeToBeAButton(this);
    }

    public void BeActive()
    {
        //becomes active and visible
        displayColor.color = new Color(displayColor.color.r, displayColor.color.g, displayColor.color.b, 255);
        displayInteractiveColor.color = new Color(displayInteractiveColor.color.r, displayInteractiveColor.color.g, displayInteractiveColor.color.b, 255);
        buttonCollider.enabled = true;
        buttonTriggerCollider.enabled = true;
    }

    public void DontBeActive()
    {
        //becomes invisible
        displayColor.color = new Color(displayColor.color.r, displayColor.color.g, displayColor.color.b, 0);
        displayInteractiveColor.color = new Color(displayInteractiveColor.color.r, displayInteractiveColor.color.g, displayInteractiveColor.color.b, 0);
        buttonCollider.enabled = false;
        buttonTriggerCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it's the player it gets activated
        if(collision.gameObject.GetComponent<CanInteract>() != null)
        {
            Use();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CanInteract>() != null)
        {
            EndOfUse();
        }
    }

    public void Use()
    {
        state++;
        if(state == 1)
        {
            spriteRenderer.sprite = stateActivated;
            if (activateTheColor)
            {
                FindFirstObjectByType<WallManager>().SetColorActive(interactWithColor);
            }
            else
            {
                FindFirstObjectByType<WallManager>().SetColorDeactive(interactWithColor);
            }
        }
    }

    public void EndOfUse()
    {
        state--;
        if(state == 0)
        {
            spriteRenderer.sprite = stateDeactivated;
            if (!activateTheColor)
            {
                FindFirstObjectByType<WallManager>().SetColorActive(interactWithColor);
            }
            else
            {
                FindFirstObjectByType<WallManager>().SetColorDeactive(interactWithColor);
            }
        }
    }
}
