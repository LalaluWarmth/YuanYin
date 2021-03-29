using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartAnimController : MonoBehaviour
{
    public float fadeOutTime = 1.0f;
    public Image uiButtonStart;

    public Image[] uiDeco;

    public SpriteRenderer baiSePanZi;

    public SpriteRenderer[] chaoGeArray;

    public SpriteRenderer[] dyArray;

    // Start is called before the first frame update
    void Start()
    {
        uiButtonStart.DOFade(1, fadeOutTime);
        foreach (var item in uiDeco)
        {
            item.DOFade(1, fadeOutTime);
        }

        baiSePanZi.DOFade(1, fadeOutTime);
        foreach (var item in chaoGeArray)
        {
            item.DOFade(1, fadeOutTime);
        }

        foreach (var item in dyArray)
        {
            item.DOFade((float)107/255, fadeOutTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}