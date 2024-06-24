using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Drag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform target;
    private bool isMouseDown = false;
    private Vector3 startMousePosition;
    private Vector3 startPosition;
    public bool shouldReturn;

    public bool getIsMouseDown()
    {
        return isMouseDown;
    }

    public void OnPointerDown(PointerEventData dt)
    {
        isMouseDown = true;

        startPosition = target.position;
        startMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData dt)
    {
        isMouseDown = false;

        if (shouldReturn)
        {
            target.position = startPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown)
        {
            Vector3 currentPosition = Input.mousePosition;

            Vector3 diff = currentPosition - startMousePosition;

            Vector3 pos = startPosition + diff;
            if (pos.x < 0) pos.x = 0;
            if (pos.x > 1420) pos.x = 1420;
            if (pos.y < 0) pos.y = 0;
            if (pos.y > 950) pos.y = 950;


            target.position = pos;
        }
    }
}