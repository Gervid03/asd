using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadTestScene() //map testing "r"
    {
        Scene mapEditorScene = SceneManager.GetSceneByName("MapEditor");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TestTempMap", LoadSceneMode.Additive);

        // Use the completed callback to set the active scene once loading is complete
        asyncLoad.completed += (AsyncOperation op) =>
        {
            Scene testMapScene = SceneManager.GetSceneByName("TestTempMap");
            
            SetSceneActive(mapEditorScene, false); // Deactivate the mapeditor

            SetSceneActive(testMapScene, true); // Activate the testing scene
        };
    }

    public static void LoadMapEditor()
    {
        Scene mapEditorScene = SceneManager.GetSceneByName("MapEditor");
        Scene testMapScene = SceneManager.GetSceneByName("TestTempMap");

        SetSceneActive(mapEditorScene, true);

        SetSceneActive(testMapScene, false);

        SceneManager.UnloadSceneAsync(testMapScene);// Unload the testing scene
    }

    private static void SetSceneActive(Scene scene, bool isActive)
    {
        if (isActive)
        {
            SceneManager.SetActiveScene(scene);
            foreach (GameObject gobject in scene.GetRootGameObjects())
            {
                gobject.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject gobject in scene.GetRootGameObjects())
            {
                if (gobject.name == "global" || gobject.name == "EventSystem" || gobject.name == "PopUpParent") continue;
                gobject.SetActive(false);
            }
        }
    }

    public static void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
