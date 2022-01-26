using UnityEditor;
using UnityEngine;

public class GameFps : MonoBehaviour
{
    private float FIxeFps = 0.5f;
    private int Fps = 0;
    private float deteTimes = 0f;
    private const float recTime = 0.5f;

    public bool isShowOther;

    private void OnGUI()
    {
#if !UNITY_EDITOR
       if (!isShowOther)
	    {
        return;
	    }
#endif                                                
        Color color = GUI.color;
        GUI.color = Color.black;
        int guiFont = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = 40;
        GUI.Label(new Rect(Screen.width - 150, 150, 200, 200), Fps.ToString());
        GUI.skin.label.fontSize = guiFont;
        GUI.color = color;
        if (!isShowOther) return;
#if UNITY_EDITOR
        #region Unity Messages
        GUILayout.TextField("Total DrawCall: " + UnityStats.drawCalls);
        GUILayout.TextField("Batch: " + UnityStats.batches);
        GUILayout.TextField("Static Batch DC: " + UnityStats.staticBatchedDrawCalls);
        GUILayout.TextField("Static Batch: " + UnityStats.staticBatches);
        GUILayout.TextField("DynamicBatch DC: " + UnityStats.dynamicBatchedDrawCalls);
        GUILayout.TextField("DynamicBatch: " + UnityStats.dynamicBatches);
        GUILayout.TextField("Tri: " + UnityStats.triangles);
        GUILayout.TextField("Ver: " + UnityStats.vertices);
        #endregion
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
        if (!isShowOther)
	    {
        return;
	    }
#endif
        deteTimes++;
        if (Time.realtimeSinceStartup > FIxeFps)
        {
            Fps = (int)(deteTimes / recTime);
            deteTimes = 0;
            FIxeFps += recTime;
        }
    }
}