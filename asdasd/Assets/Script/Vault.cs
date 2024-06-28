//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Vault : MonoBehaviour
//{
//    public string playScene;
//    public Intent intent;
//    public string mapToLoad;
//    public string mapUnderConstruction;
//    public MapEditor.Mappack mappack;
//    public string mapName;

//    public enum Intent
//    {
//        none = 0,
//        loadTempIntoEditor = 1,
//        playWithTemp = 2,
//        loadMapToLoad = 3
//    }

//    void Awake()
//    {
//        Vault[] asd = FindObjectsByType<Vault>(default);
//        if (asd.Length > 1)
//        {
//            Destroy(this.gameObject);
//        }
//        DontDestroyOnLoad(this.gameObject);
//        mapToLoad = "";
//        intent = Intent.none;
//        mappack = new MapEditor.Mappack("N/A", new MapEditor.Level[] { });
//        SceneManager.sceneLoaded += OnLoad;
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown("r") && (FindFirstObjectByType<MapEditor>() == null || !FindFirstObjectByType<MapEditor>().menu.activeSelf)) Load();
//    }

//    public void Load()
//    {
//        if (SceneManager.GetActiveScene().name == playScene)
//        {
//            intent = Intent.loadTempIntoEditor;
//            SceneManager.LoadScene("MapEditor");

//            return;
//        }

//        mappack = FindFirstObjectByType<MapEditor>().mappack;

//        intent = Intent.playWithTemp;
//        Map map = FindFirstObjectByType<Map>();

//        mapUnderConstruction = map.index; //store map name to restore selected dropdown option

//        FindFirstObjectByType<MapEditor>().GetInfos(map);
//        map.index = "!temp";

//        SaveLoadMaps.SaveMap(map);

//        SceneManager.LoadScene(playScene);
//    }

//    public void OnLoad(Scene name, LoadSceneMode lsm)
//    {
//        if (name.name == playScene && intent == Intent.playWithTemp)
//        {
//            Map map = FindFirstObjectByType<Map>();
//            map.index = "!temp";
//            map.LoadMap();
//            return;
//        }
//        else if (name.name == "MapEditor" && intent == Intent.loadTempIntoEditor)
//        {
//            Map map = FindFirstObjectByType<Map>();
//            map.index = "!temp";
//            map.LoadIntoEditor();
//            return;
//        }
//        else if (name.name == "MapEditor" && intent == Intent.loadMapToLoad)
//        {
//            FindFirstObjectByType<MapEditor>().mappack = mappack;
//            FindFirstObjectByType<MapEditor>().mapName = mapName;
//        }
//    }
//}
