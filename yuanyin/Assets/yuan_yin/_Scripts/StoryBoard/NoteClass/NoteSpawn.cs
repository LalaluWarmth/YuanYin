using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class NoteSpawn<K, V> : MonoBehaviour
    where K : class
    where V : Note
{
    private static K _instance;

    public static K GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(K)) as K;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(K)) as K;
            }
        }

        return _instance;
    }

    [Tooltip("事件对应ID")] [EventID] public string eventID;


    //-------------------------引用--------------------------
    public Koreography playingKoreo;

    protected NoteController _noteController;
    //-------------------------对象池--------------------------

    protected Stack<V> notePool = new Stack<V>(); //音符
    public Stack<GameObject> hitEffectObjectPool = new Stack<GameObject>(); //成功特效
    public Stack<GameObject> failEffectObjectPool = new Stack<GameObject>(); //失败特效

    //-------------------------音符--------------------------
    public V NoteObject;
    public V SampleNoteObject;

    public GameObject hitEffectGo;
    protected Animator hitEffectGoAnimator;
    public GameObject SampleHitEffectGo;
    protected AnimationClip[] animationHitClips;

    public GameObject failEffectGo;
    protected Animator failEffectGoAnimator;
    public GameObject SampleFailEffectGo;
    protected AnimationClip[] animationFailClips;

    public Transform storyboard;

    private void Awake()
    {
        _noteController = NoteController.GetInstance();
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0); //获取Koreography对象
        hitEffectGoAnimator = hitEffectGo.GetComponent<Animator>();
        failEffectGoAnimator = failEffectGo.GetComponent<Animator>();
        animationHitClips = hitEffectGoAnimator.runtimeAnimatorController.animationClips;
        animationFailClips = failEffectGoAnimator.runtimeAnimatorController.animationClips;
    }

    #region NoteSpawnLogic

    protected virtual void InitNoteQueue() //初始化音符队列
    {
    }

    protected virtual void CheckNoteSpawnNext() //不断检测是否生成下一个新音符
    {
    }

    #endregion

    #region 音符对象池方法

    //-------------------------音符对象池方法--------------------------
    protected virtual V GetFreshNote() //从对象池中取对象的方法
    {
        return null;
    }

    public virtual void ReturnNoteToPool(V obj) //音符对象回到对象池
    {
        if (obj)
        {
            obj.enabled = false;
            obj.gameObject.SetActive(false);
            notePool.Push(obj);
            // Debug.Log("Lali-ho!");
        }
    }

    #endregion

    #region 特效对象池方法

    //-------------------------成功特效对象池方法--------------------------
    public void GetHitEffect(Vector2 curPos)
    {
        GameObject retObj;
        if (hitEffectObjectPool.Count > 0)
        {
            retObj = hitEffectObjectPool.Pop();
        }
        else
        {
            retObj = Instantiate<GameObject>(hitEffectGo, storyboard);
        }

        retObj.transform.position = curPos;
        retObj.SetActive(true);
        StartCoroutine(DestroyHitEffect(retObj));
    }

    protected void ReturnHitEffectToPool(GameObject hitEffectGOLocal)
    {
        if (hitEffectGOLocal)
        {
            hitEffectGOLocal.SetActive(false);
            hitEffectObjectPool.Push(hitEffectGOLocal);
        }
    }

    IEnumerator DestroyHitEffect(GameObject hitEffectGOLocal)
    {
        yield return new WaitForSeconds(animationHitClips[0].length);
        ReturnHitEffectToPool(hitEffectGOLocal);
    }

    //-------------------------失败特效对象池方法--------------------------

    public void GetFailEffect(Vector2 curPos)
    {
        GameObject retObj;
        if (failEffectObjectPool.Count > 0)
        {
            retObj = failEffectObjectPool.Pop();
        }
        else
        {
            retObj = Instantiate<GameObject>(failEffectGo, storyboard);
        }

        retObj.transform.position = curPos;
        retObj.SetActive(true);
        StartCoroutine(DestroyFailEffect(retObj));
    }

    protected void ReturnFailEffectToPool(GameObject failEffectGOLocal)
    {
        if (failEffectGOLocal)
        {
            failEffectGOLocal.SetActive(false);
            failEffectObjectPool.Push(failEffectGOLocal);
        }
    }

    IEnumerator DestroyFailEffect(GameObject failEffectGOLocal)
    {
        yield return new WaitForSeconds(animationFailClips[0].length);
        ReturnFailEffectToPool(failEffectGOLocal);
    }

    #endregion
}