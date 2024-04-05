using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadMaps
{
    public static void SaveMap(Map map)
    {
        Debug.Log(Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + map.index + "map.map";
        FileStream stream = new FileStream(path, FileMode.Create);
        MapData data = new MapData(map);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static MapData LoadMap(int index)
    {
        string path = Application.persistentDataPath + "/" + index + "map.map";
        if(File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            MapData data = bf.Deserialize(stream) as MapData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
