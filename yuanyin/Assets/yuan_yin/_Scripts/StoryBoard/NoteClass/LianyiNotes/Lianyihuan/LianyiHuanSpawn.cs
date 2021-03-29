using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class LianyiHuanSpawn : MonoBehaviour
{
    private static LianyiHuanSpawn _instance;

    public static LianyiHuanSpawn GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(LianyiHuanSpawn)) as LianyiHuanSpawn;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(LianyiHuanSpawn)) as LianyiHuanSpawn;
            }
        }

        return _instance;
    }

    [Tooltip("事件对应ID")] [EventID] public string eventID;

    //-------------------------引用--------------------------
    public Koreography playingKoreo;

    private NoteController _noteController;

    private LianyiSpawn _lianyiSpawn;
    //-------------------------对象池--------------------------

    Stack<GameObject> huanRendererPool = new Stack<GameObject>();
    Stack<GameObject> huanMeasurePool = new Stack<GameObject>();

    //-------------------------Huan--------------------------
    public GameObject HuanRendererObject;
    public GameObject SampleHuanRendererObject;
    public GameObject HuanMeasureObject;
    public GameObject SampleHuanMeasureObject;

    public Transform storyboard;

    
    //-------------------------HuanRecord--------------------------
    public Dictionary<float,float> huanMeasureRecord=new Dictionary<float, float>();
    
    private void Awake()
    {
        _noteController = NoteController.GetInstance();
        _lianyiSpawn = LianyiSpawn.GetInstance();
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
    }

    void Start()
    {
        ReturnHuanMeasureToPool(SampleHuanMeasureObject);
        ReturnHuanRendererToPool(SampleHuanRendererObject);
        InitHuanQueue();
    }


    void FixedUpdate()
    {
        CheckHuanMeasureSpawnNext();
        CheckHuanRendererSpawnNext();
    }

    Queue<initLianyiHuanData> initHuanDatas = new Queue<initLianyiHuanData>();
    public Animator lianyiAppearAnimator;

    private void InitHuanQueue()
    {
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件
        foreach (var item in rawEvents)
        {
            string rawText = item.GetTextValue();
            List<float> positionData = new TextTok().TokText(rawText);
            int dataIndex = 0;
            while (dataIndex < positionData.Count)
            {
                initLianyiHuanData temp = new initLianyiHuanData();
                temp.NO = positionData[dataIndex];
                temp.NTime = item.StartSample;
                temp.DTime = item.StartSample - GetLianyiNoteOffset();
                initHuanDatas.Enqueue(temp);
                dataIndex++;
            }
        }

        System.GC.Collect();
    }

    private int GetLianyiNoteOffset() //确定在乐谱上产生音符的对应采样点的偏移量
    {
        AnimationClip[] animationClips = lianyiAppearAnimator.runtimeAnimatorController.animationClips;
        // Debug.Log(animationClips[0].length);
        return (int) (animationClips[0].length * _noteController.SampleRate); //时间*采样率=采样点偏移量
    }

    Queue<initLianyiHuanData> backupInitLianyiHuanDatas = new Queue<initLianyiHuanData>();

    private void CheckHuanMeasureSpawnNext()
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= initHuanDatas.Count && initHuanDatas.Peek().DTime <= currentTime)
        {
            initLianyiHuanData tempData = initHuanDatas.Dequeue();
            GameObject newObj=GetFreshHuanMeasure();
            newObj.GetComponent<LianyiMeasure>().Initialized(tempData.NO);
            backupInitLianyiHuanDatas.Enqueue(tempData);

            curNum++;
        }
    }

    private void CheckHuanRendererSpawnNext()
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= backupInitLianyiHuanDatas.Count && backupInitLianyiHuanDatas.Peek().NTime <= currentTime)
        {
            initLianyiHuanData tempData = backupInitLianyiHuanDatas.Dequeue();
            GetFreshHuanRenderer();
            curNum++;
        }
    }

    //-------------------------HuanRenderer对象池方法--------------------------
    public GameObject GetFreshHuanRenderer()
    {
        GameObject retObj;
        if (huanRendererPool.Count > 0)
        {
            retObj = huanRendererPool.Pop();
        }
        else
        {
            retObj = Instantiate<GameObject>(HuanRendererObject, storyboard);
        }

        retObj.SetActive(true);
        return retObj;
    }

    public void ReturnHuanRendererToPool(GameObject obj)
    {
        if (obj)
        {
            obj.SetActive(false);
            huanRendererPool.Push(obj);
        }
    }

    //-------------------------HuanMeasure对象池方法--------------------------
    public GameObject GetFreshHuanMeasure()
    {
        GameObject retObj;
        if (huanMeasurePool.Count > 0)
        {
            retObj = huanMeasurePool.Pop();
        }
        else
        {
            retObj = Instantiate<GameObject>(HuanMeasureObject, storyboard);
        }

        retObj.SetActive(true);
        return retObj;
    }

    public void ReturnHuanMeasureToPool(GameObject obj)
    {
        if (obj)
        {
            obj.SetActive(false);
            huanMeasurePool.Push(obj);
        }
    }
}