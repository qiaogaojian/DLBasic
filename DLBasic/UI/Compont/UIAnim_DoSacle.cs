using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnim_DoSacle : MonoBehaviour
{
    public float cd = 5;
    public float duration = 1f;
    public float strength = 0.25f;
    public int vibrato = 8;
    public Vector3 initScale = new Vector3(1, 1, 1);
    float startButtonAnimTime;
    void Update()
    {
        if (Time.time > startButtonAnimTime)
        {
            startButtonAnimTime = Time.time + cd;
            transform.localScale = initScale;
            transform.DOShakeScale(duration, strength, vibrato).From();
        }
    }


}
