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

    // Use this for initialization
    void Start()
    {

    }

    public void OnPointerDown(PointerEventData dt)
    {
        isMouseDown = true;

        Debug.Log("Draggable Mouse Down");

        startPosition = target.position;
        startMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData dt)
    {
        Debug.Log("Draggable mouse up");

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
            if (currentPosition.x < 0) currentPosition.x = 0;
            if (currentPosition.x > 1500) currentPosition.x = 1500;
            if (currentPosition.y < 0) currentPosition.y = 0;
            if (currentPosition.y > 800) currentPosition.y = 800;

            Vector3 diff = currentPosition - startMousePosition;

            Vector3 pos = startPosition + diff;

            target.position = pos;
        }
    }
}