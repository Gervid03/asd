using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New NPCList", menuName = "NPCList")]
public class NPCList : ScriptableObject
{
    public List<NPC_data> data;
}
