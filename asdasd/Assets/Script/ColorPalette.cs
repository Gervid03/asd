using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    public string colorPalettePath; //the location of ColorPalette.txt
    public string[] colorPaletteLines; //strings consisting of 3 RGB floats separated by ' '
    public List<Color> colors;

    void Start()
    {
        ReadInColors();
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
}
