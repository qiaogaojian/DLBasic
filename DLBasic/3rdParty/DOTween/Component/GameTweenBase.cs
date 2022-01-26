using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class GameTweenBase : MonoBehaviour
{
    public enum Space
    {
        World = 0,
        Local = 1
    }
    public enum CurveType
    {
        None,
        TweenCurve,
        AnimationCurve
    }

    public float time;
    public float delay;
    public bool playOnAwake = true;
    public bool loop;
    public int loopCount = 1;
    public LoopType loopType;
    public Space space = Space.Local;
    public bool itsLocation = true;
    public Vector3 startValue;
    public Vector3 endValue;
    public CurveType curveType = CurveType.None;
    public Ease tweenCurve;
    public AnimationCurve animCurve;
    public Tweener anim;
    public UnityEvent onStart;
    public UnityEvent onUpdate;
    public UnityEvent onComplete;

    private void OnEnable()
    {
        SetTweener();
        if (anim == null) return;
        if (!playOnAwake) anim.Pause();
    }

    private void SetTweener()
    {
        InitTweener();
        if (anim == null) return;
        if (delay != 0) anim.SetDelay(delay);
        if (curveType == GameTweenBase.CurveType.AnimationCurve) anim.SetEase(animCurve);
        else if (curveType == GameTweenBase.CurveType.TweenCurve) anim.SetEase(tweenCurve);
        if (loop) { if (loopCount == 0) loopCount = -1; anim.SetLoops(loopCount, loopType); }

        if (onStart != null) onStart.Invoke();
        if (onUpdate != null) anim.OnUpdate(() => { onUpdate.Invoke(); });
        if (onComplete != null) anim.OnComplete(() => { onComplete.Invoke(); anim = null; });
    }

    protected virtual void InitTweener() { }

    public void Play()
    {
        if (anim == null) SetTweener();
        else if (onStart != null) onStart.Invoke();
        if (anim == null) return;

        anim.PlayForward();
    }

    public void Stop()
    {
        if (anim != null && anim.IsPlaying()) anim.Kill();
    }
    public void OnDisable()
    {
        if (anim != null)
        {
            anim.Kill();
        }
    }
}

public static class TweenExtensions
{
    public static GameTweenBase GetTween(this Transform target)
    {
        return target.GetComponent<GameTweenBase>();
    }
    public static GameTweenBase[] GetTweens(this Transform target)
    {
        return target.GetComponents<GameTweenBase>();
    }
}
