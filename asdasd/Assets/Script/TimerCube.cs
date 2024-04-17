using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimerCube : MonoBehaviour
{
    public int colorIndex;
    public Rigidbody2D characterRB;
    public Collider2D characterC;
    public float lifeTime;
    public float fallSpeedLimit;
    public float birthTime;
    public Light2D light2D;
    public Light2D light2Dinside;

    private void Start()
    {
        birthTime = Time.time;
    }

    public void Set()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeATimerCube(this);
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        characterC = FindFirstObjectByType<Player>().gameObject.GetComponent<BoxCollider2D>();
        characterRB = FindFirstObjectByType<Player>().gameObject.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
        light2D.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        light2Dinside.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
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
