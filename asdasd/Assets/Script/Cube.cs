using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int colorIndex;
    public Rigidbody2D characterRB;
    public Collider2D characterC;
    public float fallSpeedLimit;

    public void Set()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeACube(this);
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 255);
        characterC = FindFirstObjectByType<Player>().gameObject.GetComponent<BoxCollider2D>();
        characterRB = FindFirstObjectByType<Player>().gameObject.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
    }

    private void Update()
    {
        Teleport();
        characterRB.velocity = new Vector2(characterRB.velocity.x, Mathf.Max(-fallSpeedLimit, characterRB.velocity.y));
    }

    public void BeActive()
    {

    }

    public void DontBeActive()
    {
        FindFirstObjectByType<WallManager>().cubes.Remove(this);
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
