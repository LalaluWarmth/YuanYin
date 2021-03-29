using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEditor.Animations;
using UnityEngine;

public class XingguiLineSpawn : MonoBehaviour
{
    private static XingguiLineSpawn _instance;

    public static XingguiLineSpawn GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(XingguiLineSpawn)) as XingguiLineSpawn;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(XingguiLineSpawn)) as XingguiLineSpawn;
            }
        }

        return _instance;
    }

    [Tooltip("事件对应ID")] [EventID] public string eventID;

    //-------------------------引用--------------------------
    public Koreography playingKoreo;

    private NoteController _noteController;

    private XingguiSpawn _xingguiSpawn;
    //-------------------------对象池--------------------------

    Stack<GameObject> linePool = new Stack<GameObject>();

    //-------------------------Line--------------------------
    public GameObject LineObject;
    public GameObject SampleLineObject;

    public Transform storyboard;

    private void Awake()
    {
        _noteController = NoteController.GetInstance();
        _xingguiSpawn = XingguiSpawn.GetInstance();
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
    }

    void Start()
    {
        ReturnLineToPool(SampleLineObject);
    }


    void FixedUpdate()
    {
        CheckLineSpawnNext();
    }

    private void CheckLineSpawnNext()
    {
        int currentTime = _noteController.DelayedSampleTime;
        for (int i = 0; i < _xingguiSpawn.runningLists.Count; i++)
        {
            if (_xingguiSpawn.runningLists[i].GetHead().DTime <= currentTime &&
                !_xingguiSpawn.runningLists[i].GetHead().IfExistLine)
            {
                GameObject newObj = GetFreshLine();
                _xingguiSpawn.runningLists[i].GetHead().IfExistLine = true;
                newObj.transform.position = Vector3.zero;
            }
        }
    }

    //-------------------------line对象池方法--------------------------
    public GameObject GetFreshLine()
    {
        GameObject retObj;
        if (linePool.Count > 0)
        {
            retObj = linePool.Pop();
        }
        else
        {
            retObj = Instantiate<GameObject>(LineObject, storyboard);
        }

        retObj.SetActive(true);
        Debug.Log("line!");
        return retObj;
    }

    public void ReturnLineToPool(GameObject obj)
    {
        if (obj)
        {
            obj.SetActive(false);
            linePool.Push(obj);
            // Debug.Log("Lali-ho!");
        }
    }
}