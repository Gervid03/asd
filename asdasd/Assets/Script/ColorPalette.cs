using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPalette : MonoBehaviour
{
    public string colorPalettePath; //the location of ColorPalette.txt
    public string[] colorPaletteLines; //strings consisting of 3 RGB floats separated by ' '
    public List<ColorDisplayButton> colors;
    public GameObject colorDisplay;
    public int rowLength;
    public float spacing;
    public GameObject colorSettings;
    public ColorDisplayButton selectedButton; //set by the clicked button
    public GameObject colorTweaker;
    public MapEditor mapEditor;
    public GameObject overwriteColorButton;
    public ColorDisplayButton colorUnderModification;
    public Image colorCarouselImage;
    public TMP_Text selectSMHWarning;

    void Start()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
        colorPalettePath = Application.dataPath + "/Saves/ColorPalette.txt";
        ReadInColors();
    }

    private void OnApplicationQuit()
    {
        SaveColors();
    }

    public void AdjustHeight()
    {
        this.gameObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(700, (Mathf.CeilToInt(colors.Count / 7f))*100);
    }

    public void CreateColor(Color szin)
    {
        GameObject c = Instantiate(colorDisplay, this.transform, false);

        c.GetComponent<Image>().color = szin;
        c.GetComponent<ColorDisplayButton>().color = szin;

        int a = mapEditor.AddColor(szin);
        c.GetComponent<ColorDisplayButton>().index = a;
        colors.Add(c.GetComponent<ColorDisplayButton>());

        AdjustHeight();
    }

    public void ReadInColors() //Read from ColorPalette.txt into colors list
    {
        colorPaletteLines = File.ReadAllLines(colorPalettePath);

        for (int i = 0; i < colorPaletteLines.Length; i++)
        {
            string[] stringRGB = colorPaletteLines[i].Trim().Split(' ');
            CreateColor(new Color(float.Parse(stringRGB[0]), float.Parse(stringRGB[1]), float.Parse(stringRGB[2]), 255));
        }
    }

    public void SaveColors() //save colors to ColorPalette.txt
    {
        StreamWriter fileColorPalette = new StreamWriter(colorPalettePath);
        for (int i = 0; i < colors.Count; i++)
        {
            fileColorPalette.WriteLine(colors[i].color.r.ToString() + ' ' + colors[i].color.g.ToString() + ' ' + colors[i].color.b.ToString());
        }
        fileColorPalette.Close();
    }

    public void AddNewColor()
    {
        colorTweaker.GetComponent<ColorTweaker>().BeActive();
        colorTweaker.GetComponent<ColorTweaker>().color = new Vector4(1, 1, 1, 1);
        colorTweaker.GetComponent<ColorTweaker>().UpdateTextsFromColor();
    }

    public void FinishedTweaking()
    {
        CreateColor(colorTweaker.GetComponent<ColorTweaker>().color);
        colorTweaker.GetComponent<ColorTweaker>().BeDeactive();
        overwriteColorButton.SetActive(false);
    }

    public void CancelTweaking()
    {
        colorTweaker.GetComponent<ColorTweaker>().BeDeactive();
        overwriteColorButton.SetActive(false);
    }

    public void ModifySelectedColor()
    {
        if (SelectWarning()) return;

        colorUnderModification = selectedButton;
        overwriteColorButton.SetActive(true);

        colorTweaker.GetComponent<ColorTweaker>().BeActive();
        colorTweaker.GetComponent<ColorTweaker>().color = selectedButton.GetComponent<Image>().color;
        colorTweaker.GetComponent<ColorTweaker>().UpdateTextsFromColor();

        UpdateColorCarousel();
    }

    public void OverwriteSelectedColor()
    {
        colors[colors.IndexOf(colorUnderModification)].color = colorTweaker.GetComponent<ColorTweaker>().color;
        colors[colors.IndexOf(colorUnderModification)].GetComponent<Image>().color = colorTweaker.GetComponent<ColorTweaker>().color;
        mapEditor.ModifyColor(colorUnderModification.index, colorTweaker.GetComponent<ColorTweaker>().color);
        
        colorTweaker.GetComponent<ColorTweaker>().BeDeactive();
        overwriteColorButton.SetActive(false);

        UpdateColorCarousel();
    }

    public bool SelectWarning()
    {
        if (selectedButton == null)
        {
            selectSMHWarning.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public void DeleteSelectedColor()
    {
        if (SelectWarning()) return;
        
        mapEditor.RemoveColor(selectedButton.index);
        colors.Remove(selectedButton);
        Destroy(selectedButton.gameObject);

        overwriteColorButton.SetActive(false);
        AdjustHeight();
    }

    public void ResetPalette()
    {
        colorPalettePath = Application.dataPath + "/Saves/DefaultColorPalette.txt";
        
        foreach (Transform child in this.transform)
        {
            mapEditor.RemoveColor(child.GetComponent<ColorDisplayButton>().index);
            GameObject.Destroy(child.gameObject);
            colors.Clear();
        }

        ReadInColors();
        colorPalettePath = Application.dataPath + "/Saves/ColorPalette.txt";
    }

    public void SelectedColorDecrement()
    {
        if (colors.IndexOf(selectedButton) - 1 < 0)
        {
            selectedButton = colors[colors.Count - 1];
        }
        else
        {
            selectedButton = colors[colors.IndexOf(selectedButton) - 1];
        }
        mapEditor.ChangeColor(selectedButton.index);

        UpdateColorCarousel();
    }

    public void SelectedColorIncrement()
    {
        if (colors.IndexOf(selectedButton) + 1 >= colors.Count)
        {
            selectedButton = colors[0];
        }
        else
        {
            selectedButton = colors[colors.IndexOf(selectedButton) + 1];
        }
        mapEditor.ChangeColor(selectedButton.index);

        UpdateColorCarousel();
    }

    public void UpdateColorCarousel()
    {
        colorCarouselImage.color = selectedButton.color;
        selectSMHWarning.gameObject.SetActive(false); //just here to abuse the frequent updates (:
    }
}