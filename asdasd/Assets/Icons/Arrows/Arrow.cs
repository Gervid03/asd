using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public float xBottomLeft, yBottomLeft, xTopRight, yTopRight; //trigger zone
    public float xBottomLeft2, yBottomLeft2, xTopRight2, yTopRight2; //not un-trigger zone
    public float xBottomLeft3, yBottomLeft3, xTopRight3, yTopRight3; //ignore mouse clicks to editor
    public Sprite normalSprite, indicateNewSprite;
    public GameObject arrow;
    private float lastInArea = -1;
    private MapEditor mapEditor;
    private bool IveSetMouseOnArrow = false;

    private bool mouseTrigger()
    {
        float x = (Input.mousePosition.x - (1920 / 2)) / (1920 / 32), y = (Input.mousePosition.y - (1080 / 2)) / (1080 / 18);

        return xBottomLeft < x && x < xTopRight && yBottomLeft < y && y < yTopRight;
    }

    private bool mouseInArea()
    {
        float x = (Input.mousePosition.x - (1920 / 2)) / (1920 / 32), y = (Input.mousePosition.y - (1080 / 2)) / (1080 / 18);

        return xBottomLeft2 < x && x < xTopRight2 && yBottomLeft2 < y && y < yTopRight2;
    }

    private void mouseOnArrow() //tell MapEditor to ignore clicks
    {
        float x = (Input.mousePosition.x - (1920 / 2)) / (1920 / 32), y = (Input.mousePosition.y - (1080 / 2)) / (1080 / 18);

        if (arrow.activeSelf && xBottomLeft2 < x && x < xTopRight2 && yBottomLeft2 < y && y < yTopRight2)
        {
            mapEditor.mouseOnArrow = true;
            IveSetMouseOnArrow = true;
        }
        else if (IveSetMouseOnArrow)
        {
            mapEditor.mouseOnArrow = false;
            IveSetMouseOnArrow = false;
        }
    }

    private void Start()
    {
        mapEditor = FindFirstObjectByType<MapEditor>();
    }

    void Update()
    {
        mouseOnArrow();

        if (!mapEditor.menu.activeSelf && mouseTrigger())
        {
            arrow.SetActive(true);
            lastInArea = Time.time;
        }
        else if (!mapEditor.menu.activeSelf && mouseInArea())
        {
            lastInArea = Time.time;
        }
        else if (mapEditor.menu.activeSelf || Time.time - lastInArea > 0.2)
        {
            arrow.SetActive(false);
        }
    }

    public void SetIndication(bool indicateNew)
    {
        arrow.GetComponent<Image>().sprite = indicateNew ? indicateNewSprite : normalSprite;
    }
}
