using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        UpdateResetButton();
    }
    private void UpdateResetButton()
    {
        if (SceneManager.GetActiveScene().name == "Menu") GameObject.Find("ResetButton").GetComponent<Button>().interactable = (PlayerPrefs.GetInt("MovementAndInteractionTutorial", 0) == 1);
    }
    public void LoadSceneFromMenu(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void ResetProgress() //overwrite progress with an empty one
    {
        PlayerPrefs.SetInt("MovementAndInteractionTutorial", 0);
        PlayerPrefs.SetInt("ManaVisionTutorial", 0);
        PlayerPrefs.SetInt("ButtonsAndCubesTutorial", 0);
        PlayerPrefs.SetInt("TimercubesTutorial", 0);
        File.Copy(Application.streamingAssetsPath + "/BackupProgress.pgs", Application.streamingAssetsPath + "/Progress.pgs", true);

        UpdateResetButton();
    }
}
