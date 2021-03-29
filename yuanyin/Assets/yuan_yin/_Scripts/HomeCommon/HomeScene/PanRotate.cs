using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanRotate : MonoBehaviour
{
    private Transform _transform;
    public float speed = 5;

    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    void Update()
    {
        _transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}