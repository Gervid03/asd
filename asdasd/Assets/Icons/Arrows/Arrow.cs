using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public float xBottomLeft, yBottomLeft, xTopRight, yTopRight; //trigger zone
    public float xBottomLeft2, yBottomLeft2, xTopRight2, yTopRight2; //not un-trigger zone
    public float xBottomLeft3, yBottomLeft3, xTopRight3, yTopRight3; //ignore mouse clicks to editor
    public GameObject normalArrow, creatingArrow;
    private float lastInArea = -1;
    private MapEditor mapEditor;
    private bool iveSetMouseOnArrow = false;
    private bool arrowType = true;

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

        if ((normalArrow.activeSelf || creatingArrow.activeSelf) && xBottomLeft3 < x && x < xTopRight3 && yBottomLeft3 < y && y < yTopRight3)
        {
            mapEditor.mouseOnArrow = true;
            iveSetMouseOnArrow = true;
        }
        else if (iveSetMouseOnArrow)
        {
            mapEditor.mouseOnArrow = false;
            iveSetMouseOnArrow = false;
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
            if (arrowType) creatingArrow.SetActive(true);
            else normalArrow.SetActive(true);

            lastInArea = Time.time;
        }
        else if (!mapEditor.menu.activeSelf && mouseInArea())
        {
            lastInArea = Time.time;
        }
        else if (mapEditor.menu.activeSelf || Time.time - lastInArea > 0.2)
        {
            normalArrow.SetActive(false);
            creatingArrow.SetActive(false);
        }
    }

    public void SetIndication(bool indicateNew)
    {
        arrowType = indicateNew;

        if (arrowType && normalArrow.activeSelf)
        {
            creatingArrow.SetActive(true);
            normalArrow.SetActive(false);
        }
        else if (!arrowType && creatingArrow.activeSelf)
        {
            normalArrow.SetActive(true);
            creatingArrow.SetActive(false);
        }
    }
}
