using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DLBasic.Common
{
    public class RedPoint : MonoBehaviour
    {
        public RectTransform childs;
        public GameObject childGo;
        public RedPointType redPointType;
        public string redPointKey;

        private void Awake()
        {
            RedPointManager.Instance.RegisterPoint(this);
            InitRedPoint();
        }

        private void InitRedPoint()
        {
            RedPointManager.Instance.SetInit(this);
        }

        public void SetData(bool isshow)
        {
            childGo.SetActive(isshow);
        }

        public void SetData(int count)
        {

        }

        private void OnDisable()
        {
            RedPointManager.Instance.UnRegisterPoint(this);
        }
    }
}
