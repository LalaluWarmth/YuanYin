using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TextTok
{
    public char seg = ',';
    private List<float> positionData = new List<float>();

    public List<float> TokText(string str)
    {
        string[] substring = str.Split(seg);
        foreach (var item in substring)
        {
            positionData.Add(float.Parse(item));
        }

        return positionData;
    }
}