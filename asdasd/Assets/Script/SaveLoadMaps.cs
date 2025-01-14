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
        Debug.Log(path);
        Debug.Log(Application.persistentDataPath);
        FileStream stream = new FileStream(path, FileMode.Create);
        Progress progress = new Progress(pg);
        bf.Serialize(stream, progress);
        stream.Close();
    }

    public static void DeleteProgress()
    {
        string path = Application.persistentDataPath + "/Progress.pgs";
        File.Delete(path);
    }

    public static Progress LoadProgress()
    {
        string path = Application.persistentDataPath + "/Progress.pgs";
        if (!File.Exists(path)) path = Application.streamingAssetsPath + "/BackupProgress.pgs";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Progress progress = bf.Deserialize(stream) as Progress;
            stream.Close();
            return progress;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
        return new Progress(new ProgressGatherer());
    }

    public static FileInfo[] GetMapList()
    {
        string path = Application.streamingAssetsPath + "/maps"; //Application.persitentDataPath
        DirectoryInfo dir = new DirectoryInfo(path);
        return dir.GetFiles("*.map");
    }

    public static void SaveMap(Map map)
    {
        Debug.Log(Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.streamingAssetsPath + "/maps/" + map.index + "map.map"; //Application.persistentDataPath + " / " + map.index + "map.map";
        FileStream stream = new FileStream(path, FileMode.Create);
        MapData data = new MapData(map);
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static MapData LoadMap(string index)
    {
        string path = Application.streamingAssetsPath + "/maps/" + index + "map.map"; //Application.persistentDataPath + " / " + index + "map.map";
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

    public static void CreateEmptyMap(string name, bool overwrite = false)
    {
        string clearPath = Application.persistentDataPath + "/maps/" + "!clear" + "map.map";
        string newPath = Application.persistentDataPath + "/maps/" + name + "map.map";
        
        if (File.Exists(newPath) && !overwrite)
        {
            Debug.Log("There is already a map with this name, not allowed to overwrite!");
        }

        File.Copy(clearPath, newPath);
    }

    public static void SaveMappack(MapEditor.Mappack mappack)
    {
        Debug.Log(Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/mappacks/" + mappack.ID + ".mappack";
        FileStream stream = new FileStream(path, FileMode.Create);
        MapEditor.Mappack.MappackData data = new MapEditor.Mappack.MappackData(mappack);
        bf.Serialize(stream, data);
        stream.Close();
    }
    public static MapEditor.Mappack.MappackData LoadMappack(string name)
    {
        string path = Application.persistentDataPath + "/mappacks/" + name + ".mappack"; //Application.persistentDataPath + " / " + index + "map.map";
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

    /*public static void SaveLevelGroup(LevelGroupData levelGroupData)
    {
        Debug.Log(Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/levelgroups/levelgroupdata/" + levelGroupData.ID + ".lg";
        FileStream stream = new FileStream(path, FileMode.Create);
        bf.Serialize(stream, levelGroupData);
        stream.Close();
    }*/
}