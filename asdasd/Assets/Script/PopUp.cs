using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PopUp : MonoBehaviour
{
    protected readonly PopUpHandler handler;
    protected readonly GameObject window;

    public PopUp()
    {
        handler = FindFirstObjectByType<PopUpHandler>();
        if (handler == null)
        {
            Debug.Log("handler is null");
            return;
        }

        window = handler.transform.Find("NewMapByArrow").gameObject;
    }

    public virtual void Up()
    {
        handler.DarkenUp();
        window.SetActive(true);
    }

    public virtual void Down()
    {
        window.SetActive(false);
        if (--handler.activePopUps == 0)
        {
            handler.Down();
        }
    }

    public class AddNewMap : PopUp
    {
        private MapEditor mapEditor;
        private string newMapName;
        private int newMapX, newMapY;

        public AddNewMap() //automatically calls base constructor before runs (since it's parameterless)
        {
            mapEditor = FindFirstObjectByType<MapEditor>();
            if (mapEditor == null)
            {
                Debug.Log("MapEditor is null");
                return;
            }
        }

        public void Set(int x, int y, bool up)
        {
            newMapX = x;
            newMapY = y;

            TMP_Text text = window.GetComponentInChildren<TMP_Text>();

            text.text = "Adding map on (" + x + "; " + y + ")";

            if (up) Up();
        }

        public void SetNewMapName(string text) //Set by inputfield
        {
            newMapName = text;
            SetButtonActive(text);
        }

        private void SetButtonActive(string mapName)
        {
            Button button = window.GetComponentInChildren<Button>();

            if (mapName == null || mapName == "")
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;

                button.GetComponentInChildren<TMP_Text>().text = "Create";
                foreach (FileInfo f in SaveLoadMaps.GetMapList()) //check if map exists
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
            bool exists = false;

            foreach (FileInfo f in SaveLoadMaps.GetMapList()) //check if map exists
            {
                if (f.Name.Substring(0, f.Name.Length - 7) == newMapName)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists) SaveLoadMaps.CreateEmptyMap(newMapName);
            mapEditor.mappack.NewMap(new MapEditor.Level(newMapName, newMapX, newMapY), true);
            mapEditor.GoToMap(newMapX, newMapY);

            Down();
        }
    }
}
