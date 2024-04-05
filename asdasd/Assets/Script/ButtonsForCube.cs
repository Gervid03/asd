using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButtonsForCube : MonoBehaviour
{
    public int colorIndex;
    public int state; //how many collisions are there currently
    public Sprite stateActivated;
    public Sprite stateDeactivated;
    public int cubeColor; //which color is activated or deactivated by this button
    public SpriteRenderer spriteRenderer;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;
    public GameObject cubePrefab;

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

    public void SubscribeToBeButtonForCube()
    {
        //informs the manager of the existence
        FindFirstObjectByType<WallManager>().SubscribeToBeAButtonForCube(this);
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
            CreateCube();
        }
    }

    public void CreateCube()
    {
        FindFirstObjectByType<WallManager>().DestroyAllCube();
        GameObject cube = Instantiate(cubePrefab, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.5f, 0), default);
        Cube cubeCube = cube.GetComponent<Cube>();
        cubeCube.colorIndex = cubeColor;
        cubeCube.Set();
    }

    public void EndOfUse()
    {
        state--;
        if(state == 0)
        {
            spriteRenderer.sprite = stateDeactivated;
        }
    }

    public void CreateNew(int color, int cubec, float x, float y)
    {
        colorIndex = color;
        cubeColor = cubec;
        SetPosition(x, y);
        SubscribeToBeButtonForCube();
    }

    public void SetPosition(float x, float y)
    {
        Debug.Log(x + " " + y);
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }
}
