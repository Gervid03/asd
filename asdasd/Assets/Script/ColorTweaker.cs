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
        if ((int)color.r + (int)color.g + (int)color.b == 0) { color.r = 1; color.g = 1; color.b = 1; } //no division with 0

        if (((int)color.r + (int)color.g + (int)color.b) >= 255) return;
        float x = 255f / (float)((int)color.r + (int)color.g + (int)color.b);
        color.r = (byte) Mathf.Min(255, Mathf.CeilToInt(color.r * x));
        color.g = (byte) Mathf.Min(255, Mathf.CeilToInt(color.g * x));
        color.b = (byte) Mathf.Min(255, Mathf.CeilToInt(color.b * x));
        
        UpdateTextsFromColor();
        UpdateDisplayColor();
    }

    public void CheckBrightness()
    {
        FindFirstObjectByType<ColorPalette>().colorExistsWarning.SetActive(false);
        if (((int)color.r + (int)color.g + (int)color.b) < 255)
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
        if (string.IsNullOrEmpty(r255Value) || !int.TryParse(r255Value, out int parsedValue) || parsedValue < 0 || parsedValue > 255)
        {
            if (!r255.GetComponent<TMP_InputField>().isFocused) UpdateTextsFromColor();
        }
        else
        {
            color.r = byte.Parse(r255Value);
            UpdateDisplayColor();
        }
    }

    public void UpdateGreen(string g255Value)
    {
        if (string.IsNullOrEmpty(g255Value) || !int.TryParse(g255Value, out int parsedValue) || parsedValue < 0 || parsedValue > 255)
        {
            if (!g255.GetComponent<TMP_InputField>().isFocused) UpdateTextsFromColor();
        }
        else
        {
            color.g = byte.Parse(g255Value);
            UpdateDisplayColor();
        }
    }
    public void UpdateBlue(string b255Value)
    {
        if (string.IsNullOrEmpty(b255Value) || !int.TryParse(b255Value, out int parsedValue) || parsedValue < 0 || parsedValue > 255)
        {
            if (!b255.GetComponent<TMP_InputField>().isFocused) UpdateTextsFromColor();
        }
        else
        {
            color.b = byte.Parse(b255Value);
            UpdateDisplayColor();
        }
    }
}
