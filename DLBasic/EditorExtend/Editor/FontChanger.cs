using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
namespace DLBasic.EditorExtend
{
    public class FontChanger
    {
        const string FontPath = "Assets/GameRes/Font/方正正粗黑简体.TTF";
        [MenuItem("Tools/替换Font")]
        public static void ChangeFont()
        {
            if (UnityEditor.Selection.activeInstanceID <= 0)
            {
                Debug.LogError("请选中文件");
                return;
            }
            Font f = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
            string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeInstanceID);
            if (!Directory.Exists(path))
            {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (g == null) return;
                ChancgePrefab(g, f);
                AssetDatabase.SaveAssets();
                Debug.Log("ChangeFont Success");
                return;
            }
            string[] gos = Directory.GetFiles(path, @"*.prefab", SearchOption.AllDirectories);
            if (gos.Length == 0) return;
            if (f == null)
            {
                Debug.LogError("ChangeFont no font");
            }
            for (int i = 0; i < gos.Length; i++)
            {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>(gos[i]);
                ChancgePrefab(g, f);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("ChangeFont Success");
        }

        static void ChancgePrefab(GameObject go, Font f)
        {
            if (go == null) return;
            Text[] arr = go.GetComponentsInChildren<Text>(true);
            bool change = false;
            for (int i = 0; i < arr.Length; i++)
            {
                //if (arr[i].font == null)
                //{
                    arr[i].font = f;
                    change = true;
                //}
            }
            if (change)
            {
                EditorUtility.SetDirty(go);
            }
        }

    }
}