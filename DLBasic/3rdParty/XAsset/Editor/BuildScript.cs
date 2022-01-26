using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XAsset.Editor
{
    public static class BuildScript
    {
        [InitializeOnLoadMethod]
        public static void Clear()
        {
            UnityEditor.EditorUtility.ClearProgressBar();
        }

        static public string CreateAssetBundleDirectory(bool clear = false, string outpath = "")
        {
            // Choose the output path according to the build target.
            string outputPath = Path.Combine(XEditorUtility.AssetBundlesOutputPath, XEditorUtility.GetPlatformName(), outpath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            else
            {
                if (clear)
                {
                    Directory.Delete(outputPath,true);
                    Directory.CreateDirectory(outputPath);
                }
            }

            return outputPath;
        }

        public static void BuildAssetBundles(List<AssetBundleBuild> builds,string outpath = "")
        {
            // Choose the output path according to the build target.
            string outputPath = CreateAssetBundleDirectory(true, outpath);

            var options = BuildAssetBundleOptions.None;

            //            bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
            //#if UNITY_TVOS
            //            shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
            //#endif
            //            if (shouldCheckODR)
            //            {
            //#if ENABLE_IOS_ON_DEMAND_RESOURCES
            //                if (PlayerSettings.iOS.useOnDemandResources)
            //                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
            //#endif
            //#if ENABLE_IOS_APP_SLICING
            //                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
            //#endif
            //            }
            if (builds == null || builds.Count == 0)
            {
                //@TODO: use append hash... (Make sure pipeline works correctly with it.)
                BuildPipeline.BuildAssetBundles(outputPath, options, EditorUserBuildSettings.activeBuildTarget);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);
            }
        }

        public static void BuildPlayerWithoutAssetBundles()
        {
            var outputPath = UnityEditor.EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                XDebug.Log("Nothing to build.");
                return;
            }

            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;

#if UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
#else
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = levels;
            buildPlayerOptions.locationPathName = outputPath + targetName;
            buildPlayerOptions.assetBundleManifestPath = GetAssetBundleManifestFilePath();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            buildPlayerOptions.options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
#endif
        }

        public static void BuildStandalonePlayer()
        {
            var outputPath = UnityEditor.EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                XDebug.Log("Nothing to build.");
                return;
            }

            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;

            CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, XEditorUtility.AssetBundlesOutputPath));
            AssetDatabase.Refresh();

#if UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
#else
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = levels;
            buildPlayerOptions.locationPathName = outputPath + targetName;
            buildPlayerOptions.assetBundleManifestPath = GetAssetBundleManifestFilePath();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            buildPlayerOptions.options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
#endif
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            if (target == BuildTarget.Android)
            {
                return "/" + name + PlayerSettings.Android.bundleVersionCode + ".apk";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return "/" + name + PlayerSettings.Android.bundleVersionCode + ".exe";
            } 
            if (target == BuildTarget.iOS || target == BuildTarget.StandaloneOSX)
            {
                return "/" + name + ".app";
            } 
            if (target == BuildTarget.iOS)
            {
                return "/iOS";
            }
            XDebug.Log("Target not implemented.");
            return null;
            //if (target == BuildTarget.WebGL)
            //{
            //    return "/web";
            //}

        }

        static public void CopyAssetBundlesTo(string outputPath)
        {
            // Clear streaming assets folder.
            //            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string outputFolder = XEditorUtility.GetPlatformName();

            // Setup the source folder for assetbundles.
            var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, XEditorUtility.AssetBundlesOutputPath), outputFolder);
            if (!System.IO.Directory.Exists(source))
                XDebug.Log("No assetBundle output folder, try to build the assetBundles first.");

            // Setup the destination folder for assetbundles.
            var destination = System.IO.Path.Combine(outputPath, outputFolder);
            if (System.IO.Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(destination);

            FileUtil.CopyFileOrDirectory(source, destination);
        }

        static string[] GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        static string GetAssetBundleManifestFilePath()
        {
            var relativeAssetBundlesOutputPathForPlatform = Path.Combine(XEditorUtility.AssetBundlesOutputPath, XEditorUtility.GetPlatformName());
            return Path.Combine(relativeAssetBundlesOutputPathForPlatform, XEditorUtility.GetPlatformName()) + ".manifest";
        }

        static void SaveManifest(string path, List<AssetBundleBuild> builds)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var writer = new StreamWriter(path))
            {
                foreach (var item in builds)
                {
                    writer.WriteLine(item.assetBundleName + ":");
                    foreach (var asset in item.assetNames)
                    {

                        if (asset.Contains(".svn") || asset.Contains("DS_Store")) continue;

                        writer.WriteLine(string.Format("\t{0}", asset));
                    }
                    writer.WriteLine();
                }
                writer.Flush();
                writer.Close();
            }
        }

        public static void BuildManifest(string path, List<AssetBundleBuild> builds, bool forceRebuild = false)
        {

            Manifest manifest = new Manifest();

            if (File.Exists(path))
            {
                using (var reader = new StreamReader(path))
                {
                    manifest.Load(reader);
                    reader.Close();
                }
            }

            Dictionary<string, string> newpaths = new Dictionary<string, string>();
            List<string> bundles = new List<string>();
            List<string> assets = new List<string>();
            bool dirty = false;
            if (builds.Count > 0)
            {
                foreach (var item in builds)
                {
                    bundles.Add(item.assetBundleName);
                    foreach (var assetPath in item.assetNames)
                    {

                        if (assetPath.Contains(".svn") || assetPath.Contains("DS_Store")) continue;

                        newpaths[assetPath] = item.assetBundleName;




                        assets.Add(assetPath + ":" + (bundles.Count - 1));
                    }
                }
            }

            if (manifest.allAssets != null && newpaths.Count == manifest.allAssets.Length)
            {
                foreach (var item in newpaths)
                {
                    if (!manifest.ContainsAsset(item.Key) || !manifest.GetBundleName(item.Key).Equals(newpaths[item.Key]))
                    {
                        dirty = true;
                        break;
                    }
                }
            }
            else
            {
                dirty = true;
            }

            if (forceRebuild || dirty || !File.Exists(path))
            {
                SaveManifest(path, builds);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();
            }

            XDebug.Log(string.Format("[BuildScript] BuildManifest with {0} assets and {1} bundels.", assets.Count, bundles.Count));
        }
    }
}