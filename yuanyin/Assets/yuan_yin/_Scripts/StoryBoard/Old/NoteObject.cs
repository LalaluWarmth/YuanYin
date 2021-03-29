using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public SpriteRenderer visuals;

    public Sprite[] noteSprites;

    private KoreographyEvent trackedEvent;

    public bool isLongNote;
    public bool isLongNoteEnd;

    [SerializeField] private LaneController laneController;
    private Transform targetTop;
    private Transform targetBottom;

    private GameController gameController;

    public int hitOffset;

    void Start()
    {
    }


    void Update()
    {
        UpdatePosition();
        GetHiteOffset();
        if (targetBottom.position.x - targetTop.position.x >= 0)
        {
            if (transform.position.x - targetBottom.position.x >= 1) //当音符离开音轨，自己销毁并重置
            {
                RetuenToPool();
            }
        }
        else
        {
            if (transform.position.x - targetBottom.position.x <= -1) //当音符离开音轨，自己销毁并重置
            {
                RetuenToPool();
            }
        }
    }

    public void Initialized(KoreographyEvent evt, int noteNum, LaneController laneCont, GameController gameCont,
        bool isLongStart, bool isLongEnd) //初始化方法
    {
        trackedEvent = evt;
        laneController = laneCont;
        targetTop = laneCont.targetTopTrans;
        targetBottom = laneCont.targetBottomTrans;
        gameController = gameCont;
        isLongNote = isLongStart;
        isLongNoteEnd = isLongEnd;
        int spriteNum = noteNum;
        if (isLongNote)
        {
            spriteNum += 6;
        }
        else if (isLongNoteEnd)
        {
            spriteNum += 12;
        }

        visuals.sprite = noteSprites[spriteNum - 1];
    }

    private void ResetNote() //重置Note对象
    {
        trackedEvent = null;
        laneController = null;
        gameController = null;
    }

    private void RetuenToPool() //返回对象池
    {
        gameController.ReturnNoteObjectToPool(this);
        ResetNote();
        // Debug.Log("Lali-ho!");
    }

    public void Onhit() //击中音符对象
    {
        RetuenToPool();
    }

    private void UpdatePosition() //更新位置
    {
        Vector3 pos = laneController.TargetPosition;
        float x = Mathf.Abs(targetTop.position.x - targetBottom.position.x);
        float y = Mathf.Abs(targetTop.position.y - targetBottom.position.y);
        float ss = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
        float xsin = (targetBottom.position.x - targetTop.position.x) / ss;
        float ysin = (targetBottom.position.y - targetTop.position.y) / ss;
        pos.x += (gameController.DelayedSampleTime - trackedEvent.StartSample) / (float) gameController.SampleRate *
                 gameController.noteSpeed * xsin;
        pos.y += (gameController.DelayedSampleTime - trackedEvent.StartSample) / (float) gameController.SampleRate *
                 gameController.noteSpeed * ysin;

        transform.position = pos;
    }

    private void GetHiteOffset() //计算音符偏移量（与HitWindow相减），放在update中观察
    {
        int curTime = gameController.DelayedSampleTime;
        int noteTime = trackedEvent.StartSample;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
        int hitWindow = gameController.HitWindowSampleWidth;
        hitOffset = hitWindow - Mathf.Abs(noteTime - curTime);
    }

    public bool IsNoteMissed() //当前音符是否已经miss，在LaneController中使用
    {
        bool bMissed = true;
        if (enabled)
        {
            int curTime = gameController.DelayedSampleTime;
            int noteTime = trackedEvent.StartSample;
            int hitWindow = gameController.HitWindowSampleWidth;
            bMissed = curTime - noteTime > hitWindow;
        }

        return bMissed;
    }

    public int IsNoteHittable() //判断命中的评级，放在LaneController中
    {
        int hitLevel = 0;
        if (hitOffset >= 0)
        {
            if (hitOffset >= 7400 && hitOffset <= 12000)
            {
                hitLevel = 2;
            }
            else
            {
                hitLevel = 1;
            }
        }
        else
        {
            this.enabled = false; //miss就将这个音符的脚本失活
        }

        return hitLevel;
    }
}