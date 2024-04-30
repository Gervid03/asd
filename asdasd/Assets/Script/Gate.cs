using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using System.Drawing;

public class Gate : MonoBehaviour
{
    public int colorIndex;
    public int interactWithColor;
    public GameObject character;
    public WallManager wallManager;
    public Tilemap image;
    public bool active;

    private void Start()
    {
        WallManager.disableColor += DontBeActive;
        character = FindAnyObjectByType<Player>().gameObject;
        //Physics2D.IgnoreCollision(character.GetComponent<BoxCollider2D>(), gameObject.GetComponent<Collider2D>(), true);
    }

    public void CreateNew(int color)
    {
        colorIndex = color;
        //interactWithColor = interactColor;
        FindFirstObjectByType<WallManager>().SubscribeToBeGate(this);
        SetColor();
    }

    public void BeActive(int color)
    {
        if (color != colorIndex) return;
        Color a = image.color;
        a.a = 1;
        image.color = a;
        active = true;
    }

    public void DontBeActive(int color)
    {
        if (color != colorIndex) return;
        Color a = image.color;
        a.a = 0;
        image.color = a;
        active = false;
    }

    public void SetColor()
    {
        this.GetComponent<Tilemap>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<GateChecker>() != null)
        {
            FindFirstObjectByType<movement>().gatesTouch++;
            if (FindFirstObjectByType<movement>().gatesTouch > 1 && active)
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

    private void OnDestroy()
    {
        WallManager.disableColor -= DontBeActive;
    }
}
