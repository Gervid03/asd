using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        mapEditor = FindAnyObjectByType<MapEditor>();
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
        colorPalettePath = Application.dataPath + "/Saves/ColorPalette.txt";
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
    }

    public void CancelTweaking()
    {
        colorTweaker.GetComponent<ColorTweaker>().BeDeactive();
    }

    public void ModifySelectedColor()
    {
        Debug.Log("asd");
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
        
        mapEditor.RemoveColor(selectedButton.index);
        colors.Remove(selectedButton);
        Destroy(selectedButton.gameObject);

        AdjustHeight();
    }
}
