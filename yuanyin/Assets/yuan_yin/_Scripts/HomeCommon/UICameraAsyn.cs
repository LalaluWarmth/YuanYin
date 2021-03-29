using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraAsyn : MonoBehaviour
{
    private Camera _cameraSelf;

    // Start is called before the first frame update
    void Start()
    {
        _cameraSelf = GetComponent<Camera>();
        _cameraSelf.orthographicSize = Camera.main.orthographicSize;
        Debug.Log(Camera.main.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
    }
}