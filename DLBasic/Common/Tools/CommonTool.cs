using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace DLBasic.Common
{
    public static class CommonTool
    {
        #region  信息类
        /// <summary>
        /// Uint32 IP地址
        /// </summary>
        /// <returns></returns>
        public static UInt32 LocalIPUInt()
        {
            IPAddress ipAddr = null;
            IPAddress[] arrIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIP)
            {
                if (System.Net.Sockets.AddressFamily.InterNetwork.Equals(ip.AddressFamily))
                {
                    ipAddr = ip;
                    break;
                }
            }
            return BitConverter.ToUInt32(ipAddr.GetAddressBytes(), 0);

        }

        /// <summary>
        /// IP地址转字符串输出
        /// </summary>
        /// <returns></returns>
        public static string LocalIPStr()
        {
            IPAddress ipAddr = null;
            IPAddress[] arrIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIP)
            {
                if (System.Net.Sockets.AddressFamily.InterNetwork.Equals(ip.AddressFamily))
                {
                    ipAddr = ip;
                    break;
                }
            }
            return ipAddr.ToString();
        }

        /// <summary>
        /// 系统版本
        /// </summary>
        /// <returns></returns>
        public static string GetOsVersion()
        {
            string OsVersion = SystemInfo.operatingSystem;
            return OsVersion;
        }

        /// <summary>
        /// 获取设备型号
        /// </summary>
        /// <returns></returns>
        public static string CurrentDeviceModel()
        {
            return SystemInfo.deviceModel;  // hardware strings
        }


        static string __MachineID = null;
        /// <summary>
        /// 获取机器码
        /// </summary>
        /// <returns></returns>
        public static string GetMachineID()
        {
            if (__MachineID == null)
            {
                __MachineID = GetMD5String(SystemInfo.deviceUniqueIdentifier);
            }
            return __MachineID;
        }

        /// <summary>
        /// 获取版本号的最后一位 
        /// </summary>
        /// <returns></returns>
        public static int GetVersionLast()
        {
            var stri = Application.version;
            var array = stri.Split('.');
            return int.Parse(array[array.Length - 1]);
        }

        //获取本机语言
        public static string GetMachineLanage()
        {
            return Application.systemLanguage.ToString();
        }


        //获取包名
        public static string GetIdentifier()
        {
            return Application.identifier;
        }


        //获取本机国家
        public static string GetCountry()
        {
            return RegionInfo.CurrentRegion.ToString();
        }

        //显存
        public static int GraphicsMemorySize { get { return SystemInfo.graphicsMemorySize; } }

        //系统内存
        public static int SystemMemorySize { get { return SystemInfo.systemMemorySize; } }

        /// <summary>
        /// 依据设备标识符生成一个唯一数字ID
        /// </summary>
        /// <param name="fromCode">重fromCode开始</param>
        /// <param name="wei"></param>
        /// <returns></returns>
        public static long GenerateID(long fromCode, int wei)
        {
            long num = 0;
            byte[] bytetest = Encoding.Default.GetBytes(SystemInfo.deviceUniqueIdentifier);
            for (int i = 0; i < bytetest.Length; i++) num += bytetest[i];
            int index = 0;
            int seed = wei;
            string tempNumStr = num.ToString();
            string numStr = (fromCode + num).ToString();
            while (numStr.Length < wei)
            {
                numStr += (int.Parse(tempNumStr.Substring(index, 1)) + (seed++)).ToString();
                index++;
                if (index >= tempNumStr.Length) index = 0;
            }
            if (numStr.Length > wei) numStr = numStr.Substring(0, wei);
            return long.Parse(numStr);
        }

        //转义设备id
        public static long TransCode(string fromCode, int wei)
        {
            long num = 0;
            byte[] bytetest = Encoding.Default.GetBytes(fromCode);
            for (int i = 0; i < bytetest.Length; i++) num += bytetest[i];
            int index = 0;
            int seed = wei;
            string tempNumStr = num.ToString();
            string numStr = (0 + num).ToString();
            while (numStr.Length < wei)
            {
                numStr += (int.Parse(tempNumStr.Substring(index, 1)) + (seed++)).ToString();
                index++;
                if (index >= tempNumStr.Length) index = 0;
            }
            if (numStr.Length > wei) numStr = numStr.Substring(0, wei);
            return long.Parse(numStr);
        }
        #endregion

        #region 字符串操作
        /// <summary>
        /// 字符串转MD5
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string GetMD5String(string strText)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(strText));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        public static int[] String2IntArray(string from, char split)
        {
            if (string.IsNullOrEmpty(from))
            {
                return new int[0];
            }
            var stringArray = from.Split(split);
            int[] reArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                reArray[i] = int.Parse(stringArray[i]);
            }
            return reArray;
        }

        public static float[] String2FloatArray(string from, char split)
        {
            if (string.IsNullOrEmpty(from))
            {
                return new float[0];
            }
            var stringArray = from.Split(split);
            float[] reArray = new float[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                reArray[i] = float.Parse(stringArray[i]);
            }
            return reArray;
        }

        public static double[] String2DoubleArray(string from, char split)
        {
            if (string.IsNullOrEmpty(from))
            {
                return new double[0];
            }
            var stringArray = from.Split(split);
            double[] reArray = new double[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                reArray[i] = double.Parse(stringArray[i]);
            }
            return reArray;
        }

        /// <summary>
        /// 修改字体颜色
        /// </summary>
        /// <param name="nochangeStr">不变色字符串</param>
        /// <param name="str">变色字</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static string AddTextColor(string str, string color)
        {
            return string.Format("<color={0}>{1}</color>", color, str);
        }

        /// <summary>
        /// 阿拉伯数字转换成中文数字
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string NumToChinese(string x)
        {
            string[] pArrayNum = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            //为数字位数建立一个位数组  
            string[] pArrayDigit = { "", "十", "百", "千" };
            //为数字单位建立一个单位数组  
            string[] pArrayUnits = { "", "万", "亿", "万亿" };
            var pStrReturnValue = ""; //返回值  
            var finger = 0; //字符位置指针  
            var pIntM = x.Length % 4; //取模  
            int pIntK;
            if (pIntM > 0)
                pIntK = x.Length / 4 + 1;
            else
                pIntK = x.Length / 4;
            //外层循环,四位一组,每组最后加上单位: ",万亿,",",亿,",",万,"  
            for (var i = pIntK; i > 0; i--)
            {
                var pIntL = 4;
                if (i == pIntK && pIntM != 0)
                    pIntL = pIntM;
                //得到一组四位数  
                var four = x.Substring(finger, pIntL);
                var P_int_l = four.Length;
                //内层循环在该组中的每一位数上循环  
                for (int j = 0; j < P_int_l; j++)
                {
                    //处理组中的每一位数加上所在的位  
                    int n = Convert.ToInt32(four.Substring(j, 1));
                    if (n == 0)
                    {
                        if (j < P_int_l - 1 && Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !pStrReturnValue.EndsWith(pArrayNum[n]))
                            pStrReturnValue += pArrayNum[n];
                    }
                    else
                    {
                        if (!(n == 1 && (pStrReturnValue.EndsWith(pArrayNum[0]) | pStrReturnValue.Length == 0) && j == P_int_l - 2))
                            pStrReturnValue += pArrayNum[n];
                        pStrReturnValue += pArrayDigit[P_int_l - j - 1];
                    }
                }
                finger += pIntL;
                //每组最后加上一个单位:",万,",",亿," 等  
                if (i < pIntK) //如果不是最高位的一组  
                {
                    if (Convert.ToInt32(four) != 0)
                        //如果所有4位不全是0则加上单位",万,",",亿,"等  
                        pStrReturnValue += pArrayUnits[i - 1];
                }
                else
                {
                    //处理最高位的一组,最后必须加上单位  
                    pStrReturnValue += pArrayUnits[i - 1];
                }
            }
            return pStrReturnValue;
        }

        public static string ReplaceAt(int index, string oldStr, string newStr)
        {
            var array = oldStr.ToArray();
            string restr = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i != index)
                {
                    restr += array[i].ToString();
                }
                else
                {
                    restr += newStr;
                }

            }
            return restr;
        }

        public static bool IsEmail(string inputData)
        {
            Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }
        #endregion

        #region 数组操作
        //从数组中随机选出不重复数据
        public static int[] GetRandomSequence(int length, int count)
        {
            if (length < count)
            {
                XDebug.LogError("无法从集合中获取更多数量的值");
                count = length;
            }

            int[] source = new int[length];
            int[] re = new int[count];

            for (int i = 0; i < length; i++) source[i] = i;

            int end = length - 1;
            for (int i = 0; i < count; i++)
            {
                int num = UnityEngine.Random.Range(0, end + 1);
                re[i] = source[num];
                source[num] = source[end];
                end--;
            }
            return re;
        }

        /// <summary>
        /// 泛型数组乱序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="tmpArray">输出</param>
        public static void Shuffle<T>(ref T[] tmpArray)
        {
            for (int i = 0; i < tmpArray.Length; i++)
            {
                T temp = tmpArray[i];
                int randomIndex = UnityEngine.Random.Range(0, tmpArray.Length);
                tmpArray[i] = tmpArray[randomIndex];
                tmpArray[randomIndex] = temp;
            }
        }

        /// <summary>
        /// 泛型list乱序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="tmpArray">输出</param>
        public static void Shuffle<T>(ref List<T> tmpArray)
        {
            for (int i = 0; i < tmpArray.Count; i++)
            {
                T temp = tmpArray[i];
                int randomIndex = UnityEngine.Random.Range(0, tmpArray.Count);
                tmpArray[i] = tmpArray[randomIndex];
                tmpArray[randomIndex] = temp;
            }
        }
        #endregion

        #region Commpont

        /// <summary>
        /// 滚动text，未处理类型
        /// </summary>
        /// <param name="text">滚动文字</param>
        /// <param name="mOldScore">就值</param>
        /// <param name="newScore">新值</param>
        /// <param name="time">滚动时间</param>
        /// <param name="endAction">结束事件，用于归一字符串类型</param>
        public static void TweenTextAnimation(Text text, float mOldScore, float newScore, float time, Action endAction)
        {
            DOTween.To(delegate (float value)
            {
                text.text = value.ToString("f1");
            }, mOldScore, newScore, time).OnComplete(() =>
            {
                endAction?.Invoke();
            });
        }

        public static void ToGraySomething(Transform go, bool isGray, bool isAll = true, bool isHasTxt = true)
        {
            if (isAll)
            {
                var images = go.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].material = isGray ? GameConfigMgr.constData.grayUI : null;
                }
                if (isHasTxt)
                {
                    var texts = go.GetComponentsInChildren<Text>();
                    for (int i = 0; i < texts.Length; i++)
                    {
                        texts[i].material = isGray ? GameConfigMgr.constData.grayUI : null;
                    }
                }
            }
        }
        #endregion

        #region 数字操作
        /// <summary>
        /// 总长度为四位
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToNumber(double number)
        {
            string str;
            if (number <= 9)
            {
                if (GetpointCount(number) > 3)
                {
                    str = number.ToString("f3");
                }
                else
                {

                    str = number.ToString();
                }
            }
            else if (number <= 99)
            {
                if (GetpointCount(number) > 2)
                {
                    str = number.ToString("f2");
                }
                else
                {

                    str = number.ToString();
                }
            }
            else if (number <= 999)
            {
                if (GetpointCount(number) > 1)
                {
                    str = number.ToString("f1");
                }
                else
                {

                    str = number.ToString();
                }
            }
            else if (number <= 9999)
            {
                str = ((int)number).ToString();
            }
            else if (number <= 999999)
            {
                str = GetStrByLength(number / 1000f, 3) + "K";
            }
            else if (number <= 999999999)
            {
                str = GetStrByLength(number / 1000000f, 3) + "M";
            }
            else if (number <= 999999999999)
            {
                str = GetStrByLength(number / 1000000000f, 3) + "B";
            }
            else
            {
                str = "∞";
            }
            return str;
        }

        //判断小数点后有几位
        private static int GetpointCount(double num)
        {
            var strs = num.ToString();
            if (strs.Contains('.'))
            {
                var strarry = strs.Split('.');
                return strarry[1].Length;

            }
            return 0;
        }

        private static string GetStrByLength(double value, int count)
        {
            var cov = value.ToString().ToCharArray();

            List<char> endArray = new List<char>();
            if (cov.Length < 3)
            {
                int need = Mathf.Abs(cov.Length - 3);
                for (int i = 0; i < cov.Length; i++)
                {
                    endArray.Add(cov[i]);
                }
                endArray.Add('.');
                for (int i = 0; i < need; i++)
                {
                    endArray.Add('0');
                }
            }
            else
            {
                endArray = cov.ToList();
            }
            string reStr = "";
            for (int i = 0; i < 3; i++)
            {
                if (endArray[i] == '.')
                {
                    count++;
                }
            }
            for (int i = 0; i < count; i++)
            {
                reStr += endArray[i];
            }
            return reStr;
        }


        /// <summary>
        /// 转义数字 +123,412,312.00
        /// </summary>
        /// <param name="number">数字</param>
        /// <param name="pointCount">小数点</param>
        /// <param name="hasSymbol">是否有符号</param>
        /// <returns></returns>
        public static string ConvertNumber(double number, int pointCount, bool hasSymbol)
        {
            string reStr;
            string symbol = "";
            if (hasSymbol)
            {
                symbol = number > 0 ? "+" : "-";
            }
            switch (pointCount)
            {
                case 0:
                    reStr = $"{symbol}{number:N0}";
                    return reStr;
                case 1:
                    reStr = $"{symbol}{number:N1}";
                    return reStr;
                case 2:
                    reStr = $"{symbol}{number:N2}";
                    return reStr;
                case 3:
                    reStr = $"{symbol}{number:N3}";
                    return reStr;
                case 4:
                    reStr = $"{symbol}{number:N4}";
                    return reStr;
            }
            return "";
        }
        #endregion

        #region Random 随机逻辑
        //计算概率(eg:{0.2f,0.3f,0.4f,0.5f} 概率随机) 
        public static int GetRandomValue(Dictionary<int, float> dic)
        {
            if (dic.Count <= 0)
            {
                XDebug.LogError("随机数量不够");
                return -1;
            }
            Dictionary<int, float> randomDIc = new Dictionary<int, float>(dic);

            float denominator = 0;
            //排序词典
            randomDIc = DictionarySort(randomDIc);

            //计算分母
            List<float> repetitionList = new List<float>();
            //记录重复数据
            Dictionary<float, int> repetionDic = new Dictionary<float, int>();
            foreach (var item in randomDIc)
            {
                denominator += item.Value;
                repetitionList.Add(item.Value);
                if (repetionDic.ContainsKey(item.Value))
                {
                    repetionDic[item.Value]++;
                }
                else
                {
                    repetionDic.Add(item.Value, 1);
                }
            }

            //随机档位
            var randomValue = UnityEngine.Random.Range(0, denominator);
            float value = repetitionList.Count;
            for (int i = 0; i < repetitionList.Count; i++)
            {
                float rate = 0;
                for (int j = 0; j <= i; j++) rate += repetitionList[j];
                rate = denominator - rate;

                if (randomValue >= rate)
                {
                    value = repetitionList[i];
                    break;
                }
            }
            int reCount = repetionDic[value];//重复的数量
            var reid = UnityEngine.Random.Range(0, reCount);
            List<int> dicID = new List<int>();
            foreach (var item in randomDIc)
            {
                if (item.Value == value)
                {
                    dicID.Add(item.Key);
                }
            }
            return dicID[reid];
        }


        //字典倒序（Value）
        public static Dictionary<int, float> DictionarySort(Dictionary<int, float> dic)
        {
            Dictionary<int, float> re = new Dictionary<int, float>();
            if (dic.Count > 0)
            {
                List<KeyValuePair<int, float>> lst = new List<KeyValuePair<int, float>>(dic);
                lst.Sort(delegate (KeyValuePair<int, float> s1, KeyValuePair<int, float> s2)
                {
                    return s2.Value.CompareTo(s1.Value);
                });
                dic.Clear();

                foreach (KeyValuePair<int, float> kvp in lst)
                {
                    re.Add(kvp.Key, kvp.Value);
                }
            }
            dic = re;
            return re;
        }

        //概率计算是否向上取整(eg:2.3f  30%=3, 70%=2 )
        public static int GetVlaueByRandom(float num, bool canZero = false)
        {
            int re;

            if (!canZero && num <= 1) return 1;

            int intNum = (int)num;
            float floatNum = num - intNum;

            if (GetOddsByFloat(floatNum))
            {
                re = Mathf.CeilToInt(num);
            }
            else
            {
                re = Mathf.FloorToInt(num);
            }
            return re;
        }

        //eg:prop=(0,1)
        public static bool GetOddsByFloat(float prop)
        {
            var rangeV = UnityEngine.Random.Range(0, 1f);
            return rangeV <= prop;
        }

        public static List<int> GetRandomIndex(int length, int count)
        {
            List<int> re = new List<int>();
            List<int> hasIndex = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int value = UnityEngine.Random.Range(0, length);
                if (hasIndex.Contains(value))
                {
                    i--;
                }
                else
                {
                    hasIndex.Add(value);
                    re.Add(value);
                }
            }
            return re;
        }
        #endregion

        #region 数学工具
        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">垂直于平面的的向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        public static Vector3 PointOfLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }

        //返回抛物线轨迹
        public static Vector3[] ReturnParabolaPath(Vector3 startPoint, Vector3 endPoint, int resolution, float height)
        {
            var bezierControlPoint = (startPoint + endPoint) * 0.5f + (Vector3.up * height);

            var _path = new Vector3[resolution];//resolution为int类型，表示要取得路径点数量，值越大，取得的路径点越多，曲线最后越平滑
            for (int i = 0; i < resolution; i++)
            {
                var t = (i + 1) / (float)resolution;//归化到0~1范围
                _path[i] = GetBezierPoint(t, startPoint, bezierControlPoint, endPoint);//使用贝塞尔曲线的公式取得t时的路径点
            }
            return _path;
        }

        /// <param name="t">0到1的值，0获取曲线的起点，1获得曲线的终点</param>
        /// <param name="start">曲线的起始位置</param>
        /// <param name="center">决定曲线形状的控制点</param>
        /// <param name="end">曲线的终点</param>
        public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
        {
            return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
        }


        //获取水平距离
        public static float V2Distance(Vector3 start, Vector3 end)
        {
            Vector2 begin = new Vector2(start.x, start.z);
            Vector2 target = new Vector2(end.x, end.z);
            return Vector2.Distance(begin, target);
        }

        //矩形范围检测
        public static bool RectCheck(Transform curTrans, Vector3 targetPos, float width, float height)
        {
            var vect = targetPos - curTrans.position;
            var curWidth = Vector3.Dot(vect, curTrans.right);
            var curheight = Vector3.Dot(vect, curTrans.forward);
            if (Mathf.Abs(curWidth) <= width / 2 && Mathf.Abs(curheight) <= height / 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 矩形检测
        /// </summary>
        /// <param name="curTrans">当前物体</param>
        /// <param name="targetPos">目标点</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="maxDis">矩形长度</param>
        /// <param name="startDis">矩形内起始点</param>
        /// <param name="isFront">是否是前方检测</param>
        /// <returns>是否在矩形内</returns>
        public static bool RectCheck(Transform curTrans, Vector3 targetPos, float width, float maxDis, float startDis = 0, bool isFront = true)
        {
            var vect = targetPos - curTrans.position;
            Vector3 normal = Vector3.Cross(vect, curTrans.right);
            if (isFront && normal.y < 0)
            {
                return false;
            }
            else if (!isFront && normal.y > 0)
            {
                return false;
            }

            var curWidth = Vector3.Dot(vect, curTrans.right);
            var curheight = Vector3.Dot(vect, curTrans.forward);
            if (Mathf.Abs(curWidth) <= width / 2 && Mathf.Abs(curheight) >= startDis && Mathf.Abs(curheight) <= maxDis)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 扇形检测
        /// </summary>
        /// <param name="maxDis">扇形最大距离</param>
        /// <param name="angle">扇形角度</param>
        /// <param name="startPos">开始位置</param>
        /// <param name="targetPos">目标位置</param>
        /// <param name="startDis">初始距离</param>
        /// <returns></returns>
        public static bool SectorCheck(Transform selftranpos, float angle, Vector3 targetPos, float maxDis, float startDis = 0)
        {
            var forward = selftranpos.forward.normalized;
            var target = (targetPos - selftranpos.position).normalized;
            float tmpAngle = Vector3.Angle(forward, target);
            var allAngle = tmpAngle;
            if (Mathf.Abs(allAngle) <= angle / 2)
            {
                var dis = Vector3.Distance(selftranpos.position, targetPos);
                if (dis <= maxDis && dis >= startDis)
                {
                    return true;
                }
            }
            return false;
        }
        //叉乘判断方向
        public static int CrossDir(Vector3 a, Vector3 b)
        {
            Vector3 c = Vector3.Cross(a, b);

            if (c.y > 0)
            {
                return 1;
            }
            else if (c.y == 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 取得两个向量之间需要旋转的时间
        /// </summary>
        /// <param name="dir1">第一根向量</param>
        /// <param name="dir2">第二根向量</param>
        /// <param name="speed">旋转速度 角度/秒</param>
        /// <returns></returns>
        public static float GetRotateTime(Vector3 dir1, Vector3 dir2, float speed)
        {
            var angle = Vector3.Angle(dir1, dir2);
            var time = Mathf.Abs(angle) / speed;
            return time;
        }


        public static Vector2 GetUIPos(RectTransform target)
        {
            var canvas = GameMain.Ins.mainCanvas;
            Vector3[] _corners = new Vector3[4];
            //获取高亮区域四个顶点的世界坐标
            target.GetWorldCorners(_corners);
            //计算高亮显示区域的中心
            float x = _corners[0].x + ((_corners[3].x - _corners[0].x) / 2f);
            float y = _corners[0].y + ((_corners[1].y - _corners[0].y) / 2f);
            Vector3 centerWorld = new Vector3(x, y, 0);
            return WorldToCanvasPos(canvas, centerWorld);
        }

        /// <summary>
        /// 世界坐标到画布坐标的转换
        /// </summary>
        /// <param name="canvas">画布</param>
        /// <param name="world">世界坐标</param>
        /// <returns>转换后在画布的坐标</returns>
        public static Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world,
                canvas.GetComponent<Camera>(), out position);
            return position;
        }

        public static Vector2 WorldToScreenPointCanvasPos(Vector3 world)
        {
            Vector2 convertedPos = Vector3.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                     GameMain.Ins.mainCanvas.transform as RectTransform, GameMain.Ins.uiCamera.WorldToScreenPoint(world),
                     GameMain.Ins.uiCamera,
                     out convertedPos);
            return convertedPos;

        }
        //获取中间值(5,9)=7
        public static float GetBetweenValue(float x, float y)
        {
            float max = Mathf.Max(x, y);
            float min = Mathf.Min(x, y);
            return min + (max - min) * 0.5f;
        }
        #endregion

        #region TimeConvert
        //获取当前年/月/日
        public static string GetTimeYMD()
        {
            return DateTime.Now.ToString("yyyy:MM:dd");
        }
        public static string GetTimeYMDHms()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int GetRunTimeDay(string oldTime)
        {
            int re;
            if (string.IsNullOrEmpty(oldTime) || oldTime == "0") return 0;
            DateTime pauseT = Convert.ToDateTime(oldTime);
            DateTime resumeT = DateTime.Now;
            TimeSpan ts1 = new TimeSpan(pauseT.Ticks);
            TimeSpan ts2 = new TimeSpan(resumeT.Ticks);
            TimeSpan tsSub = ts1.Subtract(ts2).Duration();
            re = tsSub.Days;
            return re;
        }
        public static int GetRunTimeSecent(string oldTime)
        {
            int re;
            if (string.IsNullOrEmpty(oldTime) || oldTime == "0") return 0;
            DateTime pauseT = Convert.ToDateTime(oldTime);
            DateTime resumeT = DateTime.Now;
            TimeSpan ts1 = new TimeSpan(pauseT.Ticks);
            TimeSpan ts2 = new TimeSpan(resumeT.Ticks);
            TimeSpan tsSub = ts1.Subtract(ts2).Duration();
            re = tsSub.Days * 86400 + tsSub.Hours * 3600 + tsSub.Minutes * 60 + tsSub.Seconds;
            return re;
        }
        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="seconds">秒</param>
        /// <returns></returns>
        public static string FormatTime(float seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = "";
            if (ts.Days > 0)
            {
                int hors = ts.Days * 24 + ts.Hours;
                str = hors.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Days == 0 && ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:" + ts.Seconds.ToString("00");
            }

            return str;
        }

        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="seconds">秒</param>
        /// <returns></returns>
        public static string FormatTimeCH(float seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = "";
            if (ts.Days > 0)
            {
                int hors = ts.Days * 24 + ts.Hours;
                str = hors.ToString() + "小时" + ts.Minutes.ToString() + "分钟" /*+ ts.Seconds.ToString("00")*/;
            }
            if (ts.Days == 0 && ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" /*+ ts.Seconds.ToString("00")*/;
            }
            if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "分钟" /*+ ts.Seconds.ToString("00")*/;
            }

            return str;
        }

        public static int GetTadyTime()
        {
            string oldtime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            return 86400 - GetRunTimeSecent(oldtime);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion
    }
}