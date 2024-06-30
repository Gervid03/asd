using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PopUpHandler : MonoBehaviour
{
    public GameObject darkOverlay;
    public bool popUpActive;

    #region NewMapByArrow
    public GameObject newMapByArrow;
    private string newMapName;
    private int newMapX, newMapY;

    public void NewMapByArrowUp(int x, int y)
    {
        newMapX = x;
        newMapY = y;

        newMapByArrow.SetActive(true);
        DarkenUp();
        
        TMP_Text text = newMapByArrow.GetComponentInChildren<TMP_Text>();

        text.text = "Add map on (" + x + "; " + y + ")";
    }

    public void SetNewMapName(string text)
    {
        newMapName = text;
        SetButtonActive(text);
    }

    private void SetButtonActive(string mapName)
    {
        Button button = newMapByArrow.GetComponentInChildren<Button>();
 
        if (mapName == null || mapName == "")
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
            
            button.GetComponentInChildren<TMP_Text>().text = "Create";
            foreach (FileInfo f in SaveLoadMaps.GetMapList())
            {
                if (f.Name.Substring(0, f.Name.Length - 7) == mapName)
                {
                    button.GetComponentInChildren<TMP_Text>().text = "Add";
                    break;
                }
            }
        }
    }

    public void ButtonPress()
    {
        MapEditor me = FindFirstObjectByType<MapEditor>();
        if (me == null)
        {
            Debug.Log("MapEditor is null");
            return;
        }
        SaveLoadMaps.CreateEmptyMap(newMapName);
        me.mappack.NewMap(new MapEditor.Level(newMapName, newMapX, newMapY), true);
        me.GoToMap(newMapX, newMapY);

        EveryDown();
    }
    #endregion

    public void DarkenUp()
    {
        popUpActive = true;
        darkOverlay.SetActive(true);
    }

    public void EveryDown()
    {
        popUpActive = false;
        darkOverlay.SetActive(false);
        newMapByArrow.SetActive(false);
    }
}
