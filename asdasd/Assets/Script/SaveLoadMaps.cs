using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadMaps
{
    public static void SaveProgress(ProgressGatherer pg)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.dataPath + "/Progress.pgs";
        FileStream stream = new FileStream(path, FileMode.Create);
        Progress progress = new Progress(pg);
        bf.Serialize(stream, progress);
        stream.Close();
    }

    public static Progress LoadProgress()
    {
        Progress progress = new Progress(new ProgressGatherer());
        string path = Application.dataPath + "/Progress.pgs";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            progress = bf.Deserialize(stream) as Progress;
            stream.Close();
        }
        else
        {
            Debug.LogError("Save file not found in " + path + "\nStarting new game...");
        }

        return progress;
    }

    public static void SaveMap(Map map)
    {
        Debug.Log(Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.dataPath + "/maps/" + map.index + "map.map";
        FileStream stream = new FileStream(path, FileMode.Create);
        MapData data = new MapData(map);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static MapData LoadMap(string index)
    {
        string path = Application.dataPath + "/maps/" + index + "map.map";
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
