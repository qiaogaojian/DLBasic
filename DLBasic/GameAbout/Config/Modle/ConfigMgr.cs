using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using XAsset;

namespace DLBasic.Common
{
    public class ConfigMgr
    {
        Assets m_assets = null;
        Dictionary<Type, object> cfgDic = null;
        Queue<Asset> resQeuue = null;
        bool __openrelease = true;
        public ConfigMgr(Assets a, params Type[] preloads)
        {
            m_assets = a;
            cfgDic = new Dictionary<Type, object>();
            resQeuue = new Queue<Asset>();

            __openrelease = false;
            for (int i = 0; i < preloads.Length; i++)
            {
                Type t = preloads[i];
                LoadCfg(t);
            }
            ClearCacheRes();
            __openrelease = true;
        }

        public T Get<T>(bool reload = false)
        {
            return (T)Get(typeof(T), reload);
        }
        public object Get(Type t, bool reload = false)
        {
            if (!cfgDic.ContainsKey(t) || reload)
            {
                LoadCfg(t);
            }
            return cfgDic[t];
        }
        public void Remove<T>()
        {
            Remove(typeof(T));
        }
        public void Remove(Type t)
        {
            if (cfgDic.ContainsKey(t))
            {
                cfgDic.Remove(t);
            }
        }

        void LoadCfg(Type t)
        {
            object[] cas = t.GetCustomAttributes(typeof(CfgAttribute), false);
            string path;
            if (cas.Length > 0)
            {
                CfgAttribute ca = cas[0] as CfgAttribute;
                path = ca.path;
                cfgDic[t] = LoadRes(path, t);
            }
            else
            {
                Debug.LogError(t.Name + " no CfgAttribute");
            }
        }

        //通过路径读取
        public object Get(Type t, string path)
        {
            LoadCfgByPath(t, path);
            return cfgDic[t];
        }

        private void LoadCfgByPath(Type t, string path)
        {
            cfgDic[t] = LoadRes(path, t);
        }


        const string TYPE_XML = ".xml";
        const string TYPE_JSON = ".json";
        const string TYPE_UNITY = ".asset";
        object LoadRes(string path, Type t)
        {
            object re = null;
            if (path.EndsWith(TYPE_XML))
            {
                Asset asset = m_assets.Load(path, typeof(object));
                string content = asset.asset.ToString();
                using (StringReader sr = new StringReader(content))
                {
                    XmlSerializer xmldes = new XmlSerializer(t);
                    re = xmldes.Deserialize(sr);
                }
                resQeuue.Enqueue(asset);
            }
            else if (path.EndsWith(TYPE_JSON))
            {
                Asset asset = m_assets.Load(path, typeof(object));
                string content = asset.asset.ToString();
                resQeuue.Enqueue(asset);
                re = Newtonsoft.Json.JsonConvert.DeserializeObject(content, t);
            }
            else if (path.EndsWith(TYPE_UNITY))
            {
                Asset asset = m_assets.Load(path, t);
                re = UnityEngine.Object.Instantiate(asset.asset);
                resQeuue.Enqueue(asset);
            }
            else
            {
                re = Activator.CreateInstance(t);
            }

            MethodInfo mi = t.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null)
            {
                mi.Invoke(re, null);
            }

            if (__openrelease)
            {
                ClearCacheRes();
            }
            return re;
        }
        void ClearCacheRes()
        {
            while (resQeuue.Count > 0)
            {
                resQeuue.Dequeue().Release();
            }
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class CfgAttribute : Attribute
    {
        readonly public string path;
        public CfgAttribute(string path)
        {
            this.path = path;
        }
    }
}
