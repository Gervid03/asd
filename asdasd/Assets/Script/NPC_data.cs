using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC_data : ScriptableObject
{
    public string mapName;
    public int x, y;
    public string nameOfNPC;
    public List<MultiLanguage> text;
    public Sprite sprite;
    public Color32 color;
    public GameObject prefab;
    public bool justDecoration;

    public enum Language
    {
        EN = 0,
        HU = 1
    }

    [System.Serializable]
    public struct MultiLanguage
    {
        public string en;
        public string hu; 

        public string at(int ind)
        {
            switch (ind)
            {
                case 0: return en;
                case 1: return hu;
            }
            return "";
        }
    };

    public void Summon()
    {
        if (FindFirstObjectByType<Map>() != null)
        {
            Map m = FindFirstObjectByType<Map>();
            GameObject newNPC;
            if (prefab == null) newNPC = Instantiate(m.prefabOfNPC);
            else newNPC = Instantiate(prefab);
            newNPC.transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
            if(sprite != null) newNPC.GetComponent<SpriteRenderer>().sprite = sprite;
            newNPC.GetComponent<SpriteRenderer>().color = color;
            newNPC.GetComponent<NPC>().data = this;
            newNPC.GetComponent<NPC>().justDecoration = justDecoration;
        }
        else Debug.LogError("Map doesnt exist for summoning an NPC, fix it :)");
    }
}
