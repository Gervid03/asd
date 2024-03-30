using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int colorIndex;
    public Rigidbody2D characterRB;
    public Collider2D characterC;
    
    // Start is called before the first frame update
    void Start()
    {
        characterC = characterRB.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
    }

    public void Set()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeACube(this);
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
    }

    private void Update()
    {
        Teleport();
    }

    public void BeActive()
    {

    }

    public void DontBeActive()
    {
        Destroy(this.gameObject);
    }

    public void Teleport()
    {
        if (Input.GetAxisRaw("CubeTP") > 0)
        {
            gameObject.transform.position = characterRB.position;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
