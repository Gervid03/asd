using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorTweaker : MonoBehaviour
{
    public GameObject r255, g255, b255, r1, g1, b1;
    public Color color;
    public int IndexOfcolor;
    public GameObject colorDisplayImage, colorDisplaySprite;
    public GameObject inputMethodTexts;

    public void BeActive()
    {
        this.gameObject.SetActive(true);
        inputMethodTexts.SetActive(true);
    }

    public void BeDeactive()
    {
        this.gameObject.SetActive(false);
        inputMethodTexts.SetActive(false);
    }

    public void UpdateDisplayColor()
    {
        colorDisplayImage.GetComponent<Image>().color = color;
        colorDisplaySprite.GetComponent<Image>().color = color;
    }

    public void UpdateTextsFromColor()
    {
        UpdateRed1((color.r*255).ToString());
        UpdateRed255(color.r.ToString());
        UpdateGreen1((color.g*255).ToString());
        UpdateGreen255(color.g.ToString());
        UpdateBlue1((color.b*255).ToString());
        UpdateBlue255(color.b.ToString());
    }

    public void UpdateRed1(string r255Value)
    {
        if (r255Value == "" || float.Parse(r255Value) > 255 || float.Parse(r255Value) < 0)
        {
            r1.GetComponent<TMP_InputField>().text = "0";

            return;
        }
        r1.GetComponent<TMP_InputField>().text = (float.Parse(r255Value)/255f).ToString();
        color.r = float.Parse(r255Value) / 255f;
        UpdateDisplayColor();
    }
    public void UpdateRed255(string r1Value)
    {
        if (r1Value == "" || float.Parse(r1Value) > 1 || float.Parse(r1Value) < 0)
        {
            r255.GetComponent<TMP_InputField>().text = "0";
            return;
        }
        r255.GetComponent<TMP_InputField>().text = Mathf.RoundToInt((float.Parse(r1Value) * 255f)).ToString();
        color.r = float.Parse(r1Value);
        UpdateDisplayColor();
    }
    public void UpdateGreen1(string g255Value)
    {
        if (g255Value.Length == 0 || float.Parse(g255Value) > 255 || float.Parse(g255Value) < 0)
        {
            g1.GetComponent<TMP_InputField>().text = "0";
            return;
        }
        g1.GetComponent<TMP_InputField>().text = (float.Parse(g255Value) / 255f).ToString();
        color.g = float.Parse(g255Value) / 255f;
        UpdateDisplayColor();
    }
    public void UpdateGreen255(string g1Value)
    {
        if (g1Value == "" || float.Parse(g1Value) > 1 || float.Parse(g1Value) < 0)
        {
            g255.GetComponent<TMP_InputField>().text = "0";
            return;
        }
        g255.GetComponent<TMP_InputField>().text = Mathf.RoundToInt((float.Parse(g1Value) * 255f)).ToString();
        color.g = float.Parse(g1Value);
        UpdateDisplayColor();
    }
    public void UpdateBlue1(string b255Value)
    {
        if (b255Value == "" || float.Parse(b255Value) > 255 || float.Parse(b255Value) < 0)
        {
            b1.GetComponent<TMP_InputField>().text = "0";
            return;
        }
        b1.GetComponent<TMP_InputField>().text = (float.Parse(b255Value) / 255f).ToString();
        color.b = float.Parse(b255Value) / 255f;
        UpdateDisplayColor();
    }
    public void UpdateBlue255(string b1Value)
    {
        if (b1Value == "" || float.Parse(b1Value) > 1 || float.Parse(b1Value) < 0)
        {
            b255.GetComponent<TMP_InputField>().text = "0";
            return;
        }
        b255.GetComponent<TMP_InputField>().text = Mathf.RoundToInt((float.Parse(b1Value) * 255f)).ToString();
        color.b = float.Parse(b1Value);
        UpdateDisplayColor();
    }
}
