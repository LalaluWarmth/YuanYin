using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLianyiNoteNode
{
    public InitLianyiNoteNode(float group, float dir, int nTime, int dTime)
    {
        Group = group;
        Dir = dir;
        NTime = nTime;
        DTime = dTime;
        IfExistHuan = false;
    }

    public InitLianyiNoteNode Previous;
    public InitLianyiNoteNode Next;
    public float Group { get; set; }
    public float Dir { get; set; }
    public int NTime { get; set; }
    public int DTime { get; set; }
    public bool IfExistHuan { get; set; }
}
