using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vault : MonoBehaviour
{
    public string mapToLoad;

    void Awake()
    {
        Vault[] asd = FindObjectsByType<Vault>(default);
        if (asd.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void GetDataBack()
    {
        if (mapToLoad == "") return;

        FindFirstObjectByType<Map>().index = mapToLoad;
        FindFirstObjectByType<Map>().LoadIntoEditor();
        mapToLoad = "";
    }
}
