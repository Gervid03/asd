using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC_data : ScriptableObject
{
    public string mapName;
    public int x, y;
    public string nameOfNPC;
    public List<string> text;
    public Sprite sprite;
    public Color color;

    public void Summon()
    {
        if (FindFirstObjectByType<Map>() != null)
        {
            Map m = FindFirstObjectByType<Map>();
            GameObject newNPC = Instantiate(m.prefabOfNPC);
            newNPC.transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
            newNPC.GetComponent<SpriteRenderer>().sprite = sprite;
            newNPC.GetComponent<SpriteRenderer>().color = color;
            newNPC.GetComponent<NPC>().data = this;
        }
        else Debug.LogError("Map doesnt exist for summoning an NPC, fix it :)");
    }
}
