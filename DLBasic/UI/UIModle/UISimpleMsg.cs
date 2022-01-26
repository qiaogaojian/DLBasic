using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using DLBasic.UI;

public class UISimpleMsg : UIBase
{
    public override UIType UIType
    {
        get
        {
            return UIType.LoadingUI;
        }
    }
    public override int CloseType { get { return 1; } }
    public Text TextMsgShow;
    public Transform MainPanel;
    public Image TextBg;
    [HideInInspector]
    public bool moveUp = true;
    [HideInInspector]
    public string msg;
    float animTime = 3f;

    public override void OnClose()
    {
        base.OnClose();
        OnRelease();
    }
    private void OnRelease()
    {
        TextMsgShow.text = "";
        TextMsgShow.rectTransform.sizeDelta = new Vector2(0, TextMsgShow.rectTransform.rect.height);
        MainPanel.localPosition = Vector3.zero;
        moveUp = true;
        msg = "";
        animTime = 3f;
        TextBg.DOFade(1, 0);
        TextMsgShow.DOFade(1, 0);
    }
    public void Init(string msg,  bool isMoveUp = true)
    {
        moveUp = isMoveUp;
        this.msg = msg;
        TextMsgShow.text = msg;
    }

    private void OnEnable()
    {
        if (moveUp)
        {
            MainPanel.DOLocalMoveY(200, 1f).OnComplete(() =>
            {
                Tweener textfade = TextBg.DOFade(0, 1f);
                //textfade.SetDelay(1f);
                Tweener fade = TextMsgShow.DOFade(0, 1f);
                //fade.SetDelay(2f);
                fade.OnComplete(() =>
                {
                    Close();
                });
            });
        }
        else
        {
            TextBg.rectTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            MainPanel.localScale = Vector3.one * .75f;
            Tweener anim = MainPanel.DOScale(1, .25f);
            anim.SetEase(Ease.OutBack);
            anim.OnComplete(() =>
            {
                anim = MainPanel.DOScale(1, 1f);
                anim.OnComplete(() => { Close(); });
            });
        }
    }
}
