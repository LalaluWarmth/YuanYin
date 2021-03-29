using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class ClickNoteSpawn : NoteSpawn<ClickNoteSpawn, ClickNote>
{
    [Tooltip("事件对应ID")] [EventID] public string moveDataID;

    public Transform bgEdge;


    public RectTransform clickNoteTargetCenter;
    public RectTransform clickNoteTargetRuler;


    void Start()
    {
        ReturnHitEffectToPool(SampleHitEffectGo);
        ReturnFailEffectToPool(SampleFailEffectGo);
        ReturnNoteToPool(SampleNoteObject);
        InitNoteQueue();
    }


    void FixedUpdate()
    {
        CheckNoteSpawnNext(); //检测新音符的产生
    }

    private List<InitClickNote> tempClickNotesDatas = new List<InitClickNote>();
    private Queue<InitClickNote> initClickNotesDatas = new Queue<InitClickNote>();
    private float _clickNoteDistance;

    protected override void InitNoteQueue()
    {
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件
        KoreographyTrackBase moveDataTrack = playingKoreo.GetTrackByID(moveDataID); //获取body移动
        List<KoreographyEvent> moveDataEvents = moveDataTrack.GetAllEvents(); //获取所有body移动事件
        foreach (var item in rawEvents)
        {
            string rawText = item.GetTextValue();
            List<float> positionData = new TextTok().TokText(rawText);
            int dataIndex = 0;
            while (dataIndex < positionData.Count)
            {
                InitClickNote temp = new InitClickNote();
                float rad = positionData[dataIndex] * Mathf.Deg2Rad;
                temp.Direction = rad;
                Vector2 startPos = CalculateClickStartPos(rad);
                Vector2 offset = Vector2.zero;
                foreach (var moveData in moveDataEvents)
                {
                    if (item.StartSample > moveData.StartSample)
                    {
                        string moveText = moveData.GetTextValue();
                        List<float> moveDatas = new TextTok().TokText(moveText);
                        offset = new Vector2(moveDatas[0], moveDatas[1]);
                    }
                }

                temp.TPosition = CalculateClickTargetPos(rad, offset);
                temp.NTime = item.StartSample;
                temp.DTime = item.StartSample -
                             GetClickNoteSpawnSampleOffset(startPos, temp.TPosition);
                tempClickNotesDatas.Add(temp);
                dataIndex++;

                // Debug.Log(temp.Direction + "+" + temp.TPosition + "+" +
                //           GetClickNoteSpawnSampleOffset(startPos, temp.TPosition));
            }
        }

        tempClickNotesDatas.Sort();

        for (int i = 0; i < tempClickNotesDatas.Count; i++)
        {
            initClickNotesDatas.Enqueue(tempClickNotesDatas[i]);
        }

        tempClickNotesDatas.Clear();
        System.GC.Collect();
    }

    private Vector2 CalculateClickStartPos(float degree)
    {
        Vector2 pos = Vector2.zero;
        float screenP = (float) Screen.currentResolution.width / (float) Screen.currentResolution.height;
        // Debug.Log(screenP);
        float bgHeight = bgEdge.position.y;
        if (degree >= 315 * Mathf.Deg2Rad || degree < 45 * Mathf.Deg2Rad)
        {
            pos.x = bgHeight * screenP + 1;
            pos.y = pos.x * Mathf.Tan(degree);
        }

        if (degree >= 45 * Mathf.Deg2Rad && degree < 135 * Mathf.Deg2Rad &&
            !Mathf.Approximately(degree, 90 * Mathf.Deg2Rad))
        {
            pos.y = bgHeight + 1;

            pos.x = pos.y / Mathf.Tan(degree);
        }

        if (Mathf.Approximately(degree, 90 * Mathf.Deg2Rad))
        {
            pos.y = bgHeight + 1;
            pos.x = 0;
        }

        if (degree >= 135 * Mathf.Deg2Rad && degree < 225 * Mathf.Deg2Rad)
        {
            pos.x = -(bgHeight * screenP + 1);
            pos.y = pos.x * Mathf.Tan(degree);
        }

        if (degree >= 225 * Mathf.Deg2Rad && degree < 315 * Mathf.Deg2Rad &&
            !Mathf.Approximately(degree, 270 * Mathf.Deg2Rad))
        {
            pos.y = -bgHeight - 1;
            pos.x = pos.y / Mathf.Tan(degree);
        }

        if (Mathf.Approximately(degree, 270 * Mathf.Deg2Rad))
        {
            pos.y = -bgHeight - 1;
            pos.x = 0;
        }

        return pos;
    }


    private Vector2 CalculateClickTargetPos(float degree, Vector2 offset)
    {
        Vector2 unitPos = clickNoteTargetRuler.position - clickNoteTargetCenter.position;
        Vector2 pos;
        pos.x = unitPos.x * Mathf.Cos(degree) - unitPos.y * Mathf.Sin(degree);
        pos.y = unitPos.x * Mathf.Sin(degree) + unitPos.y * Mathf.Cos(degree);
        return pos + offset;
    }

    private int GetClickNoteSpawnSampleOffset(Vector2 curClickNoteStartPosition,
        Vector2 curClickNoteTargetPosition) //确定在乐谱上产生音符的对应采样点的偏移量
    {
        float spawnDistToTarget =
            Vector2.Distance(curClickNoteStartPosition, curClickNoteTargetPosition); //计算生成为位置和结束位置的距离
        float spawnPosToTargetTime = spawnDistToTarget / _noteController.noteSpeed; //计算出音符到达结束位置所需的时间
        return (int) (spawnPosToTargetTime * _noteController.SampleRate); //时间*采样率=采样点偏移量
    }

    protected override void CheckNoteSpawnNext()
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= initClickNotesDatas.Count && initClickNotesDatas.Peek().DTime <= currentTime)
        {
            InitClickNote tempData = initClickNotesDatas.Dequeue();
            // Debug.Log("fucku//" + tempData.dTime);
            // Debug.Log("TimeToGo!");
            ClickNote newObj = GetFreshNote();
            newObj.Initialized(tempData.TPosition, tempData.Direction,
                tempData.NTime, tempData.DTime);
            curNum++;
        }
    }

    protected override ClickNote GetFreshNote() //从对象池中取对象的方法
    {
        ClickNote retObj;
        if (notePool.Count > 0)
        {
            retObj = notePool.Pop();
        }
        else
        {
            retObj = Instantiate<ClickNote>(NoteObject, storyboard);
        }

        retObj.transform.position = Vector2.one * 200;
        retObj.gameObject.SetActive(true);
        retObj.enabled = true;
        return retObj;
    }
}