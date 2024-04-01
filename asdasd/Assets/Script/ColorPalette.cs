using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ColorPalette : MonoBehaviour
{
    public string colorPalettePath; //the location of ColorPalette.txt
    public string[] colorPaletteLines; //strings consisting of 3 RGB floats separated by ' '
    public List<Color> colors;
    public GameObject colorDisplay;
    public int rowLength;
    public float spacing;
    public List<GameObject> colorDisplayButtons;
    public GameObject colorSettings;
    public GameObject selectedButton; //set by the clicked button
    public GameObject colorTweaker;

    void Start()
    {
        ReadInColors();
        UpdateColorPaletteGrid();
    }

    private void OnApplicationQuit()
    {
        SaveColors();
    }

    public void ReadInColors() //Read from ColorPalette.txt into colors list
    {
        colorPalettePath = Application.dataPath + "/Saves/ColorPalette.txt";
        colorPaletteLines = File.ReadAllLines(colorPalettePath);

        for (int i = 0; i < colorPaletteLines.Length; i++)
        {
            string[] stringRGB = colorPaletteLines[i].Trim().Split(' ');
            colors.Add(new Color(float.Parse(stringRGB[0]), float.Parse(stringRGB[1]), float.Parse(stringRGB[2]), 255)); 
        }
    }

    public void SaveColors() //save colors to ColorPalette.txt
    {
        StreamWriter fileColorPalette = new StreamWriter(colorPalettePath);
        for (int i = 0; i < colors.Count; i++)
        {
            fileColorPalette.WriteLine(colors[i][0].ToString() + ' ' + colors[i][1].ToString() + ' ' + colors[i][2].ToString());
        }
        fileColorPalette.Close();
    }

    public void UpdateColorPaletteGrid()
    {
        foreach (Transform child in this.transform) //destroy every child (:<
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < colors.Count; i++)
        {
            colorDisplayButtons.Add(Instantiate(colorDisplay, this.transform, false));
            colorDisplayButtons[colorDisplayButtons.Count - 1].transform.SetParent(this.transform);
            colorDisplayButtons[colorDisplayButtons.Count - 1].GetComponent<Image>().color = colors[i];
            colorDisplayButtons[colorDisplayButtons.Count - 1].transform.localPosition = new Vector2((i%rowLength)*spacing, -((i/rowLength)*spacing));

            int rows = colors.Count / rowLength;
            if (colors.Count % rowLength != 0) rows++;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(700, rows * spacing);
        }

    }

    public void AddNewColor()
    {
        colorTweaker.GetComponent<ColorTweaker>().BeActive();
        colorTweaker.GetComponent<ColorTweaker>().color = new Vector4(0, 0, 0, 1);
        colorTweaker.GetComponent<ColorTweaker>().UpdateTextsFromColor();
    }

    public void FinishedTweaking()
    {
        colors.Add(colorTweaker.GetComponent<ColorTweaker>().color);
        UpdateColorPaletteGrid();
        colorTweaker.GetComponent<ColorTweaker>().BeDeactive();
    }

    public void CancelTweaking()
    {
        colorTweaker.GetComponent<ColorTweaker>().BeDeactive();
    }

    public void ModifySelectedColor()
    {
        if (SelectWarning()) return;

        colorTweaker.GetComponent<ColorTweaker>().BeActive();
        colorTweaker.GetComponent<ColorTweaker>().color = selectedButton.GetComponent<Image>().color;
        colorTweaker.GetComponent<ColorTweaker>().UpdateTextsFromColor();
    }

    public bool SelectWarning()
    {
        if (selectedButton == null)
        {
            Debug.Log("SELECT A COLOR!!!");
            //throw maybe something nicer at user
            return true;
        }
        return false;
    }

    public void DeleteSelectedColor()
    {
        if (SelectWarning()) return;

        colors.Remove(selectedButton.GetComponent<Image>().color);
        Destroy(selectedButton);
        UpdateColorPaletteGrid();
    }
}
