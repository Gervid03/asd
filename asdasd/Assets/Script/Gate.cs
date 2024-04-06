using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int colorIndex;
    public int interactWithColor;
    public GameObject character;
    public WallManager wallManager;

    private void Start()
    {
        character = FindAnyObjectByType<Player>().gameObject;
    }

    public void CreateNew(int color, int interactColor, int x, int y)
    {
        colorIndex = color;
        interactWithColor = interactColor;
        SetPosition(x, y);
        wallMangager.SubscribeToBeGate();
        SetColor();
    }

    public void SetColor()
    {
        this.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    public void SetPosition(float x, float y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    void Update()
    {
        if (this.GetComponent<Collider2D>().IsTouching(character.GetComponent<Collider2D>()))
        {
            if (interactWithColor == -1)
            {
                //Deactivate every color;
            }
            else
            {
                wallManager.SetColorDeactive(interactWithColor);
            }
        }
    }
}
