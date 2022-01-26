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
    /// 获取androidGAID
    /// </summary>
    public void GetGAID()
    {
#if UNITY_EDITOR  
        XDebug.LogError($"不支持此平台获取GAID");
        return;
#elif UNITY_ANDROID
        CurrentActivity.Call("GetGoogleADSID");
#elif UNITY_IOS || UNITY_IPHONE
        XDebug.LogError("不支持此平台获取GAID");
#endif
    }

#if UNITY_ANDROID
    /// <summary>
    /// androidGAID 回调
    /// </summary>
    /// <param name="gaid"></param>
    public void GAIDBack(string gaid)
    {
        Debug.LogError(Time.deltaTime + "-----------:" + gaid);
    }
#endif

    /// <summary>
    /// 联系我们
    /// </summary>
    /// <param name="email">客服邮箱</param>
    /// <param name="main">主题</param>
    /// <param name="str">正文</param>
    public void ContactUs(string email, string main, string str)
    {
        string ID = CommonTool.GenerateID(0, 6).ToString();
        string model = CommonTool.CurrentDeviceModel();
        string version = Application.version;

        var endStr = $"------------------------\n ID: {ID}\n Model: {model}\n Version: {version}\n------------------------\n {str}";
#if UNITY_EDITOR
        XDebug.LogError($"不支持此平台邮箱，信息为：email：{email} 主题：{main} 内容：{endStr}");
        return;
#elif UNITY_ANDROID
        CurrentActivity.Call("contactUs", $"{email}/{main}/{endStr}");
#elif UNITY_IOS || UNITY_IPHONE
        XDebug.LogError("不支持此平台邮箱");
#endif
    }

    //分享
    public void Share(string meg)
    {

#if UNITY_EDITOR
        XDebug.LogError($"不支持此平台分享，信息为：meg：{meg} ");
        return;
#elif UNITY_ANDROID
        CurrentActivity.Call("Share",meg);
#elif UNITY_IOS || UNITY_IPHONE
        XDebug.LogError("不支持此平台分享");
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
