using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UIData
{
    public Quaternion panRotation;
    public bool isTablet;
    public Vector3 bodyPosInStart;
    public Vector3 bodyScaleInStart;
    public Vector3 waiPanScaleInStart;
}

public class UIInstance : MonoBehaviour
{
    private static UIInstance _instance;

    private UIData _myUIData;

    public static UIInstance UiInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UIInstance)) as UIInstance;
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent(typeof(UIInstance)) as UIInstance;
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            _instance = this as UIInstance;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetUIPanRotation(Quaternion quaternion)
    {
        _myUIData.panRotation = quaternion;
    }

    public Quaternion GetUIPanRotation()
    {
        return _myUIData.panRotation;
    }

    public void SetIsTablet(bool istablet)
    {
        _myUIData.isTablet = istablet;
    }

    public bool GetIsTablet()
    {
        return _myUIData.isTablet;
    }

    public void SetBodyPosInStart(Vector3 bodyPosInStart)
    {
        _myUIData.bodyPosInStart = bodyPosInStart;
    }

    public Vector3 GetBodyPosInStart()
    {
        return _myUIData.bodyPosInStart;
    }

    public void SetBodyScaleInStart(Vector3 bodyScaleInStart)
    {
        _myUIData.bodyScaleInStart = bodyScaleInStart;
    }

    public Vector3 GetBodyScaleInStart()
    {
        return _myUIData.bodyScaleInStart;
    }

    public void SetWaiPanScaleInStart(Vector3 waiPanScaleInStart)
    {
        _myUIData.waiPanScaleInStart = waiPanScaleInStart;
    }

    public Vector3 GetWaiPanScaleInStart()
    {
        return _myUIData.waiPanScaleInStart;
    }


    void Start()
    {
    }

    void Update()
    {
    }
}