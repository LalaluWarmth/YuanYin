using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngineInternal;

public class SelectSceneAnimationController : MonoBehaviour
{
    private bool _isTablet;

    public float speed = 5;
    private bool canPanRotate;

    public float fadeOutTime = 0.5f;
    public float fadeInTime = 0.5f;

    public Image uiButtonStart;
    public Image uiButtonBack;
    public Image uiEasy;
    public Image uiDifficult;
    public Image uiStartOnInnerPlate;
    public Image[] uiDeco;
    public SpriteRenderer baiSePanZi; // TODO | Rename this
    public GameObject chaoGe;
    private SpriteRenderer[] _chaoGeArray;
    public GameObject dy;
    private SpriteRenderer[] _dyArray;
    public SpriteRenderer[] selectBG;

    public Transform bodyTrans;
    public Transform waiPanTrans;
    public Transform[] nailsTrans;
    public Transform nailFatherTrans;
    public Transform nailGrandFatherTrans;
    public RectTransform songInfoRectTransOnWaiPan;
    private Image[] _uiSongImageOnWaiPan;
    private Text[] _uiSongTextOnWaiPan;
    public Image uiSongInfo;
    private int _songIndex = 0;

    void Start()
    {
        _isTablet = UIInstance.UiInstance.GetIsTablet();

        waiPanTrans.rotation = UIInstance.UiInstance.GetUIPanRotation();
        canPanRotate = true;

        uiButtonStart.gameObject.GetComponent<Button>().enabled = false;
        uiButtonStart.DOFade(0, fadeOutTime).OnPlay(OnOpeningAnimPlay).OnComplete(OnOpeningAnimComplete);
        foreach (var item in uiDeco)
        {
            item.DOFade(0, fadeOutTime);
        }


        baiSePanZi.material.DOFade(0, fadeOutTime);


        _chaoGeArray = chaoGe.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer item in _chaoGeArray)
        {
            item.material.DOFade(0, fadeOutTime);
            item.gameObject.SetActive(false);
        }

        _dyArray = dy.GetComponentsInChildren<SpriteRenderer>();
        foreach (var item in _dyArray)
        {
            item.material.DOFade(0, fadeOutTime);
            item.gameObject.SetActive(false);
        }

        if (_isTablet)
        {
            uiEasy.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            uiDifficult.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        _uiSongImageOnWaiPan = songInfoRectTransOnWaiPan.gameObject.GetComponentsInChildren<Image>();
        _uiSongTextOnWaiPan = songInfoRectTransOnWaiPan.gameObject.GetComponentsInChildren<Text>();
    }


