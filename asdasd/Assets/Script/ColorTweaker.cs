using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorTweaker : MonoBehaviour
{
    public GameObject r255, g255, b255;
    public Color32 color;
    public int IndexOfcolor;
    public GameObject colorDisplayImage, colorDisplaySprite;
    public GameObject inputMethodTexts, brightnessWarning;
    public bool notBrightEnough;

    public void AdjustBrightness()
    {
        if ((byte) Mathf.Max(color.r, color.g, color.b) == 255) return;
        float x = 1 / Mathf.Max(color.r, color.g, color.b);
        color.r = (byte) Mathf.Min(255, Mathf.RoundToInt(color.r * x));
        color.g = (byte) Mathf.Min(255, Mathf.RoundToInt(color.g * x));
        color.b = (byte) Mathf.Min(255, Mathf.RoundToInt(color.b * x));
        
        UpdateTextsFromColor();
        UpdateDisplayColor();
    }

    public void CheckBrightness()
    {
        FindAnyObjectByType<ColorPalette>().colorExistsWarning.SetActive(false);
        if (Mathf.Max(color.r, color.g, color.b) < 255)
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
        r255.GetComponent<TMP_InputField>().text = (color.r).ToString();
        g255.GetComponent<TMP_InputField>().text = (color.g).ToString();
        b255.GetComponent<TMP_InputField>().text = (color.b).ToString();
    }

    public void UpdateRed(string r255Value)
    {
        if (r255Value == "" || int.Parse(r255Value) > 255 || int.Parse(r255Value) < 0) return;
        color.r = byte.Parse(r255Value);
        UpdateDisplayColor();
    }

    public void UpdateGreen(string g255Value)
    {
        if (g255Value == "" || int.Parse(g255Value) > 255 || int.Parse(g255Value) < 0) return;
        color.g = byte.Parse(g255Value);
        UpdateDisplayColor();
    }
    public void UpdateBlue(string b255Value)
    {
        if (b255Value == "" || int.Parse(b255Value) > 255 || int.Parse(b255Value) < 0) return;
        color.b = byte.Parse(b255Value);
        UpdateDisplayColor();
    }
}
