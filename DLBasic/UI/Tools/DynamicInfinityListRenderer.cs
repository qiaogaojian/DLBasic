using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
/// <summary>
/// 动态无限列表
/// @panhaijie
/// 2016年3月22日 10:27:51
/// </summary>
public class DynamicInfinityListRenderer : MonoBehaviour
{
    public struct Size
    {
        public Size(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y;
    }

    public enum LimitType
    {
        Horizontal = 0,
        Vertical = 1,
        None = 2
    }

    /// <summary>
    /// 单元格尺寸（宽，高）
    /// </summary>
    public Vector2 CellSize;
    /// <summary>
    /// 单元格间隙（水平，垂直）
    /// </summary>
    public Vector2 SpacingSize;
    /// <summary>
    ///限制类型
    /// </summary>  
    public LimitType limitType = LimitType.None;
    /// <summary>
    ///限制行数
    /// </summary>
    public int LimitCount;
    /// <summary>
    /// 单元格渲染器prefab
    /// </summary>
    public GameObject RenderGO;
    /// <summary>
    /// 渲染格子数
    /// </summary>
    protected int mRendererCount;
    /// <summary>
    /// 父节点蒙版尺寸
    /// </summary>
    private Vector2 mMaskSize;
    /// <summary>
    /// 蒙版矩形
    /// </summary>
    private Rect mRectMask;
    protected ScrollRect mScrollRect;
    /// <summary>
    /// 转换器
    /// </summary>
    protected RectTransform mRectTransformContainer;
    /// <summary>
    /// 渲染脚本集合
    /// </summary>
    public List<DynamicInfinityItem> mList_items;
    /// <summary>
    /// 渲染格子字典
    /// </summary>
    private Dictionary<int, DynamicRect> mDict_dRect;
    /// <summary>
    /// 数据提供者
    /// </summary>
    protected IList mDataProviders;
    public bool mHasInited = false;
    /// <summary>
    /// 数据在横竖的数量
    /// </summary>
    private Size mSize;

    public void UpdateAllRenderer()
    {
        for (int i = 0; i < mList_items.Count; i++)
        {
            mList_items[i].CallRenderer();
        }
    }
    /// <summary>
    /// 初始化渲染脚本
    /// </summary>
    public virtual void InitRendererList(DynamicInfinityItem.OnSelect OnSelect = null, DynamicInfinityItem.OnUpdateData OnUpdate = null)
    {
        if (mHasInited) return;
        //转换器
        mRectTransformContainer = transform as RectTransform;
        //获得蒙版尺寸
        mMaskSize = transform.parent.GetComponent<RectTransform>().sizeDelta;
        mScrollRect = transform.parent.GetComponent<ScrollRect>();
        mRendererCount = GetCreatItemCount();
        _UpdateDynmicRects(mRendererCount);
        mList_items = new List<DynamicInfinityItem>();
        for (int i = 0; i < mRendererCount; ++i)
        {
            GameObject child = GameObject.Instantiate(RenderGO);
            child.transform.SetParent(transform);
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            child.layer = gameObject.layer;
            child.SetActive(true);
            DynamicInfinityItem dfItem = child.GetComponent<DynamicInfinityItem>();
            if (dfItem == null)
                throw new Exception("Render must extend DynamicInfinityItem");
            mList_items.Add(dfItem);
            mList_items[i].DRect = mDict_dRect[i];
            mList_items[i].OnSelectHandler = OnSelect;
            mList_items[i].OnUpdateDataHandler = OnUpdate;

            //TODO
            //child.SetActive(false);
            _UpdateChildTransformPos(child, i);
        }
        mHasInited = true;
    }

    //通过蒙版尺寸和格子尺寸计算需要的渲染器个数
    private int GetCreatItemCount()
    {
        int count = 0;
        LimitType type = GetLimiteType();

        int perCount = 0;
        switch (type)
        {
            case LimitType.Horizontal:
                perCount = Mathf.CeilToInt(mMaskSize.x / GetBlockSizeX()) + 1;
                break;
            case LimitType.Vertical:
                perCount = Mathf.CeilToInt(mMaskSize.y / GetBlockSizeY()) + 1;
                break;
            default:
                break;
        }
        count = LimitCount * perCount;

        return count;
    }

