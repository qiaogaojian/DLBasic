using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DLBasic.Common;

public class DNSDKTools : MonoSingletion<DNSDKTools>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// ��ȡandroidGAID
    /// </summary>
    public void GetGAID()
    {
#if UNITY_EDITOR  
        XDebug.LogError($"��֧�ִ�ƽ̨��ȡGAID");
        return;
#elif UNITY_ANDROID
        CurrentActivity.Call("GetGoogleADSID");
#elif UNITY_IOS || UNITY_IPHONE
        XDebug.LogError("��֧�ִ�ƽ̨��ȡGAID");
#endif
    }

#if UNITY_ANDROID
    /// <summary>
    /// androidGAID �ص�
    /// </summary>
    /// <param name="gaid"></param>
    public void GAIDBack(string gaid)
    {
        Debug.LogError(Time.deltaTime + "-----------:" + gaid);
    }
#endif

    /// <summary>
    /// ��ϵ����
    /// </summary>
    /// <param name="email">�ͷ�����</param>
    /// <param name="main">����</param>
    /// <param name="str">����</param>
    public void ContactUs(string email, string main, string str)
    {
        string ID = CommonTool.GenerateID(0, 6).ToString();
        string model = CommonTool.CurrentDeviceModel();
        string version = Application.version;

        var endStr = $"------------------------\n ID: {ID}\n Model: {model}\n Version: {version}\n------------------------\n {str}";
#if UNITY_EDITOR
        XDebug.LogError($"��֧�ִ�ƽ̨���䣬��ϢΪ��email��{email} ���⣺{main} ���ݣ�{endStr}");
        return;
#elif UNITY_ANDROID
        CurrentActivity.Call("contactUs", $"{email}/{main}/{endStr}");
#elif UNITY_IOS || UNITY_IPHONE
        XDebug.LogError("��֧�ִ�ƽ̨����");
#endif
    }

    //����
    public void Share(string meg)
    {

#if UNITY_EDITOR
        XDebug.LogError($"��֧�ִ�ƽ̨������ϢΪ��meg��{meg} ");
        return;
#elif UNITY_ANDROID
        CurrentActivity.Call("Share",meg);
#elif UNITY_IOS || UNITY_IPHONE
        XDebug.LogError("��֧�ִ�ƽ̨����");
#endif
    }


    #region Android
#if UNITY_ANDROID
    private AndroidJavaClass m_UnityPlayer;

    AndroidJavaClass UnityPlayer
    {
        get
        {
            if (m_UnityPlayer == null)
            {
                m_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            return m_UnityPlayer;
        }
    }


    private AndroidJavaObject m_CurrentActivity;
    AndroidJavaObject CurrentActivity
    {
        get
        {
            if (m_CurrentActivity == null)
            {
                m_CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return m_CurrentActivity;
        }
    }
#endif
    #endregion

}
