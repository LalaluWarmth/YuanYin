using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SonicBloom.Koreo;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoteController : MonoBehaviour
{
    private static NoteController _instance;

    public static NoteController GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(NoteController)) as NoteController;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(NoteController)) as NoteController;
            }
        }

        return _instance;
    }


    [Tooltip("速度")] public float noteSpeed = 1;

    [Tooltip("Miss")] [Range(0f, 1000f)] public float hitMissRangeInMS;

    public float hitMissRangeInSamples
    {
        get { return SampleRate * (hitMissRangeInMS * 0.001f); }
    }

    [Tooltip("Great")] [Range(0f, 1000f)] public float hitGreatRangeInMS;

    public float hitGreatRangeInSamples
    {
        get { return SampleRate * (hitGreatRangeInMS * 0.001f); }
    }

    [Tooltip("Perfect")] [Range(0f, 1000f)]
    public float hitPerfectRangeInMS;

    public float hitPerfectRangeInSamples
    {
        get { return SampleRate * (hitPerfectRangeInMS * 0.001f); }
    }


    public int SampleRate //采样率
    {
        get { return playingKoreo.SampleRate; }
    }

    //-------------------------引用--------------------------
    public Koreography playingKoreo;
    public AudioSource audioCom;


    //-------------------------引导时间--------------------------
    [Tooltip("开始播放音频之前提供的时间量")] public float leadInTime;
    [SerializeField] private float timeLeftToPlay; //音乐开始之前的倒计时器

    public int DelayedSampleTime
    {
        get
        {
            return playingKoreo.GetLatestSampleTime() -
                   (int) (SampleRate * timeLeftToPlay); //乐谱上的采样点时刻-提前量对应采样点个数=>延迟调用
        }
    }

    private void Awake()
    {
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
    }

    void Start()
    {
        InitializeLeadIn();
    }


    void Update()
    {
        LeadinTimeCountDown();
    }


    private void InitializeLeadIn() //初始化引导时间并控制播放
    {
        if (leadInTime > 0)
        {
            timeLeftToPlay = leadInTime;
        }
        else
        {
            audioCom.Play();
        }
    }

    private void LeadinTimeCountDown()
    {
        if (timeLeftToPlay > 0)
        {
            timeLeftToPlay = Mathf.Max(timeLeftToPlay - Time.unscaledDeltaTime, 0);
            if (timeLeftToPlay <= 0)
            {
                audioCom.Play();
            }
        }
    }
}