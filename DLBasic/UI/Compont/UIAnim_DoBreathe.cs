using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnim_DoBreathe : MonoBehaviour
{
    private Transform m_Tran;
    public Vector3 bigScale = Vector3.one * 1.2f;
    public Vector3 smallScale = Vector3.one * 0.8f;
    public float speed = 2;
    private void Start()
    {
        m_Tran = transform;
        DoBig();
    }
    public void DoBig()
    {
        m_Tran.DOScale(bigScale, speed).OnComplete(DoSmall);
    }
    public void DoSmall()
    {
        m_Tran.DOScale(smallScale, speed).OnComplete(DoBig);
    }
}
