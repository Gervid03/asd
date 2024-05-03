using System;
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
    public SpriteRenderer indicator;
    public Light2D displayColor;
    public Light2D displayInteractiveColor;
    public Collider2D leverCollider;
    public Collider2D leverTriggerCollider;
    public bool interactable;

    void Awake()
    {
        WallManager.disableColor += DontBeActive;
        WallManager.disableColor += DeactivateInteractive;
        WallManager.activateColor += BeActive;
        WallManager.activateColor += ActivateInteractive;
    }

    public void SetColor()
    {
        displayColor.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        indicator.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        displayInteractiveColor.color = FindFirstObjectByType<WallManager>().GetColor(interactWithColor);
    }

    public void BeActive(int c)
    {
        if (c != colorIndex) return;
        //becomes active and visible
        displayColor.gameObject.SetActive(true);
        displayInteractiveColor.gameObject.SetActive(true);
        //leverCollider.enabled = true;
        leverTriggerCollider.enabled = true;
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex) return;
        //becomes invisible
        displayColor.gameObject.SetActive(false);
        displayInteractiveColor.gameObject.SetActive(false);
        //leverCollider.enabled = false;
        leverTriggerCollider.enabled = false;
    }

    private void Update()
    {
        if(interactable && Input.GetAxisRaw("Interact") == 1)
        {
            Use();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it's the player it gets activated
        if (collision.gameObject.GetComponent<movement>() != null)
        {
            interactable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if it's the player it gets activated
        if (collision.gameObject.GetComponent<movement>() != null)
        {
            interactable = false;
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
        SetColor();
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    private void OnDestroy()
    {
        WallManager.disableColor -= DontBeActive;
        WallManager.disableColor -= DeactivateInteractive;
        WallManager.activateColor -= BeActive;
        WallManager.activateColor -= ActivateInteractive;
    }

    public void ActivateInteractive(int c)
    {
        if (c != interactWithColor) return;
        transform.localScale = new Vector2(1, transform.localScale.y);
        activateTheColor = true;
    }

    public void DeactivateInteractive(int c)
    {
        if (c != interactWithColor) return;
        transform.localScale = new Vector2(-1, transform.localScale.y);
        activateTheColor = false;
    }
}
