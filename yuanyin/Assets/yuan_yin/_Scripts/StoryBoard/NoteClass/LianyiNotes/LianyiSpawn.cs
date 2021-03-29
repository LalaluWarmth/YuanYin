using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class LianyiSpawn : NoteSpawn<LianyiSpawn, LianyiNote>
{
    private LianyiHuanSpawn _lianyiHuanSpawn;

    void Start()
    {
        _lianyiHuanSpawn = LianyiHuanSpawn.GetInstance();
        ReturnHitEffectToPool(SampleHitEffectGo);
        ReturnFailEffectToPool(SampleFailEffectGo);
        ReturnNoteToPool(SampleNoteObject);
        InitNoteQueue();
    }


    void FixedUpdate()
    {
        CheckNoteSpawnNext(); //检测新音符的产生
    }


    private Queue<InitLianyiList> initLianyiNotesDatas = new Queue<InitLianyiList>();
    private List<InitLianyiList> tempInitLianyiLists = new List<InitLianyiList>();
    public Animator lianyiAppearAnimator;

    protected override void InitNoteQueue()
    {
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件
        foreach (var item in rawEvents)
        {
            string rawText = item.GetTextValue();
            List<float> positionData = new TextTok().TokText(rawText);
            int dataIndex = 0;
            while (dataIndex < positionData.Count - 1)
            {
                float group = positionData[dataIndex];
                float dir = positionData[dataIndex + 1];
                int nTime = item.StartSample;
                // Debug.Log(nTime);
                int dTime = item.StartSample - GetLianyiNoteSpawnSampleOffset();

                bool ifMatch = false;
                foreach (var eachList in tempInitLianyiLists)
                {
                    if (Mathf.Approximately(eachList.GetHead().Group, group))
                    {
                        eachList.Append(group, dir, nTime, dTime);
                        eachList.MoveFrist();
                        ifMatch = true;
                        break;
                    }
                }

                if (!ifMatch)
                {
                    InitLianyiList newList = new InitLianyiList();
                    newList.Append(group, dir, nTime, dTime);
                    tempInitLianyiLists.Add(newList);
                }

                dataIndex += 2;
            }
        }


        foreach (var eachList in tempInitLianyiLists)
        {
            initLianyiNotesDatas.Enqueue(eachList);
        }

        System.GC.Collect();
    }

    private int GetLianyiNoteSpawnSampleOffset() //确定在乐谱上产生音符的对应采样点的偏移量
    {
        AnimationClip[] animationClips = lianyiAppearAnimator.runtimeAnimatorController.animationClips;
        Debug.Log(animationClips[0].length);
        return (int) (animationClips[0].length * _noteController.SampleRate); //时间*采样率=采样点偏移量
    }

    public List<InitLianyiList> runningLists = new List<InitLianyiList>();
    public Transform LianyiHuanMeasurePos;

    protected override void CheckNoteSpawnNext() //不断检测是否生成下一个新音符
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= initLianyiNotesDatas.Count && initLianyiNotesDatas.Peek().GetHead().DTime <= currentTime)
        {
            InitLianyiList tempData = initLianyiNotesDatas.Dequeue();
            runningLists.Add(tempData);
            curNum++;
        }


        InitLianyiList onDeleteList = null;
        for (int i = 0; i < runningLists.Count; i++)
        {
            int curNodeNum = 1;
            while (curNodeNum <= runningLists[i].ListCount && runningLists[i].GetCurrent().DTime <= currentTime)
            {
                LianyiNote newObj = GetFreshNote();
                InitLianyiNoteNode tempNode = runningLists[i].GetCurrent();
                newObj.Initialized(tempNode.Group, tempNode.Dir, tempNode.NTime, tempNode.DTime);
                if (_lianyiHuanSpawn.huanMeasureRecord.ContainsKey(tempNode.Group))
                {
                    Vector2 notePos;
                    notePos.x = LianyiHuanMeasurePos.position.x + _lianyiHuanSpawn.huanMeasureRecord[tempNode.Group] *
                        Mathf.Cos(tempNode.Dir * Mathf.Deg2Rad);
                    notePos.y = LianyiHuanMeasurePos.position.y + _lianyiHuanSpawn.huanMeasureRecord[tempNode.Group] *
                        Mathf.Sin(tempNode.Dir * Mathf.Deg2Rad);
                    newObj.transform.position = notePos;
                }

                if (!runningLists[i].IsEof())
                {
                    runningLists[i].MoveNext();
                }
                else
                {
                    onDeleteList = runningLists[i];
                }

                curNodeNum++;
            }
        }

        if (onDeleteList != null)
        {
            runningLists.Remove(onDeleteList);
        }
    }


    protected override LianyiNote GetFreshNote() //从对象池中取对象的方法
    {
        LianyiNote retObj;
        if (notePool.Count > 0)
        {
            retObj = notePool.Pop();
        }
        else
        {
            retObj = Instantiate<LianyiNote>(NoteObject, storyboard);
        }

        retObj.gameObject.SetActive(true);
        retObj.enabled = true;
        return retObj;
    }
}