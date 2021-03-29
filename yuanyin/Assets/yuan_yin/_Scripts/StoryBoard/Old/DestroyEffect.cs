using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public GameController gameController;
    public bool isHitted;
    public float animationTime;

    private void OnEnable()
    {
        Invoke("ReturnToPool",animationTime);
    }

    private void ReturnToPool()
    {
        if (isHitted)
        {
            gameController.ReturnEffectToPool(gameObject,gameController.hitEffectObjectPool);
            gameObject.SetActive(false);
        }
        else
        {
            gameController.ReturnEffectToPool(gameObject,gameController.downEffectObjectPool);
            gameObject.SetActive(false);
        }
    }
}