    private LimitType GetLimiteType()
    {
        if (limitType == LimitType.None)
        {

            if (mScrollRect.horizontal)
            {
                limitType = LimitType.Horizontal;
            }
            else if (mScrollRect.vertical)
            {
                limitType = LimitType.Vertical;
            }
            else
            {
                limitType = LimitType.Horizontal;
            }
        }
        return limitType;
    }
    /// <summary>
    /// 设置渲染列表的尺寸
    /// 不需要public
    /// </summary>
    /// <param name="count"></param>
    void _SetListRenderSize()
    {
        mRectTransformContainer.sizeDelta = new Vector2(mSize.x * GetBlockSizeX(), mSize.y * GetBlockSizeY());
        mRectMask = new Rect(0, -mMaskSize.y, mMaskSize.x, mMaskSize.y);
        bool vertical = mRectTransformContainer.sizeDelta.y > mMaskSize.y;
        bool horizontal = mRectTransformContainer.sizeDelta.x > mMaskSize.x;

        if (mScrollRect.vertical != vertical)
        {
            if (!vertical) mRectTransformContainer.DOAnchorPos(Vector2.zero, 0.25f);
            mScrollRect.vertical = vertical;
        }
        if (mScrollRect.horizontal != horizontal)
        {
            if (!horizontal) mRectTransformContainer.DOAnchorPos(Vector2.zero, 0.25f);
            mScrollRect.horizontal = horizontal;
        }
    }

    /// <summary>
    /// 更新各个渲染格子的位置
    /// </summary>
    /// <param name="child"></param>
    /// <param name="index"></param>
    void _UpdateChildTransformPos(GameObject child, int index)
    {
        DynamicRect rect = mDict_dRect[index];
        ((RectTransform)child.transform).anchoredPosition3D = Vector3.zero;
        ((RectTransform)child.transform).anchoredPosition = rect.Pos();
    }

    /// <summary>
    /// 获得格子块尺寸
    /// </summary>
    /// <returns></returns>
    protected float GetBlockSizeY() { return CellSize.y + SpacingSize.y; }
    protected float GetBlockSizeX() { return CellSize.x + SpacingSize.x; }

    /// <summary>
    /// 更新动态渲染格
    /// 不需要public
    /// </summary>
    /// <param name="count"></param>
    void _UpdateDynmicRects(int count)
    {
        //刷新size
        LimitType type = GetLimiteType();
        switch (type)
        {
            case LimitType.Horizontal:
                mSize = new Size(Mathf.CeilToInt(((float)count) / LimitCount), LimitCount);
                break;
            case LimitType.Vertical:
                mSize = new Size(LimitCount, Mathf.CeilToInt(((float)count) / LimitCount));
                break;
            default:
                break;
        }

        _SetListRenderSize();

        mDict_dRect = new Dictionary<int, DynamicRect>();
        for (int i = 0; i < count; ++i)
        {
            int row = i / mSize.x;
            int column = i % mSize.x;
            DynamicRect dRect = new DynamicRect(column * GetBlockSizeX(), -row * GetBlockSizeY() - CellSize.y, CellSize.x, CellSize.y, i);
            mDict_dRect[i] = dRect;
        }
    }

    /// <summary>
    /// 设置数据提供者
    /// </summary>
    /// <param name="datas"></param>
    public void SetDataProvider(IList datas)
    {
        _UpdateDynmicRects(datas.Count);
        mDataProviders = datas;
        ClearAllListRenderDr();
    }

    /// <summary>
    /// 清理可复用渲染格
    /// 不需要public
    /// </summary>
    void ClearAllListRenderDr()
    {
        if (mList_items != null)
        {
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicInfinityItem item = mList_items[i];
                item.DRect = null;
            }
        }
    }

    /// <summary>
    /// 获得数据提供者
    /// </summary>
    /// <returns></returns>
    public IList GetDataProvider() { return mDataProviders; }

    /// <summary>
    /// 数据发生变化 供外部调用刷新列表
    /// </summary>
    [ContextMenu("RefreshDataProvider")]
    public void RefreshDataProvider()
    {
        if (mDataProviders == null)
            throw new Exception("dataProviders 为空！请先使用SetDataProvider ");
        _UpdateDynmicRects(mDataProviders.Count);
        ClearAllListRenderDr();
    }

