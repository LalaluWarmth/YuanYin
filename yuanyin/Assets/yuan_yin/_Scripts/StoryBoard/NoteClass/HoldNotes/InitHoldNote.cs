using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitHoldNote : IComparable<InitHoldNote>//todo 需求有变，暂时禁用
{
    public float Group { get; set; }
    public float Direction { get; set; }
    public Vector2 TPosition { get; set; }
    public int NTime0 { get; set; }
    public int DTime0 { get; set; }
    public int NTime1 { get; set; }
    public int DTime1 { get; set; }

    public int CompareTo(InitHoldNote obj)
    {
        return this.DTime0.CompareTo(obj.DTime0);
    }
}