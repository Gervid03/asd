using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//LevelGroups are ScriptableObjects in the Assets folder, accessible by every script
//To make the campaign, mappacks aren't enough as they can't reset colors and set hard CPs and currently they aren't even supported in the play scene
//The idea is to have a pack of Mappacks and convert them to a LevelGroup
//There will be always only one (possibly another for test scene) LevelGroup always overwritten
//LevelGroup(Data)s are stored similarly to maps and mappacks and can be converted into real LevelGroups

[System.Serializable]
public class LevelGroupData: MonoBehaviour
{
    /*public string ID;
    public int x, y;
    public List<MultipleLevel.Level> levels;

    public LevelGroupData(string ID, MapEditor.Mappack.MappackData[] mappacks, (int, int)[] afterEndRooms, (int, int)[] startRooms = null) //!afterEndRooms mustn't exist as maps, just coords to place next pack's start!
    {
        if (mappacks == null || afterEndRooms == null || mappacks.Length != afterEndRooms.Length)
        {
            Debug.Log("mappacks' conversion failed");
            return;
        }

        int mapcount = mappacks.Length;

        if (startRooms == null) //assume that each mappack starts at (0; 0)
        {
            startRooms = new (int, int)[mapcount];
            for (int i = 0; i < mapcount; i++)
                startRooms[i] = (0, 0);
        }

        MultipleLevel.Level[] levels = new MultipleLevel.Level[mapcount];

        int xdiff = -startRooms[0].Item1, ydiff = -startRooms[0].Item2; //the levelgroup will start at (0; 0)

        for (int j = 0; j < mapcount; j++)
        {
            for (int i = 0; i < mapcount; i++)
            {
                MultipleLevel.Level l = new MultipleLevel.Level();
                l.Set(mappacks[0].levels[i].x + xdiff, mappacks[0].levels[i].y + ydiff, mappacks[0].levels[i].name);

                l.missingLeft = mappacks[0].levels[i].missing[0];
                l.missingRight = mappacks[0].levels[i].missing[1];
                l.missingUp = mappacks[0].levels[i].missing[2];
                l.missingDown = mappacks[0].levels[i].missing[3];

                l.isSpecial = startRooms[i] == (mappacks[0].levels[i].x, mappacks[0].levels[i].y); //start room is special, requireing resets
                //l.comeFrom = ??? TODO

                levels[j] = l;
            }

            xdiff += afterEndRooms[j].Item1 - startRooms[j].Item1; 
            ydiff += afterEndRooms[j].Item2 - startRooms[j].Item2; 
        }
    }
    //TODO call these from somewhere
    public void ConvertToLevelGroup(string Name) //this will only overwrite levelgroups that exists!
    {
        string assetPath = Application.streamingAssetsPath + "/levelgroups/levelgroupdata/" + Name + ".asset";
        LevelGroup lg = AssetDatabase.LoadAssetAtPath<LevelGroup>(assetPath);

        if (lg != null)
        {
            lg.x = x;
            lg.y = y;

            lg.levels.Clear();
            foreach (MultipleLevel.Level l in levels)
            {
                lg.levels.Add(l);
            }

            EditorUtility.SetDirty(lg); //let Unity know that lg got updated
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log(Name + " no such levelgroups exists");
        }
    }*/
}
