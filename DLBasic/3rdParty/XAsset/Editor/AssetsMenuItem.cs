using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace XAsset.Editor
{
    public static class AssetsMenuItem
    {
        private const string isBuildAssetBundles = "isBuildAssetBundles";
        private static bool IsBuildAssetBundles
        {
            get { return EditorPrefs.GetBool(isBuildAssetBundles); }
            set { EditorPrefs.SetBool(isBuildAssetBundles, value); }
        }
        private const string buildAssetBundlesIndex = "buildAssetBundlesIndex";
        private static int BuildAssetBundlesIndex
        {
            get { return EditorPrefs.GetInt(buildAssetBundlesIndex); }
            set { EditorPrefs.SetInt(buildAssetBundlesIndex, value); }
        }
        private const string buildMainState = "builMainState";
        private static string BuilMainState
        {
            get { return EditorPrefs.GetString(buildMainState); }
            set { EditorPrefs.SetString(buildMainState, value); }
        }

        [MenuItem("Assets/Copy Asset Path")]
        static void CopyAssetPath()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            GUIUtility.systemCopyBuffer = path;
            XDebug.Log(string.Format("systemCopyBuffer: {0}", path));
        }

        const string kRuntimeMode = "Assets/XAsset/Bundle Mode";

        [MenuItem(kRuntimeMode)]
        public static void ToggleRuntimeMode()
        {
            XEditorUtility.ActiveBundleMode = !XEditorUtility.ActiveBundleMode;
        }

        [MenuItem(kRuntimeMode, true)]
        public static bool ToggleRuntimeModeValidate()
        {
            Menu.SetChecked(kRuntimeMode, XEditorUtility.ActiveBundleMode);
            return true;
        }

        //const string assetsManifesttxt = "Assets/XAsset/Manifest.txt";
        //const string rulesini = "Assets/XAsset/Rule.txt";

        public static void BuildAssetManifest(string rule, string manifest)
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            List<AssetBundleBuild> builds = BuildRule.GetBuilds(manifest, rule);
            BuildScript.BuildManifest(manifest, builds);
        }

        [MenuItem("Assets/XAsset/Build Manifest")]
        public static void BuildAssetManifest()
        {

            return;
        }



        public static bool BuildAssetBundles(string rule, string manifest, string rootPath)
        {
            if (EditorApplication.isCompiling)
            {
                return false;
            }

            //LuaProductor.HandleLua(null, null);
            List<AssetBundleBuild> builds = BuildRule.GetBuilds(manifest, rule);
            BuildScript.BuildManifest(manifest, builds);
            BuildScript.BuildAssetBundles(builds, rootPath);
            return true;
        }



        [MenuItem("Assets/XAsset/Build AssetBundles")]
        public static bool BuildAssetBundles()
        {
            BuilMainState = "None";
            IsBuildAssetBundles = true;
            BuildAssetBundlesIndex = -1;
            AutoBuildAssetBundles();
            return false;
        }


        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            if (IsBuildAssetBundles == true)
            {
                EditorApplication.delayCall += DelayCallBuildAssetBundles;
            }
        }

        static void DelayCallBuildAssetBundles()
        {
            EditorApplication.delayCall -= DelayCallBuildAssetBundles;
            System.GC.Collect();
            AutoBuildAssetBundles();
        }


        static void AutoBuildAssetBundles()
        {
            string[] path = AssetDatabase.FindAssets("t:XAssetSetting");
            BuildAssetBundlesIndex += 1;
            if (BuildAssetBundlesIndex >= path.Length)
            {
                BuilMainState = "None";
                IsBuildAssetBundles = false;
                Debug.Log("<color=#0007FF>" + "AssetBundles Done" + "</color>");
                return;
            }
            string curPath = path[BuildAssetBundlesIndex];
            curPath = AssetDatabase.GUIDToAssetPath(curPath);
            XAssetSetting obj = UnityEditor.AssetDatabase.LoadAssetAtPath(curPath, typeof(object)) as XAssetSetting;
            BuildAssetBundles(obj.RulePath, obj.ManifestPath, obj.RootPath);
            Debug.Log("<color=#FF0072>" + curPath + "</color>");
        }

        [MenuItem("Assets/XAsset/Copy AssetBundles to StreamingAssets")]
        public static bool CopyAssetBundlesToStreamingAssets()
        {
            if (EditorApplication.isCompiling)
            {

                return false;
            }
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, XEditorUtility.AssetBundlesOutputPath));
            UnityEditor.AssetDatabase.Refresh();
            return true;

        }

        [MenuItem("Assets/XAsset/Build Player")]
        public static void BuildPlayer()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            BuildScript.BuildStandalonePlayer();
        }

        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            EditorApplication.wantsToQuit -= Quit;
            EditorApplication.wantsToQuit += Quit;
        }

        static bool Quit()
        {
            BuilMainState = "None";
            IsBuildAssetBundles = false;
            return true;
        }
    }
}