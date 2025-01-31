using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuManager : MonoBehaviour
{
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
        File.Copy(Application.streamingAssetsPath + "/BackupProgress.pgs", Application.persistentDataPath + "/Progress.pgs", true);
    }
}
