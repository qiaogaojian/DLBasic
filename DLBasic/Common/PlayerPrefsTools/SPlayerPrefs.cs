using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DLBasic.Common
{
    public abstract class StorageBase
    {
        public abstract void SetInt(string key, int v);
        public abstract void SetFloat(string key, float v);
        public abstract void SetSrting(string key, string v);
        public abstract int GetInt(string key, int def);
        public abstract float GetFloat(string key, float def);
        public abstract string GetSrting(string key, string def);
        public abstract void Delete(string key);
        public abstract void Save();
    }
    public class UnityStorage : StorageBase
    {
        public override float GetFloat(string key, float def)
        {
            return PlayerPrefs.GetFloat(key, def);
        }

        public override int GetInt(string key, int def)
        {
            return PlayerPrefs.GetInt(key, def);
        }

        public override string GetSrting(string key, string def)
        {
            return PlayerPrefs.GetString(key, def);
        }

        public override void SetFloat(string key, float v)
        {
            PlayerPrefs.SetFloat(key, v);
        }

        public override void SetInt(string key, int v)
        {
            PlayerPrefs.SetInt(key, v);
        }

        public override void SetSrting(string key, string v)
        {
            PlayerPrefs.SetString(key, v);
        }
        public override void Delete(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        public override void Save()
        {
            PlayerPrefs.Save();
        }
    }

    public abstract class SBase
    {
        protected StorageBase sb;
        protected string key;
        public string Key
        {
            get
            {
                return key;
            }
        }

        public void Init(StorageBase sb, string key)
        {
            this.sb = sb;
            this.key = key;
            OnInit();
        }
        protected virtual void OnInit()
        {
            Read();
        }
        protected abstract void Write();
        protected abstract void Read();
        public abstract void Delete();
        public void Save()
        {
            sb.Save();
        }
    }
    public class SInt : SBase
    {
        int value;
        int def;
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value == value) return;
                this.value = value;
                Write();
            }
        }
        public int Def
        {
            get
            {
                return def;
            }
            set
            {
                def = value;
            }
        }
        protected override void Read()
        {
            if (key == null) return;
            value = sb.GetInt(key, def);
        }
        protected override void Write()
        {
            if (key == null) return;
            sb.SetInt(key, value);
        }
        public override void Delete()
        {
            sb.Delete(key);
            value = def;
        }
    }
    public class SFloat : SBase
    {
        float value;
        float def;
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value == value) return;
                this.value = value;
                Write();
            }
        }
        public float Def
        {
            get
            {
                return def;
            }
            set
            {
                def = value;
            }
        }
        protected override void Read()
        {
            value = sb.GetFloat(key, def);
        }
        protected override void Write()
        {
            sb.SetFloat(key, value);
        }
        public override void Delete()
        {
            sb.Delete(key);
            value = def;
        }
    }
    public class SString : SBase
    {
        string value;
        string def;

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value == value) return;
                this.value = value;
                Write();
            }
        }
        public string Def
        {
            get
            {
                return def;
            }
            set
            {
                def = value;
            }
        }

        protected override void Read()
        {
            value = sb.GetSrting(key, def);
        }
        protected override void Write()
        {
            sb.SetSrting(key, value);
        }
        public override void Delete()
        {
            sb.Delete(key);
            value = def;
        }
    }
    public abstract class SStruct : SBase
    {
        const string ConnctFormat = "{0}_{1}";
        protected override void Read()
        {
            Type t = this.GetType();
            FieldInfo[] infos = t.GetFields();
            SBase tmp = null;
            for (int i = 0; i < infos.Length; i++)
            {
                FieldInfo fi = infos[i];

                if (fi.FieldType == typeof(SInt))
                {
                    tmp = new SInt();
                    SDefAttribute ca = GetAttr(fi);
                    if (ca != null) (tmp as SInt).Def = ca.intv;
                }
                else if (fi.FieldType == typeof(SFloat))
                {
                    tmp = new SFloat();
                    SDefAttribute ca = GetAttr(fi);
                    if (ca != null) (tmp as SFloat).Def = ca.floatv;
                }
                else if (fi.FieldType == typeof(SString))
                {
                    tmp = new SString();
                    SDefAttribute ca = GetAttr(fi);
                    if (ca != null) (tmp as SString).Def = ca.stringv;
                }
                else if (IsBaseType(fi.FieldType, typeof(SBase)))
                {
                    tmp = Activator.CreateInstance(fi.FieldType) as SBase;
                }
                else
                {
                    //Debug.Log(string.Format("{0} no suppot!", fi.Name));
                    continue;
                }
                tmp.Init(sb, string.Format(ConnctFormat, key, fi.Name));
                fi.SetValue(this, tmp);
            }
        }
        protected override void Write()
        {
           
        }
        public override void Delete()
        {
            Type t = this.GetType();
            FieldInfo[] infos = t.GetFields();
            for (int i = 0; i < infos.Length; i++)
            {
                FieldInfo fi = infos[i];
                if (IsBaseType(fi.FieldType, typeof(SBase)))
                {
                    SBase v = fi.GetValue(this) as SBase;
                    v.Delete();
                }
            }
        }

        bool IsBaseType(Type s, Type b)
        {
            Type baset = s.BaseType;
            while (baset != null)
            {
                if (baset == b) return true;
                baset = baset.BaseType;
            }
            return false;
        }

        SDefAttribute GetAttr(FieldInfo fi)
        {
            object[] cas = fi.GetCustomAttributes(typeof(SDefAttribute), false);
            if (cas.Length > 0)
            {
                SDefAttribute ca = cas[0] as SDefAttribute;
                return ca;
            }
            return null;
        }
    }
    public class SLinkList<T> : SBase where T : SBase,new()
    {        
        const string OriginStrFormat = "{0}_Origin";
        const string NextFormat = "{0}_n";
        const string PreviousFormat = "{0}_p";
        const string KeyFormat = "{0}_{1}";
        const string MaxIdFormat = "{0}_MaxId";
        Node __first = null;
        Node first
        {
            set
            {
                if (__first == value) return;
                __first = value;
                if (__first != null)
                {
                    m_origin = __first.Key;
                    sb.SetSrting(originkey, m_origin);
                }
                else
                {
                    sb.Delete(originkey);
                }
            }
            get
            {
                return __first;
            }
        }
        Node last;
     
        public int Count { private set; get; }
        string originkey = null;
        string m_origin = null;
        int maxid = 0;
        string MaxIDKey = null;
        int MaxID()
        {
            int re = maxid;
            maxid += 1;
            sb.SetInt(MaxIDKey, maxid);
            return re;
        }

        protected override void Read()
        {
            originkey = string.Format(OriginStrFormat, Key);
            m_origin = sb.GetSrting(originkey, null);
            MaxIDKey = string.Format(MaxIdFormat, Key);
            maxid = sb.GetInt(MaxIDKey, 0);
            if (string.IsNullOrEmpty(m_origin)) return;

            string nextkey = m_origin;
            Node previous = null;         
            while (true)
            {
                T t = new T();
                t.Init(sb, nextkey);
                Node n = new Node(t,sb);
                if (previous != null)
                {
                    n.InitPrevious(previous);
                    previous.InitNext(n);
                }
                else
                {
                    first = n;
                }
                previous = n;
                Count++;

                nextkey = sb.GetSrting(string.Format(NextFormat, n.Key), null);

                if (string.IsNullOrEmpty(nextkey))
                {
                    last = n;
                    break;
                }
            }
        }
        protected override void Write()
        {
            
        }
        public override void Delete()
        {
            Node cur = first;
            while (cur != null)
            {
                cur.Delete();
                cur = cur.Next;
            }
            first = null;
            last = null;
            sb.Delete(MaxIDKey);
        }
        public T AddLast()
        {           
            Node n = new Node(CeartNew(), sb);
            if (last == null)
            {
                first = n;
                last = n;
            }
            else
            {
                last.Next = n;
                n.Previous = last;
                last = n;
            }
            Count++;
            return n.Data;
        }
        public T AddFirst()
        {
            Node n = new Node(CeartNew(), sb);
            if (first == null)
            {
                first = n;
                last = n;
            }
            else
            {
                first.Previous = n;
                n.Next = first;
                first = n;
            }
            Count++;
            return n.Data;
        }
        public void Remove(T d)
        {
            Node cur = first;
            while (cur!=null)
            {
                if (cur.Key == d.Key)
                {
                    if (cur.Previous != null)
                    {
                        cur.Previous.Next = cur.Next;
                    }
                    if (cur.Next != null)
                    {
                        cur.Next.Previous = cur.Previous;
                    }

                    if (cur.Key == first.Key)
                    {
                        first = cur.Next;
                    }

                    if (cur.Key == last.Key)
                    {
                        last = cur.Previous;
                    }
                    Count--;
                    cur.Delete();
                    break;
                }
                cur = cur.Next;
            }
        }
        public List<T> ToList()
        {
            Node cur = first;
            List<T> re = new List<T>();
            while (cur != null)
            {
                re.Add(cur.Data);
                cur = cur.Next;
            }
            return re;
        }
        public T Find(Predicate<T> match)
        {
            Node cur = first;
            while (cur != null)
            {
                if (match(cur.Data))
                {
                    return cur.Data;
                }
                cur = cur.Next;
            }
            return null;
        }
        public void ForEach(Predicate<T> action)
        {
            Node cur = first;
            while (cur != null)
            {
                if (!action(cur.Data))
                {
                    break;
                }
                cur = cur.Next;
            }
        }
        public void MoveTo(T d, int index)
        {
            if (index >= Count) return;
            Node source = FindNode(d);
            Node target = GetNode(index);
            if (source == null || target == null) return;

            if (source.Previous != null)
            {
                source.Previous.Next = source.Next;
            }
            if (source.Next != null)
            {
                source.Next.Previous = source.Previous;
            }

            if (target.Previous != null)
            {
                target.Previous.Next = source;
            }
            source.Previous = target.Previous;

            source.Next = target;
            target.Previous = source;
        }
        public void Switch(T d1, T d2)
        {
            Node n1 = FindNode(d1);
            Node n2 = FindNode(d2);
            if (n1 == null || n2 == null) return;
            Node tmp = n1.Previous;
            n1.Previous = n2.Previous;
            n2.Previous = tmp;

            tmp = n1.Next;
            n1.Next = n2.Next;
            n2.Next = tmp;
        }
        Node FindNode(T d)
        {
            Node cur = first;
            while (cur != null)
            {
                if (d.Key == cur.Data.Key)
                {
                    return cur;
                }
                cur = cur.Next;
            }
            return null;
        }
        Node GetNode(int index)
        {
            if (index >= Count) return null;
            Node cur = first;
            int curindex = 0;
            while (cur != null)
            {
                if (curindex == index) return cur;
                curindex++;
                cur = cur.Next;
            }
            return null;
        }

        T CeartNew()
        {
            T re = new T();
            re.Init(sb, string.Format(KeyFormat, Key, MaxID()));
            return re;
        }
        public class Node
        {
            private StorageBase sb;
            Node _Next;
            public Node Next
            {
                set
                {
                    if (_Next == value) return;
                    _Next = value;
                    string _key = string.Format(SLinkList<T>.NextFormat, Key);
                    if (_Next == null)
                    {
                        sb.Delete(_key);
                    }
                    else
                    {
                        sb.SetSrting(_key, _Next.Key);
                    }
                }
                get
                {
                    return _Next;
                }
            }
            Node _Previous;
            public Node Previous
            {
                set
                {
                    if (_Previous == value) return;
                    _Previous = value;
                    string _key = string.Format(SLinkList<T>.PreviousFormat, Key);
                    if (_Previous == null)
                    {
                        sb.Delete(_key);
                    }
                    else
                    {
                        sb.SetSrting(_key, _Previous.Key);
                    }
                }
                get
                {
                    return _Previous;
                }
            }
            public T Data { get;}
            public string Key { get { return Data.Key; } }

            public Node(T d, StorageBase sb)
            {
                Data = d;
                this.sb = sb;
            }

            public void InitNext(Node n)
            {
                _Next = n;
            }
            public void InitPrevious(Node n)
            {
                _Previous = n;
            }

            public void Delete()
            {
                Data.Delete();
                sb.Delete(string.Format(SLinkList<T>.NextFormat, Key));
                sb.Delete(string.Format(SLinkList<T>.PreviousFormat, Key));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SDefAttribute : Attribute
    {
        readonly public int intv;
        readonly public float floatv;
        readonly public string stringv;
        public SDefAttribute(int v)
        {
            this.intv = v;          
        }
        public SDefAttribute(float v)
        {
            this.floatv = v;
        }
        public SDefAttribute(string v)
        {
            this.stringv = v;
        }
    }
}
