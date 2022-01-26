using System;
using System.Collections.Generic;
namespace DLBasic.Common
{
    public class ObjectPool<TValue>
    {
        public delegate void CreatObjectFun(Action<TValue> callback);
        public delegate void DestroyObjectFun(TValue value);
        int maxLimit = int.MaxValue;
        public ObjectPool(CreatObjectFun c = null, DestroyObjectFun d = null)
        {
            creatFun = c;
            destroyFun = d;
        }
        public void SetMaxLimit(int v)
        {
            maxLimit = v;
        }

        private Queue<TValue> pool = new Queue<TValue>();

        public CreatObjectFun creatFun;
        public DestroyObjectFun destroyFun;
        public TValue Get()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return Create();
        }
        public void GetAsync(Action<TValue> callback)
        {
            if (pool.Count > 0)
            {
                callback(pool.Dequeue());
                return;
            }
            CreateAsync(callback);
        }
        private TValue Create()
        {
            TValue re = default(TValue);
            if (creatFun != null)
            {
                creatFun((v) => { re = v; });
                return re;
            }
            return re;
        }
        private void CreateAsync(Action<TValue> callback)
        {
            if (creatFun != null)
            {
                creatFun(callback);
            }
            else
            {
                callback(default(TValue));
            }
        }
        public void Add(TValue value)
        {
            if (pool.Count >= maxLimit)
            {
                destroyFun(value);
                return;
            }
            pool.Enqueue(value);
        }
        public void Clear()
        {
            if (destroyFun != null)
            {
                foreach (var item in pool)
                {
                    destroyFun(item);
                }
            }
            pool.Clear();
        }
    }
    public class PoolManager<TKey, TValue> /*where TValue:Component*/
    {
        public delegate void CreatObjectFun(TKey key, Action<TValue> callback);
        public delegate void DestroyObjectFun(TKey key, TValue value);
        int maxLimit = int.MaxValue;
        public PoolManager(CreatObjectFun c = null, DestroyObjectFun d = null)
        {
            creatFun = c;
            destroyFun = d;
        }

        Dictionary<TKey, ObjectPool<TValue>> pools = new Dictionary<TKey, ObjectPool<TValue>>();

        public CreatObjectFun creatFun;
        public DestroyObjectFun destroyFun;
        public void SetMaxLimit(int v)
        {
            maxLimit = v;
        }
        public TValue Get(TKey key)
        {
            if (pools.ContainsKey(key))
            {
                var pool = pools[key];
                return pool.Get();
            }

            if (creatFun != null)
            {
                AddPool(key);
                return pools[key].Get();
            }

            return default(TValue);
        }
        public void GetAsync(TKey key, Action<TValue> callback)
        {
            if (pools.ContainsKey(key))
            {
                var pool = pools[key];
                pool.GetAsync(callback);
                return;
            }

            if (creatFun != null)
            {
                AddPool(key);
                pools[key].GetAsync(callback);
                return;
            }

            callback(default(TValue));
        }
        public void Add(TKey key, TValue value)
        {
            if (!pools.ContainsKey(key))
            {
                AddPool(key);
            }
            var pool = pools[key];
            pool.Add(value);
        }
        void AddPool(TKey key)
        {
            var pool = new ObjectPool<TValue>();
            pool.SetMaxLimit(maxLimit);
            pools[key] = pool;
            pool.creatFun = (call) =>
            {
                if (creatFun != null)
                {
                    creatFun(key, call);
                    return;
                }
                call(default(TValue));
            };
            pool.destroyFun = (value) =>
            {
                if (destroyFun != null)
                {
                    destroyFun(key, value);
                }
            };
        }
        public void Clear(TKey key)
        {
            if (pools.ContainsKey(key))
            {
                var pool = pools[key];
                pool.Clear();
                pools.Remove(key);
            }
        }
        public void ClearAll()
        {
            foreach (var item in pools)
            {
                item.Value.Clear();
            }
            pools.Clear();
        }
    }

}