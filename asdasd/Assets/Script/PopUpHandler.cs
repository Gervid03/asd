using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpHandler : MonoBehaviour
{
    public GameObject DarkOverlay;
    public GameObject NewMapByArrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NewMapByArrowUp()
    {
        NewMapByArrow.SetActive(true);
        DarkenUp();
    }

    public void DarkenUp()
    {
        DarkOverlay.SetActive(true);
    }

    public void DarkenDown()
    {
        DarkOverlay.SetActive(false);
    }
}
