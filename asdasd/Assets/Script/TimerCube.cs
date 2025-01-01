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

    private void Awake()
    {
        WallManager.disableColor += DontBeActive;
        birthTime = Time.time;
    }

    public void Set()
    {
        gameObject.GetComponent<SpriteRenderer>().color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        characterC = FindFirstObjectByType<CubePlacer>().gameObject.GetComponent<BoxCollider2D>();
        characterRB = FindFirstObjectByType<CubePlacer>().gameObject.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
        light2D.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        light2Dinside.color = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    private void Update()
    {
        if (Time.time - birthTime > lifeTime)
        {
            ForceDestroy(colorIndex);
        }
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, Mathf.Max(-fallSpeedLimit, this.GetComponent<Rigidbody2D>().velocity.y));
    }

    public void DontBeActive(int c)
    {
        if (c != colorIndex || !FindFirstObjectByType<WallManager>().colors.atVisible(c)) return;
        FindFirstObjectByType<WallManager>().timerCubes.Remove(this);
        Destroy(this.gameObject);
    }

    public void ForceDestroy(int c)
    {
        if (c != colorIndex) return;
        FindFirstObjectByType<WallManager>().timerCubes.Remove(this);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        Debug.Log(name + " " + (Time.time - birthTime));
        WallManager.disableColor -= DontBeActive;
    }
}