    void Update()
    {
        if (canPanRotate)
        {
            waiPanTrans.Rotate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnOpeningAnimPlay()
    {
        if (!_isTablet)
        {
            bodyTrans.DOMove(new Vector3(bodyTrans.position.x - 6.3f, bodyTrans.position.y), 2.5f)
                .SetEase(Ease.InOutCubic);
            waiPanTrans.DOMove(new Vector3(waiPanTrans.position.x - 6.3f - 2.9f, waiPanTrans.position.y), 2.5f)
                .SetEase(Ease.InOutCubic);
            waiPanTrans.DOScale(new Vector3(3, 3), 2.5f).SetEase(Ease.InOutCubic);
        }
        else
        {
            bodyTrans.DOMove(new Vector3(bodyTrans.position.x - 4.7f, bodyTrans.position.y), 2.5f)
                .SetEase(Ease.InOutCubic);
            bodyTrans.DOScale(new Vector3(0.83f, 0.83f), 2.5f).SetEase(Ease.InOutCubic);
            waiPanTrans.DOMove(new Vector3(waiPanTrans.position.x - 4.7f - 1.44f, waiPanTrans.position.y), 2.5f)
                .SetEase(Ease.InOutCubic);
            waiPanTrans.DOScale(new Vector3(2.6f, 2.6f), 2.5f).SetEase(Ease.InOutCubic);
        }


        canPanRotate = false;
        float toZ;
        if (!_isTablet)
        {
            if (waiPanTrans.localEulerAngles.z >= 0)
            {
                toZ = waiPanTrans.localEulerAngles.z - waiPanTrans.localEulerAngles.z % 90 + 180;
            }
            else
            {
                toZ = waiPanTrans.localEulerAngles.z - waiPanTrans.localEulerAngles.z % 90 - 180;
            }
        }
        else
        {
            if (waiPanTrans.localEulerAngles.z >= 0)
            {
                toZ = waiPanTrans.localEulerAngles.z - waiPanTrans.localEulerAngles.z % 90 + 135;
            }
            else
            {
                toZ = waiPanTrans.localEulerAngles.z - waiPanTrans.localEulerAngles.z % 90 - 135;
            }
        }


        waiPanTrans.DORotate(new Vector3(0, 0, toZ), 2.5f).SetEase(Ease.InOutCubic).OnComplete(OnBodyReady);
        foreach (var item in selectBG)
        {
            item.DOFade(0.6f, 2.5f);
        }
    }

    private void OnOpeningAnimComplete()
    {
        Debug.Log("Anim ends.");


        uiButtonStart.gameObject.SetActive(false);
        foreach (var item in uiDeco)
        {
            item.gameObject.SetActive(false);
        }

        baiSePanZi.gameObject.SetActive(false);
        foreach (SpriteRenderer item in _chaoGeArray)
        {
            item.gameObject.SetActive(false);
        }

        dy.SetActive(false);
    }

    private void OnBodyReady()
    {
        uiButtonBack.gameObject.SetActive(true);
        uiEasy.gameObject.SetActive(true);
        uiDifficult.gameObject.SetActive(true);
        uiStartOnInnerPlate.gameObject.SetActive(true);
        uiSongInfo.gameObject.SetActive(true);

        uiButtonBack.DOFade(1, fadeInTime);
        uiEasy.DOFade(1, fadeInTime);
        uiDifficult.DOFade(1, fadeInTime);
        uiStartOnInnerPlate.DOFade(1, fadeInTime);

        LoadSongInfo(_songIndex);
    }

    public void LoadSongInfo(int index)
    {
        float maxNailX = float.MinValue;
        Vector3 nailLocalPos;
        Vector3 nailToWaiPan;
        foreach (var item in nailsTrans)
        {
            nailLocalPos = item.localPosition;
            nailToWaiPan = RotateRabbit(nailLocalPos);
            Debug.Log(item.name + "  Lali-ho!  " + nailToWaiPan);
            if (nailToWaiPan.x > maxNailX)
            {
                maxNailX = nailToWaiPan.x;
            }
        }

        songInfoRectTransOnWaiPan.position = new Vector3(
            (maxNailX * nailFatherTrans.localScale.x + nailFatherTrans.localPosition.x) *
            nailGrandFatherTrans.localScale.x + nailGrandFatherTrans.localPosition.x,
            songInfoRectTransOnWaiPan.position.y, 90);

        _uiSongTextOnWaiPan[0].text = SongListInstance.SongList[index].SongRank;
        _uiSongTextOnWaiPan[1].text = SongListInstance.SongList[index].SongCombo;
        songInfoRectTransOnWaiPan.localScale =
            new Vector3(nailGrandFatherTrans.localScale.x, nailGrandFatherTrans.localScale.y, 0);
        foreach (var item in _uiSongImageOnWaiPan)
        {
            item.DOFade(1, fadeInTime);
        }

        foreach (var item in _uiSongTextOnWaiPan)
        {
            item.DOFade(1, fadeInTime);
        }

        uiSongInfo.sprite = Resources.Load<Sprite>(SongListInstance.SongList[index].SongSprite);
        uiSongInfo.DOFade(1, fadeInTime).OnComplete(LoadSongStatusChange);

        _songIndex = index;
        Debug.Log(_songIndex);
    }

    private Vector3 RotateRabbit(Vector3 rawRabbit)
    {
        float carrot = waiPanTrans.localEulerAngles.z / 180 * Mathf.PI;
        Debug.Log(carrot / Mathf.PI * 180);
        Vector3 fatRabbit = new Vector3(
            Mathf.Cos(carrot) * rawRabbit.x - Mathf.Sin(carrot) * rawRabbit.y,
            Mathf.Sin(carrot) * rawRabbit.x + Mathf.Cos(carrot) * rawRabbit.y);
        return fatRabbit;
    }

    public UITouchInSelect UITIS;

    private void LoadSongStatusChange()
    {
        UITIS.isChanging = false;
    }
    //----------------------------------------------------------

    private UITouchInSelect.slideVector _slideDir;


    public void LoadNewSongInfo(UITouchInSelect.slideVector dir)
    {
        _slideDir = dir;
        uiSongInfo.DOFade(0, fadeOutTime).OnComplete(LoadNewSongOnComplete);
        foreach (var item in _uiSongImageOnWaiPan)
        {
            item.DOFade(0, fadeOutTime);
        }

        foreach (var item in _uiSongTextOnWaiPan)
        {
            item.DOFade(0, fadeOutTime);
        }
    }

    private void LoadNewSongOnComplete()
    {
        if (_slideDir == UITouchInSelect.slideVector.up)
        {
            waiPanTrans.DORotate(new Vector3(0, 0, waiPanTrans.localEulerAngles.z + 90), 1.0f).SetEase(Ease.InOutCubic)
                .OnComplete(LoadNewSongInfoOnComplete2);
        }

        if (_slideDir == UITouchInSelect.slideVector.down)
        {
            waiPanTrans.DORotate(new Vector3(0, 0, waiPanTrans.localEulerAngles.z - 90), 1.0f).SetEase(Ease.InOutCubic)
                .OnComplete(LoadNewSongInfoOnComplete2);
        }
    }

    private void LoadNewSongInfoOnComplete2()
    {
        if (_slideDir == UITouchInSelect.slideVector.up)
        {
            if (_songIndex + 1 >= SongListInstance.SongList.Count)
            {
                _songIndex = 0;
                LoadSongInfo(_songIndex);
            }
            else
            {
                _songIndex++;
                LoadSongInfo(_songIndex);
            }
        }

        if (_slideDir == UITouchInSelect.slideVector.down)
        {
            if (_songIndex - 1 < 0)
            {
                _songIndex = SongListInstance.SongList.Count - 1;
                LoadSongInfo(_songIndex);
            }
            else
            {
                _songIndex--;
                LoadSongInfo(_songIndex);
            }
        }
    }


    //----------------------------------------------------------

    public void BackAnim()
    {
        waiPanTrans.DOScale(UIInstance.UiInstance.GetWaiPanScaleInStart(), 1.5f).SetEase(Ease.InOutCubic)
            .OnComplete(BackToHomeScene);
        waiPanTrans.DOMove(UIInstance.UiInstance.GetBodyPosInStart(), 1.5f).SetEase(Ease.InOutCubic);
        waiPanTrans.DORotate(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.InOutCubic);
        bodyTrans.DOMove(UIInstance.UiInstance.GetBodyPosInStart(), 1.5f).SetEase(Ease.InOutCubic);
        bodyTrans.DOScale(UIInstance.UiInstance.GetBodyScaleInStart(), 1.5f).SetEase(Ease.InOutCubic);
        uiButtonBack.DOFade(0, fadeOutTime);
        uiButtonBack.gameObject.GetComponent<Button>().enabled = false;
        uiEasy.DOFade(0, fadeOutTime);
        uiEasy.gameObject.GetComponent<Button>().enabled = false;
        uiDifficult.DOFade(0, fadeOutTime);
        uiDifficult.gameObject.GetComponent<Button>().enabled = false;
        uiStartOnInnerPlate.DOFade(0, fadeOutTime);
        uiStartOnInnerPlate.gameObject.GetComponent<Button>().enabled = false;
        foreach (var item in selectBG)
        {
            item.DOFade(0, fadeOutTime);
        }

        foreach (var item in _uiSongImageOnWaiPan)
        {
            item.DOFade(0, fadeOutTime);
        }

        foreach (var item in _uiSongTextOnWaiPan)
        {
            item.DOFade(0, fadeOutTime);
        }

        uiSongInfo.DOFade(0, fadeOutTime);
    }

    private void BackToHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }
}