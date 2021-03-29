using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : MonoBehaviour//todo 需求有变，暂时禁用
{
    public RectTransform head;
    public RectTransform tail;

    private NoteController _noteController;
    private HoldNoteSpawn _holdNoteSpawn;
    private float _group;
    private Vector2 _tPosition;
    private float _direction;
    private int _nTime0;
    private int _dTime0;
    private int _nTime1;
    private int _dTime1;
    private Vector3 rot;

    void Start()
    {
        _noteController = NoteController.GetInstance();
        _holdNoteSpawn = HoldNoteSpawn.GetInstance();
    }


    void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 pos0 = _tPosition;
        float distance0 = (_noteController.DelayedSampleTime - _nTime0) / (float) _noteController.SampleRate *
                          _noteController.noteSpeed;
        pos0.x -= distance0 * Mathf.Cos(_direction);
        pos0.y -= distance0 * Mathf.Sin(_direction);
        head.position = pos0;


        Vector2 pos1 = _tPosition;
        float distance1 = (_noteController.DelayedSampleTime - _nTime1) / (float) _noteController.SampleRate *
                          _noteController.noteSpeed;
        pos1.x -= distance1 * Mathf.Cos(_direction);
        pos1.y -= distance1 * Mathf.Sin(_direction);
        tail.position = pos1;
        head.eulerAngles = rot;
        tail.eulerAngles = rot;
    }

    public void Initialized(float group, Vector2 tPosition, float dir, int nTime0, int dTime0, int nTime1,
        int dTime1) //初始化方法
    {
        _group = group;
        _tPosition = tPosition;
        _direction = dir;
        _nTime0 = nTime0;
        _dTime0 = dTime0;
        _nTime1 = nTime1;
        _dTime1 = dTime1;
        rot = new Vector3(0, 0, dir * Mathf.Rad2Deg);
    }

    private void ReturnToPool() //返回对象池
    {
        _holdNoteSpawn.ReturnHoldNoteToPool(this);
    }

    public void Onhit() //击中音符对象
    {
        // Debug.Log("OnClick");
        ReturnToPool();
        // _holdNoteSpawn.GetHitEffect(transform.position);
    }

    //
    // private void ClearWhenOutOfLane()
    // {
    //     if (_noteController.DelayedSampleTime - noteTime >= _noteController.hitMissRangeInSamples)
    //     {
    //         ReturnToPool();
    //         _holdNoteSpawn.GetFailEffect(transform.position);
    //     }
    // }
}