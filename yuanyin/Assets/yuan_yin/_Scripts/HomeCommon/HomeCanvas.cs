using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeCanvas : MonoBehaviour
{
    private CanvasScaler _canvas;
    private RectTransform _rectTransform;
    private Rect _saveRect;

    void Start()
    {
        _saveRect = Screen.safeArea;
        Debug.Log("SaveAreaX:" + _saveRect.position.x + "  SaveAreaY" + _saveRect.position.y);
        Debug.Log("SaveAreaWidth:" + _saveRect.width + "  SaveAreaHeight:" + _saveRect.height);
        _rectTransform = GetComponent<RectTransform>();
#if UNITY_ANDROID
        _rectTransform.position = new Vector3(_saveRect.position.x / 200f, 0, 90);
#elif UNITY_IPHONE
        _rectTransform.position = new Vector3(0, 0, 90);
#endif

        float save_k = _saveRect.width / _saveRect.height; //安全区比例
        float saveToScreen_k = _saveRect.height / Screen.height; //安全区高度和屏幕高度比例
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1080f);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1080f * saveToScreen_k * save_k);
        _rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    void Update()
    {
    }
}