using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����������
/// </summary>
public class RectGuidanceController : MonoBehaviour
{
    //��ȡ����
    Canvas canvas;

    /// <summary>
    /// ������ʾ��Ŀ��
    /// </summary>
    [HideInInspector] public RectTransform target;

    /// <summary>
    /// ����Χ����
    /// </summary>
    private Vector3[] _corners = new Vector3[4];

    /// <summary>
    /// �ο���������
    /// </summary>
    private Vector4 _center;

    /// <summary>
    /// ���յ�ƫ��ֵX
    /// </summary>
    private float _targetOffsetX = 0f;

    /// <summary>
    /// ���յ�ƫ��ֵY
    /// </summary>
    private float _targetOffsetY = 0f;

    /// <summary>
    /// ���ֲ���
    /// </summary>
    public Material _material;

    /// <summary>
    /// ��ǰ��ƫ��ֵX
    /// </summary>
    private float _currentOffsetX = 0f;

    /// <summary>
    /// ��ǰ��ƫ��ֵY
    /// </summary>
    private float _currentOffsetY = 0f;

    /// <summary>
    /// ��������ʱ��
    /// </summary>
    [NonSerialized] public float _shrinkTime = 0.5f;


    /// <summary>
    /// �������굽���������ת��
    /// </summary>
    /// <param name="canvas">����</param>
    /// <param name="world">��������</param>
    /// <returns>ת�����ڻ���������</returns>
    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world,
            canvas.GetComponent<Camera>(), out position);
        return position;
    }

    /// <summary>
    /// ˢ�±仯mask
    /// </summary>
    /// <param name="offect">�޸�ƫ��</param>
    [Button]
    public void SetTarget(RectTransform target, Vector2 offect, float time)
    {
        _shrinkTime = time;
        this.target = target;
        RefreshMask(offect);
    }



    public void HideMask()
    {
        _material.SetFloat("_Slider", 1500);
    }


    private void RefreshMask(Vector2 offect)
    {
        canvas = GameMain.Ins.mainCanvas;
        //��ȡ���������ĸ��������������
        target.GetWorldCorners(_corners);

        //����ƫ��
        _corners[0] = new Vector3(_corners[0].x - offect.x * 0.5f, _corners[0].y - offect.y * 0.5f, _corners[0].z);
        _corners[1] = new Vector3(_corners[1].x - offect.x * 0.5f, _corners[1].y + offect.y * 0.5f, _corners[1].z);
        _corners[2] = new Vector3(_corners[2].x + offect.x * 0.5f, _corners[2].y + offect.y * 0.5f, _corners[2].z);
        _corners[3] = new Vector3(_corners[3].x + offect.x * 0.5f, _corners[3].y - offect.y * 0.5f, _corners[3].z);


        //���������ʾ����զ�����еķ�Χ
        _targetOffsetX = Vector2.Distance(WorldToCanvasPos(canvas, _corners[0]), WorldToCanvasPos(canvas, _corners[3])) / 2f;
        _targetOffsetY = Vector2.Distance(WorldToCanvasPos(canvas, _corners[0]), WorldToCanvasPos(canvas, _corners[1])) / 2f;
        //���������ʾ���������
        float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
        float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);
        Vector3 centerWorld = new Vector3(x, y, 0);
        Vector2 center = WorldToCanvasPos(canvas, centerWorld);

        //�������ֲ��������ı���
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
        _material.SetVector("_Center", centerMat);
        //���㵱ǰƫ�Ƶĳ�ʼֵ
        RectTransform canvasRectTransform = (canvas.transform as RectTransform);
        if (canvasRectTransform != null)
        {
            //��ȡ����������ĸ�����
            canvasRectTransform.GetWorldCorners(_corners);
            //��ƫ�Ƴ�ʼֵ
            for (int i = 0; i < _corners.Length; i++)
            {
                if (i % 2 == 0)
                    _currentOffsetX = Mathf.Max(Vector3.Distance(WorldToCanvasPos(canvas, _corners[i]), center), _currentOffsetX);
                else
                    _currentOffsetY = Mathf.Max(Vector3.Distance(WorldToCanvasPos(canvas, _corners[i]), center), _currentOffsetY);
            }
        }
        //�������ֲ����е�ǰƫ�Ƶı���
        _material.SetFloat("_SliderX", _currentOffsetX);
        _material.SetFloat("_SliderY", _currentOffsetY);
    }

    private float _shrinkVelocityX = 0f;
    private float _shrinkVelocityY = 0f;

    private void Update()
    {
        //�ӵ�ǰƫ��ֵ��Ŀ��ƫ��ֵ��ֵ��ʾ��������
        float valueX = Mathf.SmoothDamp(_currentOffsetX, _targetOffsetX, ref _shrinkVelocityX, _shrinkTime);
        float valueY = Mathf.SmoothDamp(_currentOffsetY, _targetOffsetY, ref _shrinkVelocityY, _shrinkTime);
        if (!Mathf.Approximately(valueX, _currentOffsetX))
        {
            _currentOffsetX = valueX;
            _material.SetFloat("_SliderX", _currentOffsetX);
        }

        if (!Mathf.Approximately(valueY, _currentOffsetY))
        {
            _currentOffsetY = valueY;
            _material.SetFloat("_SliderY", _currentOffsetY);
        }
    }

    public void UpdatePos()
    {

    }
}