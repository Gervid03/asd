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
        string path = Application.persistentDataPath + "/Progress.pgs";
        FileStream stream = new FileStream(path, FileMode.Create);
        Progress progress = new Progress(pg);
        bf.Serialize(stream, progress);
        stream.Close();
    }

    public static Progress LoadProgress()
    {
        Progress progress = new Progress(new ProgressGatherer());
        string path = Application.persistentDataPath + "/Progress.pgs";
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
        string path = Application.dataPath + "/maps/" + map.index + "map.map"; //Application.persistentDataPath + " / " + map.index + "map.map";
        FileStream stream = new FileStream(path, FileMode.Create);
        MapData data = new MapData(map);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static MapData LoadMap(string index)
    {
        string path = Application.dataPath + "/maps/" + index + "map.map"; //Application.persistentDataPath + " / " + index + "map.map";
        if (File.Exists(path))
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

    public static void SaveMappack(MapEditor.Mappack mappack)
    {
        Debug.Log(Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.dataPath + "/mappacks/" + mappack.ID + ".mappack";
        FileStream stream = new FileStream(path, FileMode.Create);
        MapEditor.Mappack.MappackData data = new MapEditor.Mappack.MappackData(mappack);
        bf.Serialize(stream, data);
        stream.Close();
    }
    public static MapEditor.Mappack.MappackData LoadMappack(string name)
    {
        string path = Application.dataPath + "/mappacks/" + name + ".mappack"; //Application.persistentDataPath + " / " + index + "map.map";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            MapEditor.Mappack.MappackData data = bf.Deserialize(stream) as MapEditor.Mappack.MappackData;
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