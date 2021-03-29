using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LianyihuanRenderer : MonoBehaviour
{
    private float t;
    private float x;
    public float speed;
    private Vector3 changeScale;
    private LianyiHuanSpawn _lianyiHuanSpawn;

    void Start()
    {
        t = 0;
        _lianyiHuanSpawn=LianyiHuanSpawn.GetInstance();
    }


    void Update()
    {
        if (t <= 1)
        {
            x = Mathf.Lerp(4, 61, t);
            t += Time.deltaTime * speed;
            changeScale.x = x;
            changeScale.y = x;
            changeScale.z = x;
            transform.localScale = changeScale;
        }
        else
        {
            t = 0;
            x = 4;
            changeScale.x = x;
            changeScale.y = x;
            changeScale.z = x;
            transform.localScale = changeScale;
            _lianyiHuanSpawn.ReturnHuanRendererToPool(this.gameObject);
        }


        if (transform.localScale.x > 35)
        {
            // transform.GetComponent<SpriteRenderer>().DOFade(0, 1);
        }
    }
}