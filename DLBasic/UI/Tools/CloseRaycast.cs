using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UGUI新建Text、Image、Button里的Text取消Raycast
/// Text文本默认居中
/// </summary>
public class CloseRaycast : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Text")]
    static void CreatText()
    {
        GameObject go = new GameObject("Text", typeof(Text));

        go.GetComponent<Text>().raycastTarget = false;
        go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        if (Selection.activeTransform && Selection.activeGameObject.layer == 5)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                go.transform.SetParent(Selection.activeTransform, false);
                go.layer = Selection.activeTransform.gameObject.layer;
            }
        }
        else
        {
            Canvas p = FindObjectOfType<Canvas>();
            if (p == null)
            {
                p = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
                p.gameObject.layer = 5;
                p.renderMode = RenderMode.ScreenSpaceOverlay;
                p.gameObject.AddComponent<CanvasScaler>();
                p.gameObject.AddComponent<GraphicRaycaster>();
                if (FindObjectOfType<EventSystem>() == null)
                    new GameObject("EventSystem", typeof(EventSystem)).AddComponent<StandaloneInputModule>();
            }
            go.transform.SetParent(p.transform, false);
            go.layer = p.gameObject.layer;
        }

        RectTransform r = go.transform as RectTransform;
        r.localPosition = Vector3.zero;
        r.localRotation = Quaternion.Euler(Vector3.zero);
        r.localScale = Vector3.one;

        Selection.activeObject = go;
        go = null;
    }

    [MenuItem("GameObject/UI/Image")]
    static void CreatImage()
    {
        GameObject go = new GameObject("Image", typeof(Image));
        go.GetComponent<Image>().raycastTarget = false;

        if (Selection.activeTransform && Selection.activeGameObject.layer == 5)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                go.transform.SetParent(Selection.activeTransform, false);
                go.layer = Selection.activeTransform.gameObject.layer;
            }
        }
        else
        {
            var p = FindObjectOfType<Canvas>();
            if (p == null)
            {
                p = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
                p.gameObject.layer = 5;
                p.renderMode = RenderMode.ScreenSpaceOverlay;
                p.gameObject.AddComponent<CanvasScaler>();
                p.gameObject.AddComponent<GraphicRaycaster>();
                if (FindObjectOfType<EventSystem>() == null)
                    new GameObject("EventSystem", typeof(EventSystem)).AddComponent<StandaloneInputModule>();
            }
            go.transform.SetParent(p.transform, false);
            go.layer = p.gameObject.layer;
        }

        RectTransform r = go.transform as RectTransform;
        r.localPosition = Vector3.zero;
        r.localRotation = Quaternion.Euler(Vector3.zero);
        r.localScale = Vector3.one;

        // EditorGUIUtility.PingObject(go);
        Selection.activeObject = go;
        go = null;
    }

    [MenuItem("GameObject/UI/Button")]
    public static void CreatButton()
    {
        GameObject go = new GameObject("Button", typeof(Image));
        RectTransform rect = go.transform as RectTransform;
        rect.sizeDelta = new Vector2(160, 30);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = go.GetComponent<Image>();

        GameObject textObj = new GameObject("Text", typeof(Text));
        RectTransform textRect = textObj.transform as RectTransform;
        textRect.SetParent(rect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.offsetMax = Vector3.zero;
        textRect.offsetMin = Vector3.zero;
        textObj.GetComponent<Text>().text = "Button";
        textObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        textObj.GetComponent<Text>().color = Color.black;
        textObj.GetComponent<Text>().raycastTarget = false;

        if (Selection.activeTransform && Selection.activeGameObject.layer == 5)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                go.transform.SetParent(Selection.activeTransform, false);
                go.layer = Selection.activeTransform.gameObject.layer;
            }
        }
        else
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
                canvas.gameObject.layer = 5;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.gameObject.AddComponent<CanvasScaler>();
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                if (FindObjectOfType<EventSystem>() == null)
                    new GameObject("EventSystem", typeof(EventSystem)).AddComponent<StandaloneInputModule>();
            }
            go.transform.SetParent(canvas.transform, false);
            go.layer = canvas.gameObject.layer;
        }

        textObj.layer = go.layer;
        Selection.activeObject = go;
        go = null;
    }
#endif
}
