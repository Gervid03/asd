using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suicide : MonoBehaviour
{
    public InverseButton b1, b2;
    public MapEditor mapEditor;

    private void Start()
    {
        mapEditor = FindFirstObjectByType<MapEditor>();
    }

    public void CommitSucide()
    {
        mapEditor.countInversePair--;
        for(int i = 0; i < mapEditor.inversePairs.Count; i++)
        {
            if (mapEditor.inversePairs[i] == this.gameObject)
            {
                mapEditor.inversePairs.RemoveAt(i);
                break;
            }
        }
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 80 + mapEditor.countInversePair);

        if (b1.index != -1)
        {
            mapEditor.inverseColor[b1.index] = -1;
        }
        if (b2.index != -1)
        {
            mapEditor.inverseColor[b2.index] = -1;
        }

        Destroy(this.gameObject);
    }
}
