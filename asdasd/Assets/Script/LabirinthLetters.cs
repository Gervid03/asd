using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LabirinthLetters : MonoBehaviour
{
    SpriteRenderer sR;
    Light2D l;
    public List<Sprite> sprites0;
    public List<Sprite> sprites1;
    public int number;
    public C c;

    public enum C
    {
        None = 0,
        Blue = 1,
        Red = 2,
        Green = 3,
        Yellow = 4
    }

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        l = GetComponentInChildren<Light2D>();
        if (number == 0) sR.sprite = sprites0[Random.Range(0, sprites0.Count)];
        else sR.sprite = sprites1[Random.Range(0, sprites1.Count)];

        if (c == C.Blue) sR.color = Color.cyan;
        else if (c == C.Red) sR.color = Color.red;
        else if (c == C.Green) sR.color = Color.green;
        else if (c == C.Yellow) sR.color = Color.yellow;

        if (c == C.Blue) l.color = Color.cyan;
        else if (c == C.Red) l.color = Color.red;
        else if (c == C.Green) l.color = Color.green;
        else if (c == C.Yellow) l.color = Color.yellow;
    }
}
