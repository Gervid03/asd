using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;


public class ButtonTimerCube : MonoBehaviour
{
    public int colorIndex;
    public int state; //how many collisions are there currently
    public Sprite stateActivated;
    public Sprite stateDeactivated;
    public int cubeColor;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer indicator;
    public Light2D displayColor;
    public Light2D displayInteractiveColor;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;
    public GameObject character;
    public float timer;

    private void Awake()
    {
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        WallManager.disableColor += DontBeActive;
        WallManager.activateColor += BeActive;
    }

    public void SetColor()
    {
        displayColor.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        indicator.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        displayInteractiveColor.color = FindFirstObjectByType<WallManager>().GetColor(cubeColor);
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
        if (collision.gameObject.GetComponent<CanInteract>() != null)
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
        if (state == 1)
        {
            spriteRenderer.sprite = stateActivated;
            GiveTimerCube();
        }
    }

    public void GiveTimerCube()
    {
        FindFirstObjectByType<WallManager>().DestroyTimerCube(cubeColor);
        FindFirstObjectByType<CubePlacer>().AddTimerCube(cubeColor, timer);
    }

    public void EndOfUse()
    {
        state--;
        if (state == 0)
        {
            spriteRenderer.sprite = stateDeactivated;
        }
    }

    public void CreateNew(int color, int cubec, float x, float y, int t)
    {
        colorIndex = color;
        timer = t;
        cubeColor = cubec;
        character = FindFirstObjectByType<CubePlacer>().gameObject;
        SetColor();
        SetPosition(x, y);
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    private void OnDestroy()
    {
        WallManager.disableColor -= DontBeActive;
        WallManager.activateColor -= BeActive;
    }
}
