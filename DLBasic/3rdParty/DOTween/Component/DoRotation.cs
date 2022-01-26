using UnityEngine;
using DG.Tweening;
public class DoRotation : GameTweenBase
{
    protected override void InitTweener()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (!rect) return;
        if (!itsLocation) onStart.AddListener(() => { rect.rotation = Quaternion.Euler(this.startValue); });
        if (space == GameTweenBase.Space.Local) anim = rect.DORotate(endValue, time, RotateMode.LocalAxisAdd);
        else if (space == GameTweenBase.Space.World) anim = rect.DORotate(endValue, time);
    }
}
