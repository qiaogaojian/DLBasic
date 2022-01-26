using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DLBasic.Common
{
    [RequireComponent(typeof(Text))]
    public class LanguageText : MonoBehaviour
    {
        private Text curText;
        public string key;

        public string extraStr;
        private void Awake()
        {
            curText = GetComponent<Text>();
            LanguageManager.Instance.ChangeLangeuageEvent += OnChangeText;   // �����Ըı��¼�
            OnChangeText();
        }

        private void OnChangeText()
        {
            LanguageManager.Instance.SetText(curText, key, extraStr);
        }

        private void OnDestroy()
        {
            LanguageManager.Instance.ChangeLangeuageEvent -= OnChangeText;   // �����Ըı��¼�
        }
    }
}