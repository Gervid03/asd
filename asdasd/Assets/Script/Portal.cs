using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject character;
    public int portalIndex;
    public int colorIndex;
    public GameObject pairPortal;
    public float timeOfTP;
    public float cooldownTP;
    public Light2D portalIndexDisplay;
    public Light2D colorLight;

    public void BeActive(int c)
    {
        if (c != colorIndex) return;
        portalIndexDisplay.gameObject.SetActive(true);
        colorLight.gameObject.SetActive(true);
        this.GetComponent<Collider2D>().enabled = true;
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex) return;
        portalIndexDisplay.gameObject.SetActive(false);
        colorLight.gameObject.SetActive(false);
        this.GetComponent<Collider2D>().enabled = false;
    }

    private void Awake()
    {
        WallManager.disableColor += DontBeActive;
        WallManager.activateColor += BeActive;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeOfTP + cooldownTP < Time.time)
        {
            if(pairPortal != null) Physics2D.IgnoreCollision(character.GetComponent<Collider2D>(), pairPortal.GetComponent<Collider2D>(), false);
        }
        if (this.gameObject.GetComponent<Collider2D>().IsTouching(character.GetComponent<Collider2D>()) && Input.GetAxisRaw("Interact") > 0)
        {
            character.GetComponent<Rigidbody2D>().position = pairPortal.transform.position;
            character.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Physics2D.IgnoreCollision(character.GetComponent<Collider2D>(), pairPortal.GetComponent<Collider2D>(), true);
            timeOfTP = Time.time;
        }
        if(pairPortal == null)
        {
            Portal[] portals = FindObjectsOfType<Portal>();
            foreach (Portal p in portals)
            {
                if (p.portalIndex == portalIndex && this != p)
                {
                    pairPortal = p.gameObject;
                }
            }
        }
    }

    public void CreateNew(int color, int portalColor, int x, int y)
    {
        colorIndex = color;
        portalIndex = portalColor;
        SetPosition(x, y); 
        character = FindFirstObjectByType<Player>().gameObject;
        colorLight.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        portalIndexDisplay.color = FindFirstObjectByType<WallManager>().GetPortalColor((float)portalIndex);
    }

    public void SetPosition(int x, int y)
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
