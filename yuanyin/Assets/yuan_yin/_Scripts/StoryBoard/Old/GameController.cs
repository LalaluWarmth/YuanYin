using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class GameController : MonoBehaviour
{
    [Tooltip("事件对应ID")] [EventID] public string eventID;

    [Tooltip("速度")] public float noteSpeed = 1;

    [Tooltip("命中范围")] [Range(8f, 300f)] public float hitWindowRangeInMS; //毫秒计的命中范围


    public float WindowSizeInUnits //命中窗口大小
    {
        get
        {
            return noteSpeed * (hitWindowRangeInMS * 0.001f); //音符速度*毫秒计范围=可视的判定范围的距离
        }
    }

    private int hitWindowRangeInSamples; //在样本的命中窗口

    public int HitWindowSampleWidth //在样本的命中窗口公开
    {
        get { return hitWindowRangeInSamples; }
    }

    public int SampleRate //采样率
    {
        get { return playingKoreo.SampleRate; }
    }

    //-------------------------引用--------------------------
    private Koreography playingKoreo;
    public AudioSource audioCom;
    public List<LaneController> noteLanes = new List<LaneController>();

    //-------------------------对象池--------------------------

    Stack<NoteObject> noteObjectPool = new Stack<NoteObject>(); //音符
    public Stack<GameObject> downEffectObjectPool = new Stack<GameObject>(); //特效
    public Stack<GameObject> hitEffectObjectPool = new Stack<GameObject>(); //特效
    public Stack<GameObject> hitLongEffectObjectPool = new Stack<GameObject>(); //特效

    //-------------------------音符--------------------------

    public NoteObject noteObject;
    public GameObject downEffectGo;
    public GameObject hitEffectGo;
    public GameObject hitLongEffectGo;

    //-------------------------引导时间--------------------------
    [Tooltip("开始播放音频之前提供的时间量")] public float leadInTime;

    private float leadInTimeLeft; //音频播放之前的剩余时间量
    private float timeLeftToPlay; //音乐开始之前的倒计时器

    public int DelayedSampleTime
    {
        get
        {
            return playingKoreo.GetLatestSampleTime() -
                   (int) (SampleRate * leadInTimeLeft); //乐谱上的采样点时刻-提前量对应采样点个数=>延迟调用
        }
    }

    void Start()
    {
        InitializeLeadIn();
        for (int i = 0; i < noteLanes.Count; i++)
        {
            noteLanes[i].Initialized(this); //每条轨道调用初始化方法
        }

        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件

        for (int i = 0; i < rawEvents.Count; i++) //遍历所有音轨
        {
            KoreographyEvent evt = rawEvents[i];
            int noteID = evt.GetIntValue(); //获取乐谱中事件的数值
            for (int j = 0; j < noteLanes.Count; j++)
            {
                LaneController lane = noteLanes[j];
                // if (noteID > 6)
                // {
                //     noteID -= 6;
                //     if (noteID > 6)
                //     {
                //         noteID -= 6;
                //     }
                // }

                if (lane.DoesMatch(noteID))
                {
                    lane.AddEventToLane(evt);
                    break;
                }
            }
        }

        hitWindowRangeInSamples = (int) (SampleRate * hitWindowRangeInMS * 0.001f); //乐谱中的击中范围计算，单位是采样点
    }


    void Update()
    {
        if (timeLeftToPlay > 0)
        {
            timeLeftToPlay = Mathf.Max(timeLeftToPlay - Time.unscaledDeltaTime, 0);
            // Debug.Log(timeLeftToPlay);
            if (timeLeftToPlay <= 0)
            {
                audioCom.Play();
            }
        }

        //倒数引导时间
        if (leadInTimeLeft > 0)
        {
            leadInTimeLeft = Mathf.Max(leadInTimeLeft - Time.unscaledDeltaTime, 0);
            // Debug.Log(leadInTimeLeft);
        }
    }

    private void InitializeLeadIn() //初始化引导时间并控制播放
    {
        if (leadInTime > 0)
        {
            leadInTimeLeft = leadInTime;
            timeLeftToPlay = leadInTime;
        }
        else
        {
            audioCom.Play();
        }
    }

    //-------------------------音符对象池方法--------------------------
    public NoteObject GetFreshNoteObject() //从对象池中取对象的方法
    {
        NoteObject retObj;
        if (noteObjectPool.Count > 0)
        {
            retObj = noteObjectPool.Pop();
        }
        else
        {
            retObj = Instantiate<NoteObject>(noteObject);
        }

        retObj.transform.position = Vector3.one * 200;
        retObj.gameObject.SetActive(true);
        retObj.enabled = true;
        return retObj;
    }

    public void ReturnNoteObjectToPool(NoteObject obj) //音符对象回到对象池
    {
        if (obj != null)
        {
            obj.enabled = false;
            obj.gameObject.SetActive(false);
            noteObjectPool.Push(obj);
            Debug.Log("Lali-ho!");
        }
    }

    //-------------------------特效对象池方法--------------------------
    public GameObject GetFreshEffectObject(Stack<GameObject> stack, GameObject effectObject)
    {
        GameObject effectGo;
        if (stack.Count > 0)
        {
            effectGo = stack.Pop();
        }
        else
        {
            effectGo = Instantiate(effectObject);
        }

        effectGo.SetActive(true);

        return effectGo;
    }

    public void ReturnEffectToPool(GameObject effectGo, Stack<GameObject> stack)
    {
        if (effectGo != null)
        {
            effectGo.gameObject.SetActive(false);
            stack.Push(effectGo);
        }
    }
}