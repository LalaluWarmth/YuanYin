using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLianyiList
{
    public InitLianyiList()
    {
        ListCountValue = 0;
        Head = null;
        Tail = null;
    }

    private InitLianyiNoteNode Head;
    private InitLianyiNoteNode Tail;
    private InitLianyiNoteNode Current;
    private int ListCountValue;

    public void Append(float group, float dir, int nTime, int dTime)
    {
        InitLianyiNoteNode newNode = new InitLianyiNoteNode(group, dir, nTime, dTime);
        if (IsNull())
        {
            Head = newNode;
            Tail = newNode;
        }
        else
        {
            Tail.Next = newNode;
            newNode.Previous = Tail;
            Tail = newNode;
        }

        Current = newNode;
        ListCountValue++;
    }

    public void MoveNext() //向后移动一个数据
    {
        if (!IsEof()) Current = Current.Next;
    }


    public void MovePrevious() //向前移动一个数据
    {
        if (!IsBof()) Current = Current.Previous;
    }


    public void MoveFrist() //移动到第一个数据
    {
        Current = Head;
    }


    public void MoveLast() //移动到最后一个数据
    {
        Current = Tail;
    }

    public bool IsNull() //判断链表是否为空
    {
        if (ListCountValue == 0)
            return true;
        return false;
    }

    public bool IsEof() //判断是否为到达尾部
    {
        if (Current == Tail)
            return true;
        return false;
    }

    public bool IsBof() //判断是否为到达头部
    {
        if (Current == Head)
            return true;
        return false;
    }

    public InitLianyiNoteNode GetCurrent() //获取当前节点
    {
        return Current;
    }

    public InitLianyiNoteNode GetHead()//获取头结点
    {
        return Head;
    }

    public InitLianyiNoteNode GetTail()//获取尾结点
    {
        return Tail;
    }

    public int ListCount //获取链表节点个数
    {
        get { return ListCountValue; }
    }
}