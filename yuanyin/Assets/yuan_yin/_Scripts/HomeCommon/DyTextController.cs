using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyTextController : MonoBehaviour
{
    public GameObject[] dy;
    public GameObject[] nails;
    private int index = 0;

    void Start()
    {
    }


    void Update()
    {
        foreach (var item in dy)
        {
            item.transform.position = nails[index].transform.position;
            index++;
        }

        index = 0;
    }
}