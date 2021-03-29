using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitClickNote : IComparable<InitClickNote>
{
    public float Direction { get; set; }
    public Vector2 TPosition { get; set; }
    public int NTime { get; set; }
    public int DTime { get; set; }

    public int CompareTo(InitClickNote obj)
    {
        return this.DTime.CompareTo(obj.DTime);
    }
}