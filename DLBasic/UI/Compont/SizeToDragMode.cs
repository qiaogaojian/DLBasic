using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace DLBasic.UI
{
    public class SizeToDragMode : UIBehaviour
    {
        public ScrollRect scrollRect;
        protected override void Awake()
        {
            base.Awake();
            Transform p = transform.parent;
            if (scrollRect == null)
            {
                while (p != null)
                {
                    scrollRect = p.GetComponent<ScrollRect>();

                    if (scrollRect != null)
                    {
                        OnRectTransformDimensionsChange();
                        return;
                    }
                    p = p.parent;
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            RectTransform rt = GetComponent<RectTransform>();

            if (scrollRect == null) { XDebug.Log(" scrollRect is null", LogHelper.B13Port); return; }
            RectTransform scrollRT = scrollRect.GetComponent<RectTransform>();

            scrollRect.horizontal = rt.rect.width > scrollRT.rect.width;
            scrollRect.vertical = rt.rect.height > scrollRT.rect.height;
        }

    }
}