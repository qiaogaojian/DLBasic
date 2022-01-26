using System.IO;
using UnityEditor;
using UnityEngine;
namespace DLBasic.EditorExtend
{
    public class ConfigEditorHelper
    {
        [MenuItem("Assets/生成配置")]
        public static void BatchCreateConfig()
        {
            CreatEmptyConfig();
        }

        private static void CreatEmptyConfig()
        {
            if (Selection.activeInstanceID < 0)
            {
                Debug.LogError("请选中配置脚本！！");
                return;
            }

            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

            string name = Path.GetFileName(path);
            string[] arr = name.Split('.');

            if (arr[1] != "cs")
            {
                Debug.LogError("请选脚本类型！！");
                return;
            }

            name = arr[0];

            ScriptableObject obj = ScriptableObject.CreateInstance(name);

            if (obj == null)
            {
                Debug.LogError("脚本类型不正确，没有继承 ScriptableObject！！");
                return;
            }
            string p = path.Replace(".cs", ".asset");
            AssetDatabase.CreateAsset(obj, p);
        }

    }
}