    #region 移动至数据
    /// <summary>
    /// 移动列表使之能定位到给定数据的位置上
    /// </summary>
    /// <param name="target"></param>
    public virtual void LocateRenderItemAtTarget(object target, float delay, LimitType limitType = LimitType.None, bool isAgainst = false)
    {
        LocateRenderItemAtIndex(mDataProviders.IndexOf(target), delay);
    }
    public virtual void LocateRenderItemAtIndex(int index, float delay, LimitType limitType = LimitType.None, bool isAgainst = false)
    {
        if (index < 0 || index > mDataProviders.Count - 1) return;

        index = Math.Min(index, mDataProviders.Count - mRendererCount + 2);
        index = Math.Max(0, index);
        Vector2 pos = mRectTransformContainer.anchoredPosition;
        int row = index / LimitCount;
        Vector2 v2Pos = new Vector2(pos.x, row * GetBlockSizeY());
        switch (limitType)
        {
            case LimitType.Horizontal:
                v2Pos = new Vector2(row * GetBlockSizeX() * (isAgainst ? -1 : 1), pos.y);
                break;
            case LimitType.Vertical:
                v2Pos = new Vector2(pos.x, row * GetBlockSizeY() * (isAgainst ? -1 : 1));
                break;
            default:
                break;
        }
        if (!gameObject.activeInHierarchy)
        {
            mRectTransformContainer.anchoredPosition = v2Pos;
        }
        else
        {
            m_Coroutine = StartCoroutine(TweenMoveToPos(pos, v2Pos, delay));
            mScrollRect.StopMovement();
        }
    }
    protected IEnumerator TweenMoveToPos(Vector2 pos, Vector2 v2Pos, float delay)
    {
        bool running = true;
        float passedTime = 0f;
        while (running)
        {
            yield return new WaitForEndOfFrame();
            passedTime += Time.deltaTime;
            Vector2 vCur;
            if (passedTime >= delay)
            {
                vCur = v2Pos;
                running = false;
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            else
            {
                vCur = Vector2.Lerp(pos, v2Pos, passedTime / delay);
            }
            mRectTransformContainer.anchoredPosition = vCur;
        }

    }
    protected Coroutine m_Coroutine = null;
    #endregion

    public void UpdateRender(bool force = false)
    {
        if (mRectTransformContainer == null) return;
        mRectMask.y = -mMaskSize.y - mRectTransformContainer.anchoredPosition.y;
        mRectMask.x = -mRectTransformContainer.anchoredPosition.x;
        Dictionary<int, DynamicRect> inOverlaps = new Dictionary<int, DynamicRect>();
        foreach (DynamicRect dR in mDict_dRect.Values)
        {
            if (dR.Overlaps(mRectMask))
            {
                inOverlaps.Add(dR.Index, dR);
            }
        }
        int len = mList_items.Count;
        for (int i = 0; i < len; ++i)
        {
            DynamicInfinityItem item = mList_items[i];
            if (force || item.DRect != null && !inOverlaps.ContainsKey(item.DRect.Index))
                item.DRect = null;
        }
        foreach (DynamicRect dR in inOverlaps.Values)
        {
            if (GetDynmicItem(dR) == null)
            {
                DynamicInfinityItem item = GetNullDynmicItem();
                if (item == null)
                {
                    continue;
                }
                item.DRect = dR;
                _UpdateChildTransformPos(item.gameObject, dR.Index);

                if (mDataProviders != null && dR.Index < mDataProviders.Count)
                {
                    item.SetData(mDataProviders[dR.Index]);
                }
            }
        }
    }

    /// <summary>
    /// 获得待渲染的渲染器
    /// </summary>
    /// <returns></returns>
    DynamicInfinityItem GetNullDynmicItem()
    {
        int len = mList_items.Count;
        for (int i = 0; i < len; ++i)
        {
            DynamicInfinityItem item = mList_items[i];
            if (item.DRect == null)
                return item;
        }
        Debug.Log("Error");
        return null;

    }

    /// <summary>
    /// 通过动态格子获得动态渲染器
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    DynamicInfinityItem GetDynmicItem(DynamicRect rect)
    {
        int len = mList_items.Count;
        for (int i = 0; i < len; ++i)
        {
            DynamicInfinityItem item = mList_items[i];
            if (item.DRect == null)
                continue;
            if (rect.Index == item.DRect.Index)
                return item;
        }
        return null;
    }

    void Update()
    {
        if (mHasInited)
            UpdateRender();
    }
}
