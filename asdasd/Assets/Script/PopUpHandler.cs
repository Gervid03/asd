using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PopUpHandler : MonoBehaviour
{
    public bool popupActive;
    private GameObject darkOverlay;
    private PopUp[] popUps;
    private int _activePopUps;
    public int activePopUps
    {
        get
        {
            return _activePopUps;
        }
        set
        {
            if (value < 0)
            {
                _activePopUps = 0;
                popupActive = false;
                return;
            }
            popupActive = value != 0; //store wheter atleast a single popup is active or not
            _activePopUps = value;
        }
    }

    void Start()
    {
        darkOverlay = transform.Find("DarkLayer").gameObject; //find returns the this.transform's child's transform with a specific name
        popUps = new PopUp[]
        {
            new PopUp.AddNewMap(),
            new PopUp.PauseScreen()
        };
    }

    public void DarkenUp()
    {
        popupActive = true;
        darkOverlay.SetActive(true);
    }

    public void DarkenDown()
    {
        darkOverlay.SetActive(false);
    }

    public void Down() //Disables all popups
    {
        DarkenDown();

        for (int i = 0; i < popUps.Length; i++)
        {
            popUps[i].Down();
        }
    }


    public void SetNewMapNameByInputField(string text) => popUps[0].addNewMap.SetNewMapName(text);
    public void NewMapButtonPress() => popUps[0].addNewMap.ButtonPress();

    public void BackToMainMenu() => popUps[1].pauseScreen.BackToMainMenu();
    public void ResetProgress() => popUps[1].pauseScreen.ResetProgress(FindFirstObjectByType<MultipleLevel>());
}
