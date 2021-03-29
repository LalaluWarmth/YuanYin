using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class HoldNoteSpawn : MonoBehaviour//todo 需求有变，暂时禁用
{
    private static HoldNoteSpawn _instance;

    public static HoldNoteSpawn GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(HoldNoteSpawn)) as HoldNoteSpawn;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(HoldNoteSpawn)) as HoldNoteSpawn;
            }
        }

        return _instance;
    }

    [Tooltip("事件对应ID")] [EventID] public string eventID;
    [Tooltip("事件对应ID")] [EventID] public string moveDataID;

    //-------------------------引用--------------------------
    public Koreography playingKoreo;
    public Transform bgEdge;
    private NoteController _noteController;

    //-------------------------对象池--------------------------

    Stack<HoldNote> notePool = new Stack<HoldNote>(); //音符
    // public Stack<GameObject> hitEffectObjectPool = new Stack<GameObject>(); //成功特效
    // public Stack<GameObject> failEffectObjectPool = new Stack<GameObject>(); //失败特效

    //-------------------------音符--------------------------
    public HoldNote HoldNoteObject;
    public HoldNote SampleHoldNoteObject;

    // public GameObject hitEffectGo;
    // private Animator hitEffectGoAnimator;
    // public GameObject SampleHitEffectGo;

    // public GameObject failEffectGo;
    // private Animator failEffectGoAnimator;
    // public GameObject SampleFailEffectGo;


    public RectTransform holdNoteTargetCenter;
    public RectTransform holdNoteTargetRuler0;
    public RectTransform holdNoteTargetRuler1;


    public Transform storyboard;

    private void Awake()
    {
        _noteController = NoteController.GetInstance();
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
        // hitEffectGoAnimator = hitEffectGo.GetComponent<Animator>();
        // failEffectGoAnimator = failEffectGo.GetComponent<Animator>();
    }

    void Start()
    {
        // ReturnHitEffectToPool(SampleHitEffectGo);
        // ReturnFailEffectToPool(SampleFailEffectGo);
        ReturnHoldNoteToPool(SampleHoldNoteObject);
        InitHoldNoteQueue();
    }


    void FixedUpdate()
    {
        CheckHoldNoteSpawnNext(); //检测新音符的产生
    }

    #region HoldNoteSpawnLogic

    //-------------------------HoldNote--------------------------

    private List<InitHoldNote> tempHoldNotesDatas = new List<InitHoldNote>();
    private Queue<InitHoldNote> initHoldNotesDatas = new Queue<InitHoldNote>();
    private float _holdNoteDistance;

    private void InitHoldNoteQueue()
    {
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件
        KoreographyTrackBase moveDataTrack = playingKoreo.GetTrackByID(moveDataID); //获取body移动
        List<KoreographyEvent> moveDataEvents = moveDataTrack.GetAllEvents(); //获取所有body移动事件
        List<InitHoldNote> unMatchHoldNotes = new List<InitHoldNote>();
        foreach (var item in rawEvents)
        {
            string rawText = item.GetTextValue();
            List<float> positionData = new TextTok().TokText(rawText);
            int dataIndex = 0;
            while (dataIndex < positionData.Count)
            {
                float group = positionData[dataIndex];
                float rad = positionData[dataIndex + 1] * Mathf.Deg2Rad;
                Vector2 startPos = CalculateHoldStartPos(rad);
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

                int targetTrack = (int) positionData[dataIndex + 2];
                Vector2 targetPosition = CalculateHoldTargetPos(rad, offset, targetTrack);

                bool ifMatched = false;
                foreach (var unMatchItem in unMatchHoldNotes)
                {
                    if (Mathf.Approximately(unMatchItem.Group, group))
                    {
                        unMatchItem.NTime1 = item.StartSample;
                        unMatchItem.DTime1 = item.StartSample -
                                             GetHoldNoteSpawnSampleOffset(startPos, targetPosition);
                        tempHoldNotesDatas.Add(unMatchItem);
                        unMatchHoldNotes.Remove(unMatchItem);
                        ifMatched = true;
                        break;
                    }
                }

                if (!ifMatched)
                {
                    InitHoldNote newUnmatchHold = new InitHoldNote();
                    newUnmatchHold.Group = group;
                    newUnmatchHold.Direction = rad;
                    newUnmatchHold.TPosition = targetPosition;
                    newUnmatchHold.NTime0 = item.StartSample;
                    newUnmatchHold.DTime0 = item.StartSample -
                                            GetHoldNoteSpawnSampleOffset(startPos, targetPosition);
                    unMatchHoldNotes.Add(newUnmatchHold);
                }

                dataIndex += 3;

                // Debug.Log(temp.Direction + "+" + temp.TPosition + "+" +
                //           GetClickNoteSpawnSampleOffset(startPos, temp.TPosition));
            }
        }

        tempHoldNotesDatas.Sort();

        for (int i = 0; i < tempHoldNotesDatas.Count; i++)
        {
            // Debug.Log("AAAAAAA  " + tempHoldNotesDatas[i].DTime0);
            initHoldNotesDatas.Enqueue(tempHoldNotesDatas[i]);
        }

        tempHoldNotesDatas.Clear();
        System.GC.Collect();
    }

    private Vector2 CalculateHoldStartPos(float degree)
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


    private Vector2 CalculateHoldTargetPos(float degree, Vector2 offset, int track)
    {
        Vector2 unitPos = Vector2.zero;
        switch (track)
        {
            case 0:
                unitPos = holdNoteTargetRuler0.position - holdNoteTargetCenter.position;
                break;
            case 1:
                unitPos = holdNoteTargetRuler1.position - holdNoteTargetCenter.position;
                break;
        }

        Vector2 pos;
        pos.x = unitPos.x * Mathf.Cos(degree) - unitPos.y * Mathf.Sin(degree);
        pos.y = unitPos.x * Mathf.Sin(degree) + unitPos.y * Mathf.Cos(degree);
        return pos + offset;
    }

    private int GetHoldNoteSpawnSampleOffset(Vector2 curHoldNoteStartPosition,
        Vector2 curHoldNoteTargetPosition) //确定在乐谱上产生音符的对应采样点的偏移量
    {
        float spawnDistToTarget =
            Vector2.Distance(curHoldNoteStartPosition, curHoldNoteTargetPosition); //计算生成为位置和结束位置的距离
        float spawnPosToTargetTime = spawnDistToTarget / _noteController.noteSpeed; //计算出音符到达结束位置所需的时间
        return (int) (spawnPosToTargetTime * _noteController.SampleRate); //时间*采样率=采样点偏移量
    }

    private void CheckHoldNoteSpawnNext() //不断检测是否生成下一个新音符
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= initHoldNotesDatas.Count && initHoldNotesDatas.Peek().DTime0 <= currentTime)
        {
            // Debug.Log("HoldSpawn!!");
            InitHoldNote tempData = initHoldNotesDatas.Dequeue();
            // Debug.Log("fucku//" + tempData.dTime);
            // Debug.Log("TimeToGo!");
            HoldNote newObj = GetFreshHoldNote();
            newObj.Initialized(tempData.Group, tempData.TPosition, tempData.Direction, tempData.NTime0,
                tempData.DTime0, tempData.NTime1, tempData.DTime1);
            curNum++;
        }
    }

    #endregion

    #region HoldNote音符对象池方法

    //-------------------------HoldNote音符对象池方法--------------------------
    private HoldNote GetFreshHoldNote() //从对象池中取对象的方法
    {
        HoldNote retObj;
        if (notePool.Count > 0)
        {
            retObj = notePool.Pop();
        }
        else
        {
            retObj = Instantiate<HoldNote>(HoldNoteObject, storyboard);
        }
        
        retObj.gameObject.SetActive(true);
        retObj.enabled = true;
        return retObj;
    }

    public void ReturnHoldNoteToPool(HoldNote obj) //音符对象回到对象池
    {
        if (obj)
        {
            obj.enabled = false;
            obj.gameObject.SetActive(false);
            notePool.Push(obj);
            // Debug.Log("Lali-ho!");
        }
    }

    #endregion

    #region HoldNote特效对象池方法

    //-------------------------HoldNote成功特效对象池方法--------------------------
    // public void GetHitEffect(Vector2 curPos)
    // {
    //     GameObject retObj;
    //     if (hitEffectObjectPool.Count > 0)
    //     {
    //         retObj = hitEffectObjectPool.Pop();
    //     }
    //     else
    //     {
    //         retObj = Instantiate<GameObject>(hitEffectGo, storyboard);
    //     }
    //
    //     retObj.transform.position = curPos;
    //     retObj.SetActive(true);
    //     StartCoroutine(DestroyHitEffect(retObj));
    // }
    //
    // private void ReturnHitEffectToPool(GameObject hitEffectGOLocal)
    // {
    //     if (hitEffectGOLocal)
    //     {
    //         hitEffectGOLocal.SetActive(false);
    //         hitEffectObjectPool.Push(hitEffectGOLocal);
    //     }
    // }
    //
    // IEnumerator DestroyHitEffect(GameObject hitEffectGOLocal)
    // {
    //     yield return new WaitForSeconds(hitEffectGoAnimator.GetCurrentAnimatorStateInfo(0).length);
    //     ReturnHitEffectToPool(hitEffectGOLocal);
    // }
    //
    // //-------------------------HoldNote失败特效对象池方法--------------------------
    //
    // public void GetFailEffect(Vector2 curPos)
    // {
    //     GameObject retObj;
    //     if (failEffectObjectPool.Count > 0)
    //     {
    //         retObj = failEffectObjectPool.Pop();
    //     }
    //     else
    //     {
    //         retObj = Instantiate<GameObject>(failEffectGo, storyboard);
    //     }
    //
    //     retObj.transform.position = curPos;
    //     retObj.SetActive(true);
    //     StartCoroutine(DestroyFailEffect(retObj));
    // }
    //
    // private void ReturnFailEffectToPool(GameObject failEffectGOLocal)
    // {
    //     if (failEffectGOLocal)
    //     {
    //         failEffectGOLocal.SetActive(false);
    //         failEffectObjectPool.Push(failEffectGOLocal);
    //     }
    // }
    //
    // IEnumerator DestroyFailEffect(GameObject failEffectGOLocal)
    // {
    //     Animator failEffectGoAnimator = failEffectGOLocal.GetComponent<Animator>();
    //     yield return new WaitForSeconds(failEffectGoAnimator.GetCurrentAnimatorStateInfo(0).length);
    //     ReturnFailEffectToPool(failEffectGOLocal);
    // }

    #endregion
}