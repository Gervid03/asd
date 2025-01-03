using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PopUpHandler : MonoBehaviour
{
    private GameObject darkOverlay;
    private PopUp[] popUps;
    public bool popupActive;

    private int _activePopUps;
    public int activePopUps //Dark layer reacts to this variable
    {
        get
        {
            return _activePopUps;
        }
        set
        {
            if (value < 0)
            {
                Debug.Log("negative number of popups are on screen");
            }
            else if (value == 0)
            {
                popupActive = false;
                DarkenDown();
            }
            else
            {
                popupActive = true;
                DarkenUp();
            }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupActive) Down(-1);
            else if (SceneManager.GetActiveScene().name != "MapEditor") popUps[1].Up(); //raise menu
        }
    }

    public void DarkenUp()
    {
        darkOverlay.SetActive(true);
    }

    public void DarkenDown()
    {
        darkOverlay.SetActive(false);
    }

    public void Down(int id = -1) //-1 Disables all popups
    {
        if (id == -1)
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                popUps[i].Down();
            }
        }
        else popUps[id].Down();
    }


    public void SetNewMapNameByInputField(string text) => popUps[0].addNewMap.SetNewMapName(text);
    public void NewMapButtonPress() => popUps[0].addNewMap.ButtonPress();

    public void BackToMainMenu() => popUps[1].pauseScreen.BackToMainMenu();
}
