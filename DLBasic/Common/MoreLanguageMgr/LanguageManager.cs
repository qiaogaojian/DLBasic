using UnityEngine.UI;

namespace DLBasic.Common
{
    public enum LanguageType
    {
        CHINESE,
        ENGLISH,
        JAPANESE,
        KOREAN
    }

    public delegate void ChangeLanguage();
    public delegate void ChangeFont(UnityEngine.Font font);
    public class LanguageManager : MonoSingletion<LanguageManager>
    {

        //����һ��ί���¼������ڸı�����ʱ�����Ըı䵱ǰ����ʾ���ı�
        public event ChangeLanguage ChangeLangeuageEvent;
        public event ChangeFont ChangeFont;

        public void SentEvent()
        {
            ChangeLangeuageEvent?.Invoke();
        }

        public void LanguageChangeFont(UnityEngine.Font font)
        {
            ChangeFont?.Invoke(font);
        }
        /// <summary>
        /// ͨ��key��ȡ���ݣ�����Text���ı���ʾ����
        /// </summary>
        public void SetText(Text text, string target, string extraStr = "")
        {
            if (string.IsNullOrEmpty(target))
            {
                return;
            }
            if (string.IsNullOrEmpty(extraStr))
            {
                //text.text = GameConfigMgr.languageConfig.GetStrByKey(target);
            }
            else
            {
                //text.text = string.Format(GameConfigMgr.languageConfig.GetStrByKey(target), extraStr);
            }


        }

    }
}