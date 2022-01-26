using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DLBasic.Common;

public enum FallType : int
{
    Normal,
    EnemySubHp,//减血
    GetHpText,//加血
    DodgeTxt,
    PlayerSubHp,
    Relive

}

public class FlyText : MonoSingletion<FlyText>
{
    private PoolManager<string, FlyTextItem> flyTextPool;
    private List<FlyTextItem> curTextList = new List<FlyTextItem>();

    private void Awake()
    {
        flyTextPool = new PoolManager<string, FlyTextItem>(CreateFun, DestoryFun);
    }

    private void Update()
    {
        for (int i = 0; i < curTextList.Count; i++)
        {
            var item = curTextList[i];
            if (item.isRemove)
            {
                curTextList.RemoveAt(i);
                Release(item);
                i--;
                continue;
            }
            curTextList[i].OnUpdate();
        }
    }

    private void CreateFun(string key, Action<FlyTextItem> callback)
    {
        FlyTextItem bulletItem = new FlyTextItem();
        GameObject go = Resources.Load<GameObject>($"UI/UIWindow/{key}");
        go = Instantiate(go, GameMain.Ins.mainCanvas.transform);
        //FixShader.Register(go);
        bulletItem.SetModel(go);
        bulletItem.testID = key;
        callback(bulletItem);
    }

    private void DestoryFun(string key, FlyTextItem value)
    {
        DestroyImmediate(value.model);
    }

    private void Release(FlyTextItem text)
    {
        text.model.SetActive(false);
        if (text.showTxt) text.showTxt.RestNum();
        flyTextPool.Add(text.testID, text);
    }

    private void ReleaseAll()
    {
        if (Instance == null) return;
        for (int i = 0; i < curTextList.Count; i++)
        {
            Release(curTextList[i]);
        }
        curTextList.Clear();
    }

    //外部清理
    [Button]
    public void ClearPool()
    {
        ReleaseAll();
        flyTextPool.ClearAll();
    }

    //外部调用生成 id同枪id
    public FlyTextItem CreateFallByID(float time, FallType type, int text, Vector3 pos, bool hadSysbol)
    {
        var go = flyTextPool.Get(type.ToString());
        curTextList.Add(go);

        Vector2 mouseDown = GameMain.Ins.mainCamera.WorldToScreenPoint(pos);
        _ = new Vector2();
        Vector2 mouseUGUIPos;
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(GameMain.Ins.mainCanvas.transform as RectTransform, mouseDown, GameMain.Ins.uiCamera, out mouseUGUIPos);
        if (isRect)
        {
            go.model.GetComponent<RectTransform>().anchoredPosition = mouseUGUIPos;
        }
        go.SetData(time, text, type, hadSysbol);

        return go;
    }
    public FlyTextItem CreateEnemyFallByID(float time, FallType type, int text, Transform pos, bool hasSysbol)
    {
        if (!pos) return null;
        var go = flyTextPool.Get(type.ToString());
        curTextList.Add(go);
        Vector2 mouseDown = GameMain.Ins.mainCamera.WorldToScreenPoint(pos.position);
        _ = new Vector2();
        Vector2 mouseUGUIPos;
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(GameMain.Ins.mainCanvas.transform as RectTransform, mouseDown, GameMain.Ins.uiCamera, out mouseUGUIPos);
        if (isRect)
        {
            go.model.GetComponent<RectTransform>().anchoredPosition = mouseUGUIPos;
        }
        go.SetEnemyAnim(time, text, pos, hasSysbol);

        return go;
    }
    //事例代码
    //FlyText.Instance.CreateBulletByID(3, 0, "123456", pos);
}

public class FlyTextItem
{
    public string testID;
    public GameObject model;
    public bool isRemove;
    public float timer;
    public ImageText showTxt;
    public Transform followTran;
    public CanvasGroup canvasGroup;
    public void SetData(float time, int text, FallType fallType, bool hasSysbol)
    {

        model.SetActive(true);
        timer = time;
        isRemove = false;

        if (showTxt != null)
        {
            showTxt.Value(text, hasSysbol);
        }

        Vector3 targetPos = model.transform.localPosition;
        Vector3 olapos = targetPos;
        if (fallType == FallType.DodgeTxt)
        {
            targetPos.y += 50;
        }
        else
        {
            targetPos.x += UnityEngine.Random.Range(-80, 80);
            targetPos.y += UnityEngine.Random.Range(100, 150);
        }

        Sequence ani = DOTween.Sequence();
        ani.Append(model.transform.DOLocalMove(targetPos, 0.3f)).SetEase(Ease.OutBack);
        ani.Join(canvasGroup.DOFade(1, 0.2f));
        ani.Join(model.transform.DOScale(1.4f, 0.4f).OnComplete(() =>
        {
            model.transform.DOScale(1f, 0.2f);
        }));
        ani.AppendInterval(2f);
        ani.Append(model.transform.DOLocalMove(olapos, 0.2f));
        ani.Join(canvasGroup.DOFade(0, 0.2f));
        ani.Join(model.transform.DOScale(0f, 0.2f));

    }
    public void SetEnemyAnim(float time, int text, Transform followPos, bool sysbol)
    {
        followTran = followPos;
        model.SetActive(true);
        timer = time;
        isRemove = false;

        if (showTxt != null)
        {
            showTxt.Value(text, sysbol);
        }
        model.transform.localScale = Vector3.zero;
        Vector3 targetPos = model.transform.localPosition - Vector3.up * 5;
        Sequence ani = DOTween.Sequence();
        ani.Append(model.transform.DOLocalMove(targetPos, time / 4));
        ani.Append(model.transform.DOLocalMove(targetPos, time / 1.5f));
        Sequence ani1 = DOTween.Sequence();
        ani1.Append(model.transform.DOScale(Vector3.one * 2, time / 4));
        ani1.Append(model.transform.DOScale(Vector3.zero, time / 2));
        ani.Play();
    }

    public void SetModel(GameObject go)
    {
        model = go;
        model.SetActive(true);
        showTxt = go.transform.GetChild(0).GetComponent<ImageText>();
        canvasGroup = go.transform.GetChild(0).GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        model.transform.localScale = Vector3.zero;
    }

    public void OnUpdate()
    {
        if (isRemove) return;
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isRemove = true;
            }
        }
        if (followTran)
        {
            Vector2 mouseDown = GameMain.Ins.mainCamera.WorldToScreenPoint(followTran.position);
            _ = new Vector2();
            Vector2 mouseUGUIPos;
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(GameMain.Ins.mainCanvas.transform as RectTransform, mouseDown, GameMain.Ins.uiCamera, out mouseUGUIPos);
            if (isRect)
            {
                var ancPos = model.GetComponent<RectTransform>().anchoredPosition;
                model.GetComponent<RectTransform>().anchoredPosition = new Vector2(mouseUGUIPos.x, ancPos.y);
            }
        }
    }
}