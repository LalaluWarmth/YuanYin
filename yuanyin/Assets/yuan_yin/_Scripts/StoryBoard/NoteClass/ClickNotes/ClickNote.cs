using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class ClickNote : Note
{
    [SerializeField] private RectTransform selfTransform;

    private NoteController _noteController;
    private ClickNoteSpawn _clickNoteSpawn;
    private Vector2 targetPosition;
    private float direction;
    private int noteTime;
    private int delayTime;
    

    void Start()
    {
        selfTransform = GetComponent<RectTransform>();
        _noteController = NoteController.GetInstance();
        _clickNoteSpawn = ClickNoteSpawn.GetInstance();
    }


    void FixedUpdate()
    {
        UpdatePosition();
        ClearWhenOutOfLane();
    }


    public void Initialized(Vector2 tPosition, float dir,
        int nTime, int dTime) //初始化方法
    {
        direction = dir;
        targetPosition = tPosition;
        noteTime = nTime;
        delayTime = dTime;
    }


    private void UpdatePosition()
    {
        Vector2 pos = targetPosition;
        float distance = (_noteController.DelayedSampleTime - noteTime) / (float) _noteController.SampleRate *
                         _noteController.noteSpeed;
        pos.x -= distance * Mathf.Cos(direction);
        pos.y -= distance * Mathf.Sin(direction);
        selfTransform.position = pos;
    }
    

    protected override void ReturnToPool() //返回对象池
    {
        _clickNoteSpawn.ReturnNoteToPool(this);
        // Debug.Log("Lali-ho!");
    }

    public override void Onhit() //击中音符对象
    {
        // Debug.Log("OnClick");
        ReturnToPool();
        _clickNoteSpawn.GetHitEffect(transform.position);
    }

    protected override void ClearWhenOutOfLane()
    {
        if (_noteController.DelayedSampleTime - noteTime >= _noteController.hitMissRangeInSamples)
        {
            ReturnToPool();
            _clickNoteSpawn.GetFailEffect(transform.position);
        }
    }
}