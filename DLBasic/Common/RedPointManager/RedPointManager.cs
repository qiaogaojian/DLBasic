using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DLBasic.Common
{
    public enum RedPointType
    {
        normal,//正常红点
        numRedPoint//数字红点
    }

    public class RedPointManager : MonoSingletion<RedPointManager>
    {
        private Dictionary<string, RedPoint> redPos = new Dictionary<string, RedPoint>();

        public Dictionary<string, bool> redStateDic = new Dictionary<string, bool>();

        public void RegisterPoint(RedPoint redPoint)
        {
            if (!redPos.ContainsKey(redPoint.redPointKey))
            {
                redPos.Add(redPoint.redPointKey, redPoint);
            }
        }

        public void UnRegisterPoint(RedPoint redPoint)
        {
            if (redPos.ContainsKey(redPoint.redPointKey))
            {
                redPos.Remove(redPoint.redPointKey);
                return;
            }
        }

        //普通红点显示不显示
        public void ChangeRedPointState(string redPintKey, bool isShow)
        {
            if (redPos.ContainsKey(redPintKey))
            {
                redPos[redPintKey].SetData(isShow);
            }

            if (redStateDic.ContainsKey(redPintKey))
            {
                redStateDic[redPintKey] = isShow;
            }
            else
            {
                redStateDic.Add(redPintKey, isShow);
            }
        }


        public void SetInit(RedPoint redPintKey)
        {
            if (redStateDic.ContainsKey(redPintKey.redPointKey))
            {
                redPintKey.SetData(redStateDic[redPintKey.redPointKey]);
                return;
            }
        }

        //带数字红点，显示多少数字，0不显示红点
        public void ChangeRedPointState(string redPintKey, int count)
        {

        }
    }
}