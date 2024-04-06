using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButtonTimerCube : MonoBehaviour
{
    public int colorIndex;
    public int state; //how many collisions are there currently
    public Sprite stateActivated;
    public Sprite stateDeactivated;
    public int cubeColor;
    public SpriteRenderer spriteRenderer;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;
    public GameObject character;
    public float timer;

    private void Awake()
    {
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
    }

    public void SetColor()
    {
        spriteRenderer.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    public void SubscribeToBeButtonForTimerCube()
    {
        //informs the manager of the existence
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
        CubePlacer.hasTimerCube = true;
        CubePlacer.timerCubeColor = cubeColor;
        CubePlacer.timer = timer;
    }

    public void EndOfUse()
    {
        state--;
        if (state == 0)
        {
            spriteRenderer.sprite = stateDeactivated;
        }
    }

    public void Create(int color, int cubec, float x, float y, int t)
    {
        colorIndex = color;
        timer = t;
        cubeColor = cubec;
        SubscribeToBeButtonForTimerCube();
        SetColor();
        SetPosition(x, y);
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }
}
