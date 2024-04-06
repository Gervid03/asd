using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerCube : MonoBehaviour
{
    public int colorIndex;
    public Rigidbody2D characterRB;
    public Collider2D characterC;
    public float lifeTime;
    public float fallSpeedLimit;
    public float birthTime;

    private void Start()
    {
        birthTime = Time.time;
    }

    public void Set()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeATimerCube(this);
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 255);
        characterC = FindFirstObjectByType<Player>().gameObject.GetComponent<BoxCollider2D>();
        characterRB = FindFirstObjectByType<Player>().gameObject.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    private void Update()
    {
        if (Time.time - birthTime > lifeTime)
        {
            DontBeActive();
        }
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, Mathf.Max(-fallSpeedLimit, this.GetComponent<Rigidbody2D>().velocity.y));
    }

    public void BeActive()
    {

    }

    public void DontBeActive()
    {
        FindFirstObjectByType<WallManager>().timerCubes.Remove(this);
        Destroy(this.gameObject);
    }
}
