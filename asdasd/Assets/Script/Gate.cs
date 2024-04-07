using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class Gate : MonoBehaviour
{
    public int colorIndex;
    public int interactWithColor;
    public GameObject character;
    public WallManager wallManager;

    private void Start()
    {
        character = FindAnyObjectByType<Player>().gameObject;
        //Physics2D.IgnoreCollision(character.GetComponent<BoxCollider2D>(), gameObject.GetComponent<Collider2D>(), true);
    }

    public void CreateNew(int color, int interactColor)
    {
        colorIndex = color;
        interactWithColor = interactColor;
        wallManager.SubscribeToBeGate(this);
        SetColor();
    }

    public void SetColor()
    {
        this.GetComponent<Tilemap>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    void Update()
    {
        /*
        if (this.GetComponent<Collider2D>().IsTouching(character.GetComponent<Collider2D>()))
        {
            if (interactWithColor == -1)
            {
                foreach (int index in wallManager.colors.getIndicies())
                {
                    if (wallManager.inverzColor[index] != -1)
                    {
                        if (true)//get default color
                        {
                            wallManager.SetColorActive(index);
                        }
                        else
                        {
                            wallManager.SetColorDeactive(index);
                        }
                    }
                }
            }
            else
            {
                wallManager.SetColorDeactive(interactWithColor);
            }
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<GateChecker>() != null)
        {
            FindFirstObjectByType<movement>().gatesTouch++;
            if (FindFirstObjectByType<movement>().gatesTouch > 1)
            {
                FindFirstObjectByType<WallManager>().SetDefaultState();
                Debug.Log(":)");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<GateChecker>() != null)
        {
            FindFirstObjectByType<movement>().gatesTouch--;
        }
    }
}
