using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class JoysticksPanel : MonoBehaviour
{
    public bool isMove;
    private Vector3 oldPos;

    public float Angle { set; get; }

    public bool isEnter;

    public RectTransform handlerbarRect;//最小的
    public RectTransform barRect;//中间
    public RectTransform rectTransform;//外边
    public RectTransform boundaryImage;//边界

    public CanvasGroup canvasGrop;
    //半径
    private float radius;

    //对外事件
    public Action<bool> BeginJoysticks;

    public Action<float> UpdateAngle;
    public Action<Vector2> UpdateDir;

    public Action<PointerEventData> OnPointEnter;

    private RectTransform canvasRect;

    private float yScreenHRadio;
    private float xSreenWRadio;

    float offect = 80;


    private void Start()
    {
        radius = rectTransform.sizeDelta.x * 0.5f - offect;
        oldPos = rectTransform.anchoredPosition;

        canvasRect = GameMain.Ins.mainCanvas.GetComponent<RectTransform>();

        xSreenWRadio = canvasRect.sizeDelta.x * 1.00f / Camera.main.pixelWidth * 1.00f;
        yScreenHRadio = canvasRect.sizeDelta.y * 1.00f / Camera.main.pixelHeight * 1.00f;
    }

    private void OnEnable()
    {
        if (isMove)
        {
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnBeginDragEvent += OnParentPanelPointEnter;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnDragEvent += OnDrag;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnEndDragEvent += OnEndDrag;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnPointerClickEvent += OnOnPointerClick;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnPointerDownEvent += OnPointerDownEvent;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnPointerEventUpEvent += OnPointerUPEvent;
        }
        else
        {
            Event_OnUnityEvent.Add(this.gameObject).OnPointerClickEvent += OnOnPointerClick;
            Event_OnUnityEvent.Add(this.gameObject).OnBeginDragEvent += OnBeginDragEvent;
            Event_OnUnityEvent.Add(this.gameObject).OnDragEvent += OnDrag;
            Event_OnUnityEvent.Add(this.gameObject).OnEndDragEvent += OnEndDrag;
            Event_OnUnityEvent.Add(this.gameObject).OnPointerDownEvent += OnPointerDownEvent;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnPointerEventUpEvent += OnPointerUPEvent;
        }
    }

    private void OnPointerUPEvent(PointerEventData arg0)
    {
        canvasGrop.alpha = 0;
    }

    Vector3 OriginalPos;
    private void OnPointerDownEvent(PointerEventData arg0)
    {
        OriginalPos = Input.mousePosition;
        Vector2 from = ((Vector2)handlerbarRect.localPosition - (Vector2)rectTransform.localPosition).normalized;
        Vector2 to = Vector2.up;

        //移动handlerbar 并计算其与圆心的角度s
        float angle = Vector2.Angle(from, to);
        angle = Vector3.Cross(from, to).z > 0 ? angle : (-(angle - 180)) + 180;
        //Vector3 dir = to - from;
        //Debug.LogError(from);
        TransfomToUguiPos(handlerbarRect);

        //计算距离移动bar与圆心的距离 加入限定
        float dis = Vector3.Distance(handlerbarRect.anchoredPosition, rectTransform.anchoredPosition);
        dis = dis >= radius ? radius : dis;


        //更新展示的bar位置
        barRect.anchoredPosition = rectTransform.anchoredPosition + new Vector2(dis * Mathf.Sin(Mathf.Deg2Rad * angle), dis * Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    private void OnOnPointerClick(PointerEventData obj)
    {
        OnPointEnter?.Invoke(obj);
    }

    private void OnDisable()
    {
        if (isMove)
        {
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnBeginDragEvent -= OnParentPanelPointEnter;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnDragEvent -= OnDrag;
            Event_OnUnityEvent.Add(transform.parent.gameObject).OnEndDragEvent -= OnEndDrag;
        }
        else
        {
            Event_OnUnityEvent.Add(this.gameObject).OnBeginDragEvent -= OnBeginDragEvent;
            Event_OnUnityEvent.Add(this.gameObject).OnDragEvent -= OnDrag;
            Event_OnUnityEvent.Add(this.gameObject).OnEndDragEvent -= OnEndDrag;
        }
    }

    private void OnBeginDragEvent(PointerEventData arg0)
    {
        BeginJoysticks?.Invoke(true);
    }
    public Action<float> UpdageDragDis;
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset = Input.mousePosition - OriginalPos;
        #region Bai
        //Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 from = ((Vector2)handlerbarRect.localPosition - (Vector2)rectTransform.localPosition).normalized;
        Vector2 to = Vector2.up;

        //移动handlerbar 并计算其与圆心的角度s
        float angle = Vector2.Angle(from, to);
        angle = Vector3.Cross(from, to).z > 0 ? angle : (-(angle - 180)) + 180;
        //Vector3 dir = to - from;
        //Debug.LogError(from);
        TransfomToUguiPos(handlerbarRect);

        ////计算距离移动bar与圆心的距离 加入限定
        float dis = Vector3.Distance(OriginalPos, Input.mousePosition);
        dis = dis >= radius ? radius : dis;

        float dragRate = dis / (radius * 0.5f);//拖动比例
        dragRate = Mathf.Clamp(dragRate, 0, 1);
        //更新展示的bar位置
        barRect.anchoredPosition = rectTransform.anchoredPosition + new Vector2(dis * Mathf.Sin(Mathf.Deg2Rad * angle), dis * Mathf.Cos(Mathf.Deg2Rad * angle));

        #endregion
        //委托自定义入口
        UpdateDir?.Invoke(offset.normalized);

        Vector3 V3 = new Vector3(0, 0, GetUnityDirection(rectTransform.anchoredPosition, barRect.anchoredPosition) - 90);
        boundaryImage.localEulerAngles = V3;

        boundaryImage.gameObject.SetActive(Vector2.Distance(rectTransform.anchoredPosition, barRect.anchoredPosition) * 1.2f >= radius);
        UpdageDragDis?.Invoke(dragRate);
    }
    /// <summary>
    /// -180-180 转为 0-360
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public float Angle180To360(float angle)
    {
        if (angle >= 0 && angle <= 180)
            return angle;
        else
            return 360 + angle;
    }

    Vector2 p;
    public float PointToAngle(Vector2 p1, Vector2 p2)
    {
        p.x = p2.x - p1.x;
        p.y = p2.y - p1.y;
        return Mathf.Atan2(p.y, p.x) * 180 / Mathf.PI;
    }

    /// <summary>
    /// 返回Unity的角度
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public float GetUnityDirection(Vector2 p1, Vector2 p2)
    {
        float angle = Angle180To360(PointToAngle(p1, p2));
        float temp = 360 * 0.0027f;//分为8个方向
        float dir = 0;
        for (int i = 0; i < 360; i++)
        {
            if (angle >= (i * temp) - (temp * 0.5f) && angle < (i * temp) + (temp * 0.5f))
            {
                dir = i * temp;
                break;
            }
        }
        return dir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGrop.alpha = 0;
        handlerbarRect.transform.localPosition = Vector3.zero;
        barRect.transform.localPosition = Vector3.zero;

        if (isMove)
        {
            rectTransform.anchoredPosition = oldPos;
            handlerbarRect.anchoredPosition = oldPos;
            barRect.anchoredPosition = oldPos;
        }
        isEnter = false;

        BeginJoysticks?.Invoke(isEnter);
    }

    public void OnParentPanelPointEnter(PointerEventData eventData)
    {
        if (isMove) TransfomToUguiPos(rectTransform);
        //委托自定义入口
        isEnter = true;
        BeginJoysticks?.Invoke(isEnter);

        canvasGrop.alpha = 1;
    }

    private void TransfomToUguiPos(RectTransform tagret)
    {
        tagret.anchoredPosition = new Vector2(Input.mousePosition.x * xSreenWRadio, Input.mousePosition.y * yScreenHRadio);
    }
}