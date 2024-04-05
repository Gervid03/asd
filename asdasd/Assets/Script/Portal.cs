using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject character;
    public int portalIndex;
    public int colorIndex;
    public GameObject pairPortal;
    public float timeOfTP;
    public float cooldownTP;
    public GameObject portalIndexDisplay;
    // Start is called before the first frame update
    void Start()
    {
        character = FindFirstObjectByType<Player>().gameObject;
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        portalIndexDisplay.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(portalIndex);

        Portal[] portals = FindObjectsOfType<Portal>();
        foreach (Portal p in portals)
        {
            if (p.portalIndex == portalIndex)
            {
                pairPortal = p.gameObject;
            }
        }
    }

    public void BeActive()
    {
        this.GetComponent<Collider2D>().enabled = true;
        Color color = this.GetComponent<SpriteRenderer>().color;
        color.a = 1;
        this.GetComponent<SpriteRenderer>().color = color;
    }

    public void DontBeActive()
    {
        this.GetComponent<Collider2D>().enabled = false;
        Color color = this.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        this.GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeOfTP + cooldownTP < Time.time)
        {
            Physics2D.IgnoreCollision(character.GetComponent<Collider2D>(), pairPortal.GetComponent<Collider2D>(), false);
        }
        if (this.gameObject.GetComponent<Collider2D>().IsTouching(character.GetComponent<Collider2D>()) && Input.GetAxisRaw("Interact") > 0)
        {
            character.GetComponent<Rigidbody2D>().position = pairPortal.transform.position;
            character.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Physics2D.IgnoreCollision(character.GetComponent<Collider2D>(), pairPortal.GetComponent<Collider2D>(), true);
            timeOfTP = Time.time;
        }
    }
}
