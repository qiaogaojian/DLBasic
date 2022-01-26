using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;

//游戏常量
public class ConstData : ScriptableObject
{
    [LabelText("UI置灰Shader")] public Material grayUI;
    [LabelText("隐私政策")] public string policy = "https://cube2048.royalcasualgame.com/privacypolicy.html";
    [LabelText("用户协议")] public string terms = "https://cube2048.royalcasualgame.com/termsofservice.html";
}
