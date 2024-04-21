using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorTweaker : MonoBehaviour
{
    public GameObject r255, g255, b255;
    public Color color;
    public int IndexOfcolor;
    public GameObject colorDisplayImage, colorDisplaySprite;
    public GameObject inputMethodTexts, brightnessWarning;
    public bool notBrightEnough;

    public void AdjustBrightness()
    {
        if (Mathf.Max(color.r, color.g, color.b) > 0.98) return;
        float x = 1 / Mathf.Max(color.r, color.g, color.b);
        color.r = Mathf.Min(1, color.r * x);
        color.g = Mathf.Min(1, color.g * x);
        color.b = Mathf.Min(1, color.b * x);
        
        UpdateTextsFromColor();
        UpdateDisplayColor();
    }

    public void CheckBrightness()
    {
        FindAnyObjectByType<ColorPalette>().colorExistsWarning.SetActive(false);
        if (Mathf.Max(color.r, color.g, color.b) < 0.98)
        {
            brightnessWarning.SetActive(true);
            notBrightEnough = true;
        }
        else
        {
            brightnessWarning.SetActive(false);
            notBrightEnough = false;
        }
    }

    public void BeActive()
    {
        this.gameObject.SetActive(true);
        inputMethodTexts.SetActive(true);
        notBrightEnough = false;
        UpdateTextsFromColor();
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
        r255.GetComponent<TMP_InputField>().text = (color.r * 255).ToString();
        g255.GetComponent<TMP_InputField>().text = (color.g * 255).ToString();
        b255.GetComponent<TMP_InputField>().text = (color.b * 255).ToString();
    }

    public void UpdateRed(string r255Value)
    {
        if (r255Value == "" || float.Parse(r255Value) > 255 || float.Parse(r255Value) < 0) return;
        color.r = float.Parse(r255Value) / 255f;
        UpdateDisplayColor();
    }

    public void UpdateGreen(string g255Value)
    {
        if (g255Value.Length == 0 || float.Parse(g255Value) > 255 || float.Parse(g255Value) < 0) return;
        color.g = float.Parse(g255Value) / 255f;
        UpdateDisplayColor();
    }
    public void UpdateBlue(string b255Value)
    {
        if (b255Value == "" || float.Parse(b255Value) > 255 || float.Parse(b255Value) < 0) return;
        color.b = float.Parse(b255Value) / 255f;
        UpdateDisplayColor();
    }
}
