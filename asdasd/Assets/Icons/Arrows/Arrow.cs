using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float xBottomLeft, yBottomLeft, xTopRight, yTopRight; //trigger zone
    public float xBottomLeft2, yBottomLeft2, xTopRight2, yTopRight2; //not un-trigger zone
    public Sprite normalSprite, indicateNewSprite;
    public GameObject arrow;
    private float lastInArea = -1;

    private bool mouseTrigger()
    {
        float x = (Input.mousePosition.x-(1920/2)) / (1920/32), y = (Input.mousePosition.y-(1080/2)) / (1080/18);

        return xBottomLeft < x && x < xTopRight && yBottomLeft < y && y < yTopRight;
    }

    private bool mouseInArea()
    {
        float x = (Input.mousePosition.x-(1920/2)) / (1920/32), y = (Input.mousePosition.y-(1080/2)) / (1080/18);

        return xBottomLeft2 < x && x < xTopRight2 && yBottomLeft2 < y && y < yTopRight2;
    }

    void Update()
    {
        if (mouseTrigger())
        {
            arrow.SetActive(true);
            lastInArea = Time.time;
        }
        else if (mouseInArea())
        {
            lastInArea = Time.time;
        }
        else if (Time.time - lastInArea > 0.2)
        {
            arrow.SetActive(false);
        }
    }

    public void SetIndication(bool indicateNew)
    {
        arrow.GetComponent<SpriteRenderer>().sprite = indicateNew ? indicateNewSprite : normalSprite;
    }
}
