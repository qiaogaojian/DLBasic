using UnityEngine;

namespace DLBasic.Common
{
    public abstract class Singletion<T> where T : Singletion<T>, new()
    {

        protected static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                    m_instance.Init();

                }
                return m_instance;
            }
        }
        protected virtual void Init()
        {
        }

    }
    public abstract class MonoSingletion<T> : MonoBehaviour where T : MonoSingletion<T>
    {

        protected static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();
                    if (m_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        m_instance = go.AddComponent<T>();
                        m_instance.Init();
                    }
                }
                return m_instance;
            }
        }

        public virtual void Init()
        {

        }
    }
}
