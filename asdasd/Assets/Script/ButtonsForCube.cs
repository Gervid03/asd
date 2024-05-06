using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;


public class ButtonsForCube : MonoBehaviour
{
    public int colorIndex;
    public int state; //how many collisions are there currently
    public Sprite stateActivated;
    public Sprite stateDeactivated;
    public int cubeColor; //which color is activated or deactivated by this button
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer indicator;
    public Light2D displayColor;
    public Light2D displayInteractiveColor;
    public Collider2D buttonCollider;
    public Collider2D buttonTriggerCollider;
    public GameObject cubePrefab;

    private void Awake()
    { 
        WallManager.disableColor += DontBeActive;
        WallManager.activateColor += BeActive;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
        WallManager.activateColor -= BeActive;
    }
}
