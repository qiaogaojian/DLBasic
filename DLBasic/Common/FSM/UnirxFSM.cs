using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
public class UnirxFSM<T, T2>
    where T : MonoBehaviour
    where T2 : UnirxFSMCommonDataBase
{
    private readonly Dictionary<string, IUnirxFSM<T>> fsmDict;

    public string lastState { get; private set; }
    public string nowState { get; private set; }
    private readonly T myMono;
    public readonly T2 commonData;
    private UnirxFSM()
    {

    }


    public UnirxFSM(T t, T2 t2)
    {
        nowState = null;
        fsmDict = new Dictionary<string, IUnirxFSM<T>>();

        myMono = t;
        commonData = t2;

        Observable.EveryUpdate()
           .Where(_ => nowState != null)
           .Subscribe(_ =>
           {
               fsmDict[nowState].OnUpdate(t);
           })
           .AddTo(t);

        myMono.OnDestroyAsObservable()
           .Subscribe(_ =>
           {
               if (nowState == null) return;
               fsmDict[nowState].OnExit(myMono);
           })
           .AddTo(t);
    }

    public void AddFSM(IUnirxFSM<T> iUnirxFSM)
    {
        if (fsmDict.ContainsKey(iUnirxFSM.state))
        {
            throw new Exception($"状态重复!{iUnirxFSM.state}");
        }
        fsmDict.Add(iUnirxFSM.state, iUnirxFSM);
    }

    public void SwitchState(string newState)
    {
        //Debug.LogError("CurState:" + newState);
        if (newState == nowState) return;
        if (!fsmDict.ContainsKey(newState)) return;
        if (nowState != null)
        {
            fsmDict[nowState].OnExit(myMono);
        }
        lastState = nowState;
        nowState = newState;
        fsmDict[newState].OnEnter(myMono);
    }

}


