using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class LaneController : MonoBehaviour
{
    private GameController gameController;

    [Tooltip("按键设置")] public KeyCode keyboardButton;

    [Tooltip("音轨编号")] public int laneID;

    [Tooltip("键盘按下视觉效果")] public Transform targetVisuals;

    //-------------------------上下边界--------------------------
    public Transform targetTopTrans;
    public Transform targetBottomTrans;

    private List<KoreographyEvent> laneEvents = new List<KoreographyEvent>(); //包含在此音轨中的所有时间列表

    private Queue<NoteObject> trackedNotes = new Queue<NoteObject>(); //包含此音轨当前活动的所有音符对象的队列

    private int pendingEventIndex = 0; //检测此音轨中的生成的下一个事件的索引

    public Vector3 TargetPosition
    {
        get { return this.gameObject.transform.GetChild(1).transform.position; }
    }

    void Start()
    {
    }

    private Ray ray;
    private RaycastHit hit;

    public bool hasLongNote;
    public float timeVal;
    public GameObject longNoteHitEffectGO;

    void Update()
    {
        while (trackedNotes.Count > 0 && trackedNotes.Peek().IsNoteMissed()) //清除出音轨未点击的无效音符
        {
            if (trackedNotes.Peek().isLongNoteEnd)
            {
                hasLongNote = false;
                timeVal = 0;
            }

            trackedNotes.Dequeue();
        }

        CheckSpawnNext(); //检测新音符的产生
        if (Input.GetMouseButtonDown(0))
        {
            // 主相机屏幕点转换为射线
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //射线碰到了物体
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.transform == gameObject.transform.GetChild(1))
                {
                    Debug.Log(laneID+"被单击");
                    CheckNoteHit();
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            // 主相机屏幕点转换为射线
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //射线碰到了物体
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.transform == gameObject.transform.GetChild(1))
                {
                    Debug.Log(laneID+"被长按");
                    if (hasLongNote)
                    {
                        if (timeVal >= 0.15f)
                        {
                            if (longNoteHitEffectGO.activeSelf)
                            {
                                CreateLongEffect();
                            }

                            timeVal = 0;
                        }
                        else
                        {
                            timeVal += Time.deltaTime;
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 主相机屏幕点转换为射线
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //射线碰到了物体
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.transform == gameObject.transform.GetChild(1))
                {
                    Debug.Log(laneID+"鼠标抬起");
                    if (hasLongNote)
                    {
                        longNoteHitEffectGO.SetActive(false);
                        CheckNoteHit();
                    }
                }
            }
        }
    }

    public void Initialized(GameController controller) //初始化每条音轨获取到控制器（之后考虑一下单例）
    {
        gameController = controller;
    }

    public bool DoesMatch(int noteID) //检测事件是否匹配当前编号的音轨
    {
        return noteID == laneID;
    }

    public void AddEventToLane(KoreographyEvent evt) //如果匹配，则把当前事件添加进音轨所持有的事件列表
    {
        laneEvents.Add(evt);
    }

    private int GetSpawnSampleOffset() //确定在乐谱上产生音符的对应采样点的偏移量
    {
        float x = Mathf.Abs(targetTopTrans.position.x - targetBottomTrans.position.x);
        float y = Mathf.Abs(targetTopTrans.position.y - targetBottomTrans.position.y);
        float spawnDistToTarget = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)); //计算生成为位置和结束位置的距离

        float spawnPosToTargetTime = spawnDistToTarget / gameController.noteSpeed; //计算出音符到达结束位置所需的时间

        return (int) (spawnPosToTargetTime * gameController.SampleRate); //时间*采样率=采样点偏移量
    }

    private void CheckSpawnNext() //不断检测是否生成下一个新音符
    {
        int samplesToTarget = GetSpawnSampleOffset();

        int currentTime = gameController.DelayedSampleTime;

        while (pendingEventIndex < laneEvents.Count &&
               laneEvents[pendingEventIndex].StartSample < currentTime + samplesToTarget)
        {
            KoreographyEvent evt = laneEvents[pendingEventIndex];
            int noteNum = evt.GetIntValue();
            NoteObject newObj = gameController.GetFreshNoteObject();

            bool isLongNoteStart = false;
            bool isLongNoteEnd = false;
            // if (noteNum > 6)
            // {
            //     isLongNoteStart = true;
            //     noteNum -= 6;
            //     if (noteNum > 6)
            //     {
            //         isLongNoteEnd = true;
            //         isLongNoteStart = false;
            //         noteNum -= 6;
            //     }
            // }

            newObj.Initialized(evt, noteNum, this, gameController, isLongNoteStart, isLongNoteEnd);
            trackedNotes.Enqueue(newObj);
            pendingEventIndex++;
        }
    }

//-------------------------生成特效有关方法-------------------------
    private void CreateDownEffect()
    {
        GameObject downEffectGo =
            gameController.GetFreshEffectObject(gameController.downEffectObjectPool, gameController.downEffectGo);
        downEffectGo.transform.position = targetVisuals.position;
    }

    private void CreateHitEffect()
    {
        GameObject hitEffectGo =
            gameController.GetFreshEffectObject(gameController.hitEffectObjectPool, gameController.hitEffectGo);
        hitEffectGo.transform.position = targetVisuals.position;
    }

    private void CreateLongEffect()
    {
        longNoteHitEffectGO.SetActive(true);
        longNoteHitEffectGO.transform.position = targetVisuals.position;
    }

    public void CheckNoteHit() //检测音符是否击中，若是，它将执行命中并删除
    {
        if (trackedNotes.Count > 0)
        {
            NoteObject noteObject = trackedNotes.Peek();
            if (noteObject.hitOffset > -6800) //该音符偏移量小于-6800时检测命中
            {
                trackedNotes.Dequeue(); //出队
                int hitLevel = noteObject.IsNoteHittable();
                if (hitLevel > 0) //击中
                {
                    if (noteObject.isLongNote)
                    {
                        hasLongNote = true;
                        CreateLongEffect();
                    }
                    else if (noteObject.isLongNoteEnd)
                    {
                        hasLongNote = false;
                    }
                    else
                    {
                        CreateHitEffect();
                    }
                }
                else //未击中
                {
                }

                noteObject.Onhit();
            }
            else //该音符偏移量大于-6000时不检测命中，只产生特效
            {
                CreateDownEffect();
            }
        }
        else
        {
            CreateDownEffect();
        }
    }
}