using UnityEngine;
using UnityEditor;

public static class TextureOverviewMenuItem
{
#if UNITY_EDITOR
    [MenuItem("Window/Texture Overview")]
    static void ShowWindow()
    {
        var wnd = TextureOverview.MainWindow.CreateWindow();
        if (wnd != null)
            wnd.Show();
    }
#endif
}
