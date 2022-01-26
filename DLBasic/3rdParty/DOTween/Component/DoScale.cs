using UnityEngine;
using DG.Tweening;

public class DoScale : GameTweenBase
{
    protected override void InitTweener()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (!rect) return;
        if (!itsLocation) onStart.AddListener(() => { rect.localScale = this.startValue; });
        anim = rect.DOScale(endValue, time);
    }
}
