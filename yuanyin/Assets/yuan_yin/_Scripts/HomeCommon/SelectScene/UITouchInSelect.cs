using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UITouchInSelect : MonoBehaviour
{
    public SelectSceneAnimationController SSAC;
    public bool isChanging;

    void Start()
    {
    }


    void Update()
    {
        // CheckIfOnUI();
        CheckSlide();
    }

    private void CheckIfOnUI()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
#if UNITY_IPHONE || UNITY_ANDROID
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
                Debug.Log("当前触摸在UI上");

            else
            {
                Debug.Log("当前没有触摸在UI上");
            }
        }
    }

    public void SettingsUI()
    {
        Debug.Log("在Settings上");
    }

    public void StartUI()
    {
        Debug.Log("在Start上");
    }

    //---------------------------------------------------
    public enum slideVector
    {
        nullVector,
        up,
        down,
        left,
        right
    };

    private Vector2 touchFirst = Vector2.zero; //手指开始按下的位置
    private Vector2 touchSecond = Vector2.zero; //手指拖动的位置
    private slideVector currentVector = slideVector.nullVector; //当前滑动方向
    private float timer; //时间计数器  
    private bool ifEnd; //为单词滑动检测服务的flag
    public float offsetTime = 0.1f; //判断的时间间隔 
    public float SlidingDistance = Screen.width * 0.15f;

    void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
            //判断当前手指是按下事件 
        {
            touchFirst = Event.current.mousePosition; //记录开始按下的位置
            ifEnd = false;
        }

        if (Event.current.type == EventType.MouseDrag)
            //判断当前手指是拖动事件
        {
            touchSecond = Event.current.mousePosition;

            timer += Time.deltaTime; //计时器

            if (timer > offsetTime)
            {
                touchSecond = Event.current.mousePosition; //记录结束下的位置
                Vector2 slideDirection = touchFirst - touchSecond;
                float x = slideDirection.x;
                float y = slideDirection.y;

                if (y + SlidingDistance < x && y > -x - SlidingDistance && !ifEnd)
                {
                    Debug.Log("left");

                    currentVector = slideVector.left;
                    ifEnd = true;
                }
                else if (y > x + SlidingDistance && y < -x - SlidingDistance && !ifEnd)
                {
                    Debug.Log("right");

                    currentVector = slideVector.right;
                    ifEnd = true;
                }
                else if (y > x + SlidingDistance && y - SlidingDistance > -x && !ifEnd)
                {
                    Debug.Log("up");

                    currentVector = slideVector.up;
                    ifEnd = true;
                }
                else if (y + SlidingDistance < x && y < -x - SlidingDistance && !ifEnd)
                {
                    Debug.Log("Down");

                    currentVector = slideVector.down;
                    ifEnd = true;
                }

                timer = 0;
                touchFirst = touchSecond;
            }
        } // 滑动方法

        if (Event.current.type == EventType.MouseUp)
        {
            //滑动结束  
            currentVector = slideVector.nullVector;
            Debug.Log("当前触摸状态：" + currentVector);
            ifEnd = false;
        }
    }

    //---------------------------------------------------
    private void CheckSlide()
    {
        if (currentVector == slideVector.up && !isChanging)
        {
            isChanging = true;
            SSAC.LoadNewSongInfo(currentVector);
        }

        if (currentVector == slideVector.down && !isChanging)
        {
            isChanging = true;
            SSAC.LoadNewSongInfo(currentVector);
        }
    }
}