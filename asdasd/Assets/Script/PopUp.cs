using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PopUp
{
    protected PopUpHandler handler;
    protected GameObject window;
    public bool active = false;
    public AddNewMap addNewMap;
    public PauseScreen pauseScreen;
    //public ManaVision manaVision;
    //public MovementAndInteractionTutorial movementAndInteraction;
    //public ManaVisionTutorial manaVisionTutorial;
    //public ButtonsAndCubesTutorial buttonsAndCubesTutorial;
    //public TimercubesTutorial timercubesTutorial;

    public PopUp()
    {
        handler = MonoBehaviour.FindFirstObjectByType<PopUpHandler>();
        if (handler == null)
        {
            Debug.Log("handler is null");
            return;
        }
    }

    public virtual void Up()
    {
        active = true;
        handler.activePopUps++;
        window.SetActive(true);
    }

    public virtual void Down()
    {
        active = false;
        if (window.activeInHierarchy)
        { 
            window.SetActive(false);
            handler.activePopUps--; //handles dark layer automatically
        }
    }

    public class AddNewMap : PopUp
    {
        private MapEditor mapEditor;
        private string newMapName;
        private int newMapX, newMapY;

        public AddNewMap() //automatically calls base constructor before runs (since it's parameterless)
        {
            window = handler.transform.Find("NewMapByArrow").gameObject;
            addNewMap = this;

            mapEditor = MonoBehaviour.FindFirstObjectByType<MapEditor>();
            if (mapEditor == null)
            {
                Debug.Log("MapEditor is null");
                return;
            }
            mapEditor.newMapPopUp = this; //let mapEditor know, since this isn't MonoBehaviour
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

        public void ButtonPress() //TODO load map's name into savename
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

    public class PauseScreen : PopUp
    {
        public PauseScreen()
        {
            window = handler.transform.Find("PauseWindow").gameObject;
            pauseScreen = this;
        }
        
        public void BackToMainMenu()
        {
            SceneLoader.LoadSceneByName("Menu");
        }
    }

    public class ManaVision : PopUp
    {
        public ManaVision()
        {
            window = handler.transform.Find("ManaVision").gameObject;
            handler.manaVisionTextParent = window;
            //manaVision = this;
        }

        public override void Up()
        {
            base.Up();
            handler.StartTextAnimation();
            handler.CreateManaVisionTexts();
            Time.timeScale = 0f;
        }

        public override void Down()
        {
            Time.timeScale = 1f;
            handler.StopTextAnimation();
            handler.DeleteManaVisionTexts();
            base.Down();
        }
    }

    //Tutorial popups
    public class MovementAndInteractionTutorial : PopUp
    {
        public MovementAndInteractionTutorial()
        {
            window = handler.transform.Find("MovementAndInteractionTutorial").gameObject;
            //movementAndInteraction = this;
        }
    }
    public class ManaVisionTutorial : PopUp
    {
        public ManaVisionTutorial()
        {
            window = handler.transform.Find("ManaVisionTutorial").gameObject;
            //manaVisionTutorial = this;
        }
    }
    public class ButtonsAndCubesTutorial : PopUp
    {
        public ButtonsAndCubesTutorial()
        {
            window = handler.transform.Find("ButtonsAndCubesTutorial").gameObject;
            //buttonsAndCubesTutorial = this;
        }
    }
    public class TimercubesTutorial : PopUp
    {
        public TimercubesTutorial()
        {
            window = handler.transform.Find("TimercubesTutorial").gameObject;
            //timercubesTutorial = this;
        }
    }
}
