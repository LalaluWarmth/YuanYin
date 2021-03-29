using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianyiMeasure : MonoBehaviour
{
    private float t;
    private float x;
    public float speed;
    private Vector3 changeScale;
    public Transform ruler;
    private float dis;
    private LianyiHuanSpawn _lianyiHuanSpawn;

    private float NO;

    void Start()
    {
        t = 0;
        _lianyiHuanSpawn = LianyiHuanSpawn.GetInstance();
    }

    public void Initialized(float no)
    {
        NO = no;
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

            dis = Vector2.Distance(ruler.position, transform.position);
            if (!_lianyiHuanSpawn.huanMeasureRecord.ContainsKey(NO))
            {
                _lianyiHuanSpawn.huanMeasureRecord.Add(NO, dis);
            }
            else
            {
                _lianyiHuanSpawn.huanMeasureRecord[NO] = dis;
            }
        }
        else
        {
            t = 0;
            x = 4;
            changeScale.x = x;
            changeScale.y = x;
            changeScale.z = x;
            transform.localScale = changeScale;
            _lianyiHuanSpawn.huanMeasureRecord.Remove(NO);
            _lianyiHuanSpawn.ReturnHuanMeasureToPool(this.gameObject);
        }


        if (transform.localScale.x > 35)
        {
            // transform.GetComponent<SpriteRenderer>().DOFade(0, 1);
        }
    }
}