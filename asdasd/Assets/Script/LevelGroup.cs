using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New LevelGroup", menuName = "LevelGroup")]
public class LevelGroup : ScriptableObject
{
    public List<MultipleLevel.Level> levels;
}
