using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
/// <summary>
/// Բ�������ο�����
/// </summary>
public class CircleGuidanceController : MonoBehaviour
{
    private Canvas canvas;

    /// <summary>
    /// Ҫ������ʾ��Ŀ��
    /// </summary>
    public RectTransform target;

    /// <summary>
    /// ����Χ����
    /// </summary>
    private Vector3[] _corners = new Vector3[4];

    /// <summary>
    /// �ο�����Բ��
    /// </summary>
    private Vector4 _center;

    /// <summary>
    /// �ο�����뾶
    /// </summary>
    private float _radius;

    /// <summary>
    /// ���ֲ���
    /// </summary>
    private Material _material;

    /// <summary>
    /// ��ǰ��������İ뾶
    /// </summary>
    private float _currentRadius;

    /// <summary>
    /// �����������ŵĶ���ʱ��
    /// </summary>
    [NonSerialized] public float _shrinkTime = 0.2f;

    /// <summary>
    /// ���������򻭲�����ת��
    /// </summary>
    /// <param name="canvas">����</param>
    /// <param name="world">��������</param>
    /// <returns>���ػ����ϵĶ�ά����</returns>
    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            world, canvas.GetComponent<Camera>(), out position);
        return position;
    }

    [Button]
    public void SetTarget(RectTransform target)
    {
        this.target = target;
        RefreshMask();
    }

    public void RefreshMask()
    {
        canvas = GameMain.Ins.mainCanvas;
        //��ȡ����������ĸ��������������
        target.GetWorldCorners(_corners);
        //�������ո�����ʾ����İ뾶
        _radius = Vector2.Distance(WorldToCanvasPos(canvas, _corners[0]), WorldToCanvasPos(canvas, _corners[2])) / 2f;
        //���������ʾ�����Բ��
        float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
        float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);
        Vector3 centerWorld = new Vector3(x, y, 0);
        Vector2 center = WorldToCanvasPos(canvas, centerWorld);
        //�������ֲ����е�Բ�ı���
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
        _material = GetComponent<Image>().material;
        _material.SetVector("_Center", centerMat);
        //���㵱ǰ������ʾ����İ뾶
        RectTransform canRectTransform = canvas.transform as RectTransform;
        if (canRectTransform != null)
        {
            //��ȡ����������ĸ�����
            canRectTransform.GetWorldCorners(_corners);
            //��������������������������Զ�ľ�����Ϊ��ǰ��������뾶�ĳ�ʼֵ
            foreach (Vector3 corner in _corners)
            {
                _currentRadius = Mathf.Max(Vector3.Distance(WorldToCanvasPos(canvas, corner), center), _currentRadius);
            }
        }
        _material.SetFloat("_Slider", _currentRadius);
    }

    /// <summary>
    /// �����ٶ�
    /// </summary>
    private float _shrinkVelocity = 0f;

    private void Update()
    {
        //�ӵ�ǰ�뾶��Ŀ��뾶��ֵ��ʾ��������
        float value = Mathf.SmoothDamp(_currentRadius, _radius, ref _shrinkVelocity, _shrinkTime);
        if (!Mathf.Approximately(value, _currentRadius))
        {
            _currentRadius = value;
            _material.SetFloat("_Slider", _currentRadius);
        }
    }
}