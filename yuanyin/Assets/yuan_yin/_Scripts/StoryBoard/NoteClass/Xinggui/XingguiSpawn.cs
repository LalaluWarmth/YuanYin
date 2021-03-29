using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class XingguiSpawn : NoteSpawn<XingguiSpawn, XingguiNote>
{
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


    private Queue<InitXingguiList> initXingguiNotesDatas = new Queue<InitXingguiList>();
    private List<InitXingguiList> tempInitXingguiLists = new List<InitXingguiList>();
    public Animator xingguiAppearAnimator;

    protected override void InitNoteQueue()
    {
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID); //获取时间轨迹
        List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents(); //获取所有事件
        foreach (var item in rawEvents)
        {
            string rawText = item.GetTextValue();
            List<float> positionData = new TextTok().TokText(rawText);
            int dataIndex = 0;
            while (dataIndex < positionData.Count - 2)
            {
                float group = positionData[dataIndex];
                Vector2 pos = new Vector2(positionData[dataIndex + 1], positionData[dataIndex + 2]);
                int nTime = item.StartSample;
                // Debug.Log(nTime);
                int dTime = item.StartSample - GetXingguiNoteSpawnSampleOffset();

                bool ifMatch = false;
                foreach (var eachList in tempInitXingguiLists)
                {
                    if (Mathf.Approximately(eachList.GetHead().Group, group))
                    {
                        eachList.Append(group, pos, nTime, dTime);
                        eachList.MoveFrist();
                        ifMatch = true;
                        break;
                    }
                }

                if (!ifMatch)
                {
                    InitXingguiList newList = new InitXingguiList();
                    newList.Append(group, pos, nTime, dTime);
                    tempInitXingguiLists.Add(newList);
                }

                dataIndex += 3;
            }
        }


        foreach (var eachList in tempInitXingguiLists)
        {
            initXingguiNotesDatas.Enqueue(eachList);
        }

        System.GC.Collect();
    }

    private int GetXingguiNoteSpawnSampleOffset() //确定在乐谱上产生音符的对应采样点的偏移量
    {
        AnimationClip[] animationClips = xingguiAppearAnimator.runtimeAnimatorController.animationClips;
        Debug.Log(animationClips[0].length);
        return (int) (animationClips[0].length * _noteController.SampleRate); //时间*采样率=采样点偏移量
    }

    public List<InitXingguiList> runningLists = new List<InitXingguiList>();

    protected override void CheckNoteSpawnNext() //不断检测是否生成下一个新音符
    {
        int currentTime = _noteController.DelayedSampleTime;
        int curNum = 1;
        while (curNum <= initXingguiNotesDatas.Count && initXingguiNotesDatas.Peek().GetHead().DTime <= currentTime)
        {
            InitXingguiList tempData = initXingguiNotesDatas.Dequeue();
            runningLists.Add(tempData);
            curNum++;
        }


        InitXingguiList onDeleteList = null;
        for (int i = 0; i < runningLists.Count; i++)
        {
            int curNodeNum = 1;
            while (curNodeNum <= runningLists[i].ListCount && runningLists[i].GetCurrent().DTime <= currentTime)
            {
                XingguiNote newObj = GetFreshNote();
                InitXingguiNoteNode tempNode = runningLists[i].GetCurrent();
                newObj.Initialized(tempNode.Group, tempNode.TPosition, tempNode.NTime, tempNode.DTime);
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

    protected override XingguiNote GetFreshNote() //从对象池中取对象的方法
    {
        XingguiNote retObj;
        if (notePool.Count > 0)
        {
            retObj = notePool.Pop();
        }
        else
        {
            retObj = Instantiate<XingguiNote>(NoteObject, storyboard);
        }

        retObj.transform.position = Vector2.one * 200;
        retObj.gameObject.SetActive(true);
        retObj.enabled = true;
        return retObj;
    }
}