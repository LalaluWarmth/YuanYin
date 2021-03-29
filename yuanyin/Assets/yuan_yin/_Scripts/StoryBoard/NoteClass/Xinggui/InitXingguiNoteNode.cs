using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitXingguiNoteNode
{
    public InitXingguiNoteNode(float group, Vector2 tPosition, int nTime, int dTime)
    {
        Group = group;
        TPosition = tPosition;
        NTime = nTime;
        DTime = dTime;
        IfExistLine = false;
    }

    public InitXingguiNoteNode Previous;
    public InitXingguiNoteNode Next;
    public float Group { get; set; }
    public Vector2 TPosition { get; set; }
    public int NTime { get; set; }
    public int DTime { get; set; }
    public bool IfExistLine { get; set; }
}
