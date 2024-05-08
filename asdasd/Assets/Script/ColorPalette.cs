using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System;

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
    public ColorTweaker colorTweaker;
    public MapEditor mapEditor;
    public GameObject overwriteColorButton;
    public ColorDisplayButton colorUnderModification;
    public Image colorCarouselImage;
    public TMP_Text selectSMHWarning;
    public RectTransform colorPaletteParent;
    public GameObject defaultStateTogglePrefab;
    public Transform defaultStateToggleParent;
    public GameObject colorExistsWarning;
    public static event Action<int, Color32> modifyColor;
    public static event Action<int> deleteColor;

    void Awake()
    {
        mapEditor = FindFirstObjectByType<MapEditor>();
        mapEditor.tilemaps.makeItNotNull(new List<int>(), new List<Tilemap>(), new List<bool>());
        colorPalettePath = Application.dataPath + "/Saves/ColorPalette.txt";
        colorTweaker = FindFirstObjectByType<ColorTweaker>(FindObjectsInactive.Include);
        CreateColor(new Color32(255, 255, 255, 255));
        ReadInColors();
    }

    private void OnApplicationQuit()
    {
        SaveColors();
    }

    private void Update()
    {
        if (selectedButton == null && colors.Count > 0)
        {
            selectedButton = colors[0];
            UpdateColorCarousel();
        }
    }

    public void AdjustHeight()
    {
        colorPaletteParent.sizeDelta = new Vector2(700, (Mathf.CeilToInt(colors.Count / 7f)) * 100);
        FindFirstObjectByType<DefaultStateHeight>(FindObjectsInactive.Include).AdjustHeight((Mathf.CeilToInt(colors.Count / 7f)) * 100);
    }

    public bool ColorExists(Color32 c)
    {
        int i;
        for (i = 0; i < colors.Count; i++) if (SameColor(colors[i].color, c)) break;
        return i != colors.Count;
    }

    public void CreateColor(Color32 szin, int index = -1)
    {
        GameObject c = Instantiate(colorDisplay, colorPaletteParent, false);
        GameObject defaultStateToggle = Instantiate(defaultStateTogglePrefab, defaultStateToggleParent, false);

        c.GetComponent<Image>().color = szin;
        c.GetComponent<ColorDisplayButton>().color = szin;
        c.GetComponent<ColorDisplayButton>().toggle = defaultStateToggle;

        defaultStateToggle.GetComponent<SetDefaultState>().colorDisplay.color = szin;

        if (mapEditor == null)
        {
            mapEditor = FindFirstObjectByType<MapEditor>();
            Debug.Log(FindObjectsByType<MapEditor>(default).Length);
        }
        int a = mapEditor.AddColor(szin, index);
        c.GetComponent<ColorDisplayButton>().index = a;
        colors.Add(c.GetComponent<ColorDisplayButton>());

        defaultStateToggle.GetComponent<SetDefaultState>().colorIndex = a;

        AdjustHeight();
    }

    public void ReadInColors() //Read from ColorPalette.txt into colors list
    {
        colorPaletteLines = File.ReadAllLines(colorPalettePath);

        for (int i = 0; i < colorPaletteLines.Length; i++)
        {
            string[] stringRGB = colorPaletteLines[i].Trim().Split(' ');
            if (stringRGB[0] == "255" && stringRGB[1] == "255" && stringRGB[2] == "255") continue; //fehér már van
            CreateColor(new Color32(byte.Parse(stringRGB[0]), byte.Parse(stringRGB[1]), byte.Parse(stringRGB[2]), 255));
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
        colorExistsWarning.SetActive(false);
        colorTweaker.BeActive();
        colorTweaker.color = new Color32(255, 255, 255, 255);
        colorTweaker.UpdateTextsFromColor();
    }

    public void FinishedTweaking()
    {
        colorTweaker.AdjustBrightness();
        if (ColorExists(colorTweaker.color))
        {
            colorTweaker.brightnessWarning.SetActive(false);
            colorExistsWarning.SetActive(true);
            return;
        }
        CreateColor(colorTweaker.color);
        colorTweaker.BeDeactive();
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

        if (!SameColor(selectedButton.color, new Color32(255, 255, 255, 255)))
        {
            colorUnderModification = selectedButton;
            overwriteColorButton.SetActive(true);
        }

        colorTweaker.BeActive();
        colorTweaker.color = selectedButton.GetComponent<ColorDisplayButton>().color;
        colorTweaker.UpdateTextsFromColor();

        UpdateColorCarousel();
    }

    public void OverwriteSelectedColor()
    {
        if (SameColor(selectedButton.color, new Color32(255, 255, 255, 255))) return; //fehéret nem bántjuk!

        colorTweaker.AdjustBrightness();
        if (ColorExists(colorTweaker.color))
        {
            colorTweaker.brightnessWarning.SetActive(false);
            colorExistsWarning.SetActive(true);
            return;
        }

        ColorPalette.modifyColor?.Invoke(colorUnderModification.index, colorTweaker.color);

        mapEditor.ModifyColor(colorUnderModification.index, colorTweaker.color);
        
        colorTweaker.BeDeactive();
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
        if (SameColor(selectedButton.color, new Color32(255, 255, 255, 255))) return; //fehéret nem bántjuk!


        mapEditor.RemoveColor(selectedButton.index);
        colors.Remove(selectedButton);
        mapEditor.tilemaps.remove(selectedButton.index);

        ColorPalette.deleteColor?.Invoke(selectedButton.index);

        overwriteColorButton.SetActive(false);
        selectedButton = colors[0];
        AdjustHeight();
    }

    public void ResetPalette()
    {
        colorPalettePath = Application.dataPath + "/Saves/DefaultColorPalette.txt";

        KillAllTheChildren();

        ReadInColors();
        colorPalettePath = Application.dataPath + "/Saves/ColorPalette.txt";
        selectedButton = null;
    }

    public void KillAllTheChildren()
    {
        foreach (Transform child in colorPaletteParent)
        {
            if (child.GetComponent<ColorDisplayButton>().index == 0) continue;
            Destroy(child.GetComponent<ColorDisplayButton>().toggle);
            mapEditor.RemoveColor(child.GetComponent<ColorDisplayButton>().index);
            mapEditor.tilemaps.remove(child.GetComponent<ColorDisplayButton>().index);
            GameObject.Destroy(child.gameObject);
        }
        ColorDisplayButton temp = colors[0];
        colors.Clear();
        colors.Add(temp);
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
        colorExistsWarning.SetActive(false);
    }

    public bool SameColor(Color32 a, Color32 b)
    {
        return (a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a);
    }
}