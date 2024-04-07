using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStateHeight : MonoBehaviour
{
    public void AdjustHeight(int height)
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(700, height);
    }
}
