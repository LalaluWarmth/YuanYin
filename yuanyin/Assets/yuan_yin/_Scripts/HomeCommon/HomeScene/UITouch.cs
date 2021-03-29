using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UITouch : MonoBehaviour
{
    public Transform waiPanTrans;
    public Transform bodyTrans;

    void Start()
    {
    }


    void Update()
    {
        CheckIfOnUI();
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
        UIInstance.UiInstance.SetUIPanRotation(waiPanTrans.rotation);
        // Debug.Log(UIInstance.UiInstance.GetUIPanRotation());
        UIInstance.UiInstance.SetWaiPanScaleInStart(waiPanTrans.localScale);
        UIInstance.UiInstance.SetBodyPosInStart(bodyTrans.localPosition);
        UIInstance.UiInstance.SetBodyScaleInStart(bodyTrans.localScale);

        SceneManager.LoadScene("SelectScene");
    }
}