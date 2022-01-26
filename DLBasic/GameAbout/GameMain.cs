using Cinemachine;
using DLBasic.Common;
using DLBasic.UI;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using UnityEngine;
using XAsset;

public class GameMain : MonoBehaviour
{
    public static GameMain Ins = null;
    [NonSerialized]
    public Assets Assets = null;
    public UIMgr UIMgr = null;
    public ConfigMgr ConfigMgr = null;
    [NonSerialized] public AudioMgr audioMgr = null;

    public CinemachineBrain cinCamera;
    public Canvas mainCanvas;
    public RectTransform canvasRectt;
    public Camera uiCamera;
    public Camera mainCamera;

    [NonSerialized] public InformationCollection.ICMgr iCMgr = null;
    [NonSerialized] public DuLu.AD.ADMgr aDMgr = null;

    private void Awake()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
#if UNITY_EDITOR
        LogHelper.Init(true);
#endif
        LogHelper.Init(true);
        Ins = this;
        XDebug.Log("**********GameMain**********", LogHelper.B13Port);
        Application.targetFrameRate = 80;
        Input.multiTouchEnabled = true;
        Assets = XAsset.Assets.Creat();
        DontDestroyOnLoad(Assets.gameObject);
        UIMgr = new UIMgr(Assets);
        ConfigMgr = new ConfigMgr(Assets);
        GameConfigMgr.LoadConfig(ConfigMgr);
        audioMgr = AudioMgr.Create(Assets);

#if UNITY_EDITOR
        iCMgr = InformationCollection.ICMgr.Creat();
        aDMgr = DuLu.AD.ADMgr.Creat();
#elif UNITY_ANDROID||UNITY_IPHONE
        iCMgr = Main.Ins.iCMgr;
        aDMgr = Main.Ins.aDMgr;
#endif
    }

}
