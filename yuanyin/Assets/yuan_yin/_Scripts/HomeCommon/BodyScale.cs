using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyScale : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private bool bIsTablet;


    void Start()
    {
        _transform = GetComponent<Transform>();

#if UNITY_IPHONE
        string deviceModel = SystemInfo.deviceModel.ToLower().Trim();
        if(deviceModel.StartsWith("ipad"))
        {
        bIsTablet = true;
        }
        else
        {
        bIsTablet = false;
        }
#elif UNITY_ANDROID
        float physicScreenSize = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi;
        if (physicScreenSize >= 7f) //If the screen size is >= 7 inches, it's a tablet
        {
            bIsTablet = true;
        }
        else
        {
            bIsTablet = false;
        }
#else
        bIsTablet = false;
#endif
        
        UIInstance.UiInstance.SetIsTablet(bIsTablet);

        if (bIsTablet)
        {
            _transform.localScale = new Vector3(0.7f, 0.7f);
        }
        else
        {
            _transform.localScale = new Vector3(1f, 1f);
        }

#if UNITY_ANDROID
        _transform.position = new Vector3(Screen.safeArea.position.x / 200, 0, 0);
#elif UNITY_IPHONE
        _transform.position = new Vector3(0,0,0);
#endif
    }


    void Update()
    {
    }
}