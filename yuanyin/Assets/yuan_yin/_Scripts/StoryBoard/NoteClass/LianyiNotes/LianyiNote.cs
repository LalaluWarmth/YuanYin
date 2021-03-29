using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianyiNote : Note
{
    private NoteController _noteController;
    private LianyiSpawn _lianyiNoteSpawn;
    private float groupNO;
    private float direction;
    private int noteTime;
    private int delayTime;


    void Start()
    {
        _noteController = NoteController.GetInstance();
        _lianyiNoteSpawn = LianyiSpawn.GetInstance();
    }


    void FixedUpdate()
    {
        ClearWhenOutOfLane();
    }


    public void Initialized(float group, float dir, int nTime, int dTime) //初始化方法
    {
        groupNO = group;
        direction = dir;
        noteTime = nTime;
        delayTime = dTime;
    }

    protected override void ReturnToPool() //返回对象池
    {
        _lianyiNoteSpawn.ReturnNoteToPool(this);
        // Debug.Log("Lali-ho!");
    }

    public override void Onhit() //击中音符对象
    {
        // Debug.Log("OnClick");
        ReturnToPool();
        _lianyiNoteSpawn.GetHitEffect(transform.position);
    }

    protected override void ClearWhenOutOfLane()
    {
        if (_noteController.DelayedSampleTime - noteTime >= _noteController.hitMissRangeInSamples)
        {
            ReturnToPool();
            _lianyiNoteSpawn.GetFailEffect(transform.position);
        }
    }
}