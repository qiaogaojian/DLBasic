using System;
using UnityEngine;
using DG.Tweening;
using DLBasic.Common;

namespace DLBasic.UI
{
    public class UIBase : MonoBehaviour
    {
        public Action OnCloseCall = null;
        public virtual UIType UIType { get { return UIType.WindowUI; } }
        public virtual int UIDepth { get { return (int)UIType; } }
        public virtual int CloseType { get { return 0; } }//0:销毁 1:隐藏

        [HideInInspector]
        public long GUID;

        [HideInInspector]
        public string UIPath;

        [HideInInspector]
        public bool IsClose = false;

        private UIMgr mgr;

        private GameObject __screenmask = null;

        public bool ScreenMask
        {
            set
            {
                if (__screenmask == null)
                {
                    __screenmask = new GameObject("__screenmask");
                    __screenmask.transform.SetParent(transform);
                    __screenmask.AddComponent<Empty4Raycast>().raycastTarget = true;
                    var rectc = __screenmask.GetComponent<RectTransform>();
                    rectc = rectc == null ? __screenmask.AddComponent<RectTransform>() : rectc;
                    rectc.anchorMin = new Vector2(0f, 0f);
                    rectc.anchorMax = new Vector2(1f, 1f);
                    rectc.offsetMin = new Vector2(0f, 0f);
                    rectc.offsetMax = new Vector2(0f, 0f);
                }
                __screenmask.transform.SetAsLastSibling();
                __screenmask.SetActive(value);
            }
            get
            {
                return __screenmask != null && __screenmask.activeInHierarchy;
            }
        }

        public void Init(UIMgr mgr)
        {
            this.mgr = mgr;
            IsClose = false;
        }

        public void Close()
        {
            IsClose = true;
            mgr.Close(GUID);
        }

        public void SetChangeDepth()
        {
            if (UIDepth == (int)UIType) return;
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = UIDepth;
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        public virtual void OnClose()
        {
            OnCloseCall?.Invoke();
        }

        public void ClickBtnAudio()
        {
            //GameMain.Ins.audioMgr.Play(MusicPath.ButtonClickAudio, false, AudioMgr.PlayType.OnShot, AudioMgr.AudioType.effect);
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        }

        public void CloseBtnAudio()
        {
            //GameMain.Ins.audioMgr.Play(MusicPath.ButtonClickAudio, false, AudioMgr.PlayType.OnShot, AudioMgr.AudioType.effect);
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        }
    }
}