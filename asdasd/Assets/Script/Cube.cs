using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Cube : MonoBehaviour
{
    public int colorIndex;
    public Rigidbody2D characterRB;
    public Collider2D characterC;
    public float fallSpeedLimit;
    public Light2D light2D;
    public Light2D light2Dinside;

    public void Set()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeACube(this);
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 255);
        characterC = FindFirstObjectByType<Player>().gameObject.GetComponent<BoxCollider2D>();
        characterRB = FindFirstObjectByType<Player>().gameObject.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
        light2D.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        light2Dinside.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        FindFirstObjectByType<movement>().GotNewCube(colorIndex);
    }

    private void Update()
    {
        Teleport();
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, Mathf.Max(-fallSpeedLimit, this.GetComponent<Rigidbody2D>().velocity.y));
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
