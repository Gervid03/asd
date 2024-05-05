using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Vault : MonoBehaviour
{
    public string playScene;
    public Intent intent;
    public string mapToLoad;
    public string mapUnderConstruction;

    public enum Intent
    {
        none = 0,
        loadTempIntoEditor = 1,
        playWithTemp = 2,
        loadMapToLoad = 3
    }

    void Awake()
    {
        Vault[] asd = FindObjectsByType<Vault>(default);
        if (asd.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        mapToLoad = "";
        intent = Intent.none;
        SceneManager.sceneLoaded += OnLoad;
    }

    private void Update()
    {
        if (Input.GetKeyDown("r") && (FindFirstObjectByType<MapEditor>() == null || !FindFirstObjectByType<MapEditor>().menu.activeSelf)) Load();
    }

    public void Load()
    {
        if (SceneManager.GetActiveScene().name == playScene)
        {
            intent = Intent.loadTempIntoEditor;
            SceneManager.LoadScene("MapEditor");

            return;
        }
        intent = Intent.playWithTemp;
        Map map = FindAnyObjectByType<Map>();

        mapUnderConstruction = map.index; //store map name to restore selected dropdown option

        FindAnyObjectByType<MapEditor>().GetInfos(map);
        map.index = "!temp";

        SaveLoadMaps.SaveMap(map);

        SceneManager.LoadScene(playScene);
    }

    public void OnLoad(Scene name, LoadSceneMode lsm)
    {
        if (name.name == playScene && intent == Intent.playWithTemp)
        {
            Map map = FindAnyObjectByType<Map>();
            map.index = "!temp";
            map.LoadMap();
            return;
        }
        else if (name.name == "MapEditor" && intent == Intent.loadTempIntoEditor)
        {
            Map map = FindAnyObjectByType<Map>();
            map.index = "!temp";
            map.LoadIntoEditor();
            return;
        }
    }
}
