using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailAlign : MonoBehaviour
{
    public Transform[] nails;

    public float length;
    // Start is called before the first frame update
    void Start()
    {
        float degree = 0;
        foreach (var item in nails)
        {
            float rad = degree / 180 * Mathf.PI;
            item.localPosition = new Vector3(length * Mathf.Cos(rad),length * Mathf.Sin(rad),0);
            degree += 45;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
