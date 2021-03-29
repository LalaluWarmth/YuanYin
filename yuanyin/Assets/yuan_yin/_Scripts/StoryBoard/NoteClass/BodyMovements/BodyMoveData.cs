using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;

public class BodyMoveData : MonoBehaviour
{
    private static BodyMoveData _instance;

    public static BodyMoveData GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(BodyMoveData)) as BodyMoveData;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(BodyMoveData)) as BodyMoveData;
            }
        }

        return _instance;
    }

    [Tooltip("事件对应ID")] [EventID] public string eventID;

    //-------------------------引用--------------------------
    public Koreography playingKoreo;
    public Transform storyboard;
    private NoteController _noteController;

    private void Awake()
    {
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
        _noteController = NoteController.GetInstance();
    }

    void Start()
    {
        InitBodyMoveQueue();
    }

    void FixedUpdate()
    {
        CheckNext();
    }

    private Queue<InitBodyMoveData> initBodyMoveDatas = new Queue<InitBodyMoveData>();

    private void InitBodyMoveQueue()
    {
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件
        foreach (var item in rawEvents)
        {
            string rawText = item.GetTextValue();
            List<float> datas = new TextTok().TokText(rawText);
            InitBodyMoveData temp = new InitBodyMoveData();
            temp.Position = new Vector2(datas[0], datas[1]);
            temp.Scale = new Vector2(datas[2], datas[2]);
            temp.NTime = item.StartSample;
            float translateTime = datas[3];
            temp.TranslateTime = translateTime;
            temp.DTime = item.StartSample - GetMoveActionOffset(translateTime);
            initBodyMoveDatas.Enqueue(temp);
            // Debug.Log(temp.Position + "//" + temp.Scale + "//" + temp.TranslateTime);
        }

        System.GC.Collect();
    }

    private int GetMoveActionOffset(float time)
    {
        return (int) time * _noteController.SampleRate;
    }

    private void CheckNext()
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= initBodyMoveDatas.Count && initBodyMoveDatas.Peek().DTime <= currentTime)
        {
            // Debug.Log("FuckU");
            InitBodyMoveData tempData = initBodyMoveDatas.Dequeue();
            TranslateBody(tempData);
            curNum++;
        }
    }

    private void TranslateBody(InitBodyMoveData temp)
    {
        storyboard.DOMove(temp.Position, temp.TranslateTime).SetEase(Ease.InOutSine);
    }
}