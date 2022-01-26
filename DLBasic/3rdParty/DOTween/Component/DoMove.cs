using UnityEngine;
using DG.Tweening;
public class DoMove : GameTweenBase
{
    protected override void InitTweener()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (!rect) return;
        if (!itsLocation) onStart.AddListener(() => { rect.anchoredPosition3D = this.startValue; });
        if (space == GameTweenBase.Space.Local) anim = rect.DOLocalMove(endValue, time);
        else if (space == GameTweenBase.Space.World) rect.DOMove(endValue, time);
    }
}
