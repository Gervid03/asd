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
    //public bool activateTheColor; //if false then deactivates that color, else activates that color
    public SpriteRenderer spriteRenderer; 
    public SpriteRenderer indicator;
    public Light2D displayColor;
    public Light2D displayInteractiveColor;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;

    private void Awake()
    {
        WallManager.disableColor += DontBeActive;
        WallManager.activateColor += BeActive;
    }

    public void CreateNew(int color, int interactColor, float x, float y, bool activate = false)
    {
        colorIndex = color;
        interactWithColor = interactColor;
        //activateTheColor = activate;
        SetPosition(x, y);
        SetColor();
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
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
        buttonTriggerCollider.enabled = true;
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex) return;
        //becomes invisible
        displayColor.gameObject.SetActive(false);
        displayInteractiveColor.gameObject.SetActive(false);
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
            if (FindFirstObjectByType<WallManager>().colors.atVisible(interactWithColor))
            {
                FindFirstObjectByType<WallManager>().SetColorDeactive(interactWithColor);
            }
            else
            {
                FindFirstObjectByType<WallManager>().SetColorActive(interactWithColor);
            }
        }
    }

    public void EndOfUse()
    {
        state--;
        if(state == 0 && FindFirstObjectByType<WallManager>() != null)
        {
            spriteRenderer.sprite = stateDeactivated;
            if (FindFirstObjectByType<WallManager>().colors.atVisible(interactWithColor))
            {
                FindFirstObjectByType<WallManager>().SetColorDeactive(interactWithColor);
            }
            else
            {
                FindFirstObjectByType<WallManager>().SetColorActive(interactWithColor);
            }
        }
    }

    private void OnDestroy()
    {
        WallManager.disableColor -= DontBeActive;
        WallManager.activateColor -= BeActive;
    }
}
