using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DLBasic.UI
{
    public class UIPopUp : UIBase
    {
        Transform panel;
        protected virtual void Start()
        {
            panel = transform.Find("MainPanel");
            if (panel != null)
            {
                panel.transform.localScale = Vector3.zero;
                panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(OnShowFinish);
            }
        }

        protected virtual void OnShowFinish()
        {

        }

        public void ClosePopup()
        {
            if (panel != null)
            {
                panel.transform.DOScale(Vector3.zero, 0.2f).OnComplete(Close);
            }
        }
    }

    public class UIJumpDown : UIBase
    {
        protected virtual void Start()
        {
            Transform panel = transform.Find("MainPanel");
            if (panel != null)
            {
                Vector3 pos = panel.transform.localPosition;
                panel.transform.localPosition = Vector3.up * 500;
                panel.transform.DOLocalMove(pos, 0.6f).SetEase(Ease.OutBounce).OnComplete(OnShowFinish);
            }

        }

        protected virtual void OnShowFinish()
        {

        }
    }
}
