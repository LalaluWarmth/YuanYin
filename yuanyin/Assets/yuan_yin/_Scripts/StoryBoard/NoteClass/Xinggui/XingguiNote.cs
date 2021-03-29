using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XingguiNote : Note
{
    private NoteController _noteController;
    private XingguiSpawn _xingguiNoteSpawn;
    private float groupNO;
    private Vector2 targetPosition;
    private int noteTime;
    private int delayTime;


    void Start()
    {
        _noteController = NoteController.GetInstance();
        _xingguiNoteSpawn = XingguiSpawn.GetInstance();
    }


    void FixedUpdate()
    {
        ClearWhenOutOfLane();
        transform.position = targetPosition;
    }


    public void Initialized(float group, Vector2 tPosition, int nTime, int dTime) //初始化方法
    {
        groupNO = group;
        targetPosition = tPosition;
        noteTime = nTime;
        delayTime = dTime;
    }

    protected override void ReturnToPool() //返回对象池
    {
        _xingguiNoteSpawn.ReturnNoteToPool(this);
        // Debug.Log("Lali-ho!");
    }

    public override void Onhit() //击中音符对象
    {
        // Debug.Log("OnClick");
        ReturnToPool();
        _xingguiNoteSpawn.GetHitEffect(transform.position);
    }

    protected override void ClearWhenOutOfLane()
    {
        if (_noteController.DelayedSampleTime - noteTime >= _noteController.hitMissRangeInSamples)
        {
            ReturnToPool();
            _xingguiNoteSpawn.GetFailEffect(transform.position);
        }
    }
}