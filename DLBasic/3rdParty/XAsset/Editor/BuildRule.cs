using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XAsset.Editor
{
    public class DependenciesStruct
    {
        public List<string> bundles = new List<string>();
        public List<string> directories = new List<string>();
        public List<string> files = new List<string>();
        public string packgeTag = null;

        public void AddBundle(string name)
        {
            if (!bundles.Contains(name))
            {
                bundles.Add(name);
            }
        }
        public void AddDir(string name)
        {
            if (!directories.Contains(name))
            {
                directories.Add(name);
            }
        }
        public void AddFile(string name)
        {
            if (!files.Contains(name))
            {
                files.Add(name);
            }
        }
    }

    public abstract class BuildRule
    {
        protected static List<string> packedAssets = new List<string>();
        protected static List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        static List<BuildRule> rules = new List<BuildRule>();
        protected static Dictionary<string, List<string>> ABDependencies = new Dictionary<string, List<string>>();
        protected static Dictionary<string, DependenciesStruct> ABAtlas = new Dictionary<string, DependenciesStruct>();
        protected static Dictionary<string, DependenciesStruct> allDependencies = new Dictionary<string, DependenciesStruct>();
        static BuildRule()
        {

        }

        public static List<AssetBundleBuild> GetBuilds(string manifestPath,string rulesini)
        {
            packedAssets.Clear();
            builds.Clear();
            allDependencies.Clear();
            ABDependencies.Clear();
            ABAtlas.Clear();

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = "manifest";
            build.assetNames = new string[] { manifestPath };
            builds.Add(build);

            if (File.Exists(rulesini))
            {
                LoadRules(rulesini);
            }
            else
            {
                rules.Add(new BuildAssetsWithAssetBundleName("Assets/GameConfig", "*.prefab", SearchOption.AllDirectories, "test"));
                SaveRules(rulesini);
            }

            foreach (var item in rules)
            {
                item.CollectDependencies();
            }

            BuildDependenciesAssets();

            foreach (var item in rules)
            {
                item.Build();
            }

#if ENABLE_ATLAS
			BuildAtlas(); 
#endif
            UnityEditor.EditorUtility.ClearProgressBar();

            return builds;
        }

        static void BuildAtlas()
        {
            foreach (var item in builds)
            {
                var assets = item.assetNames;
                foreach (var asset in assets)
                {
                    var importer = AssetImporter.GetAtPath(asset);
                    if (importer is TextureImporter)
                    {
                        var ti = importer as TextureImporter;
                        if (ti.textureType == TextureImporterType.Sprite)
                        {
                            var tex = AssetDatabase.LoadAssetAtPath<Texture>(asset);
                            if (tex.texelSize.x >= 1024 || tex.texelSize.y >= 1024)
                            {
                                continue;
                            }

                            var tag = item.assetBundleName.Replace("/", "_");
                            if (! tag.Equals(ti.spritePackingTag))
                            {
                                var settings = ti.GetPlatformTextureSettings(XEditorUtility.GetPlatformName());
                                settings.format = ti.GetAutomaticFormat(XEditorUtility.GetPlatformName());
                                settings.overridden = true;
                                ti.SetPlatformTextureSettings(settings);
                                ti.spritePackingTag = tag;
                                ti.SaveAndReimport();
                            }
                        }
                    }
                }
 
            }
        }

        static void SaveRules(string rulesini)
        {
            using (var s = new StreamWriter(rulesini))
            {
                foreach (var item in rules)
                {
                    s.WriteLine("[{0}]", item.GetType().Name);
                    s.WriteLine("searchPath=" + item.searchPath);
                    s.WriteLine("searchPattern=" + item.searchPattern);
                    s.WriteLine("searchOption=" + item.searchOption);
                    s.WriteLine("bundleDir=" + item.bundleDir);
                    s.WriteLine("bundleName=" + item.bundleName);
                    s.WriteLine();
                }
                s.Flush();
                s.Close();
            }
        }

        static void LoadRules(string rulesini)
        {
            using (var s = new StreamReader(rulesini))
            {
                rules.Clear();

                string line = null;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty || line.StartsWith("#", StringComparison.CurrentCulture) || line.StartsWith("//", StringComparison.CurrentCulture))
                    {
                        continue;
                    }
                    if (line.Length > 2 && line[0] == '[' && line[line.Length - 1] == ']')
                    {
                        var name = line.Substring(1, line.Length - 2);
                        var searchPath = s.ReadLine().Split('=')[1];
                        var searchPattern = s.ReadLine().Split('=')[1];
                        var searchOption = s.ReadLine().Split('=')[1];
                        var bundleDir = s.ReadLine().Split('=')[1];
                        var bundleName = s.ReadLine().Split('=')[1];
                        var type = typeof(BuildRule).Assembly.GetType("XAsset.Editor." + name);
                        if (type != null)
                        {
                            var rule = Activator.CreateInstance(type) as BuildRule;
                            rule.searchPath = searchPath;
                            rule.searchPattern = searchPattern;
                            rule.searchOption = (SearchOption)Enum.Parse(typeof(SearchOption), searchOption);
                            rule.bundleDir = bundleDir == null ? "" : bundleDir;
                            rule.bundleName = bundleName;
                            rules.Add(rule);
                        }
                    }
                }
            }
        }

        protected static List<string> GetFilesWithoutDirectories(string prefabPath, string searchPattern, SearchOption searchOption)
        {
            string[] spArr = searchPattern.Split('|');
            const string metastr = ".meta";
            List<string> items = new List<string>();
            for (int i = 0; i < spArr.Length; i++)
            {
                var files = Directory.GetFiles(prefabPath, spArr[i], searchOption);
                foreach (var item in files)
                {
                    if (item.EndsWith(metastr)) continue;
                    var assetPath = item.Replace('\\', '/');
                    if (!Directory.Exists(assetPath))
                    {
                        items.Add(assetPath);
                    }
                }
            }
            return items;
        }

        protected static string GetPackingTag(string asset)
        {
            var importer = AssetImporter.GetAtPath(asset);
            if (importer is TextureImporter)
            {
                var ti = importer as TextureImporter;
                if (ti.textureType == TextureImporterType.Sprite)
                {
                    return ti.spritePackingTag;
                }
            }
            return null;
        }

        protected static void BuildDependenciesAssets()
        {
            Dictionary<string, List<string>> bundles = new Dictionary<string, List<string>>();
            foreach (var item in allDependencies)
            {
                var assetPath = item.Key;
                if (!assetPath.EndsWith(".cs", StringComparison.CurrentCulture))
                {
                    if (packedAssets.Contains(assetPath))
                    {
                        continue;
                    }
                    if (assetPath.EndsWith(".shader", StringComparison.CurrentCulture))
                    {
                        List<string> list = null;
                        if (!bundles.TryGetValue("shader/shaders", out list))
                        {
                            list = new List<string>();
                            bundles.Add("shader/shaders", list);
                        }
                        if (!list.Contains(assetPath))
                        {
                            list.Add(assetPath);
                            packedAssets.Add(assetPath);
                        }
                    }
                    else
                    {
                        string name = null;
                        if (!string.IsNullOrEmpty(item.Value.packgeTag))
                        {
                            DependenciesStruct aa = ABAtlas[item.Value.packgeTag];
                            if (aa.bundles.Count > 1)
                            {
                                if (aa.directories.Count > 1)
                                {
                                    name = StandardPath("shared/spritepacker/" + item.Value.packgeTag);
                                }
                                else
                                {
                                    name = StandardPath(aa.directories[0] + "shared/spritepacker/" + item.Value.packgeTag);
                                }
                            }
                        }
                        else if (item.Value.bundles.Count > 1)
                        {
                            if (item.Value.directories.Count > 1)
                            {
                                name = "shared/" + BuildAssetBundleNameWithAssetPath(Path.GetDirectoryName(assetPath));
                            }
                            else
                            {
                                name = item.Value.directories[0] + "shared/" + BuildAssetBundleNameWithAssetPath(Path.GetDirectoryName(assetPath));
                            }
                        }
                        if (name != null)
                        {
                            List<string> list = null;
                            if (!bundles.TryGetValue(name, out list))
                            {
                                list = new List<string>();
                                bundles.Add(name, list);
                            }
                            if (!list.Contains(assetPath))
                            {
                                list.Add(assetPath);
                                packedAssets.Add(assetPath);
                            }
                        }
                    }
                }
            }
            foreach (var item in bundles)
            {
                AssetBundleBuild build = new AssetBundleBuild();
				build.assetBundleName = item.Key + "_" + item.Value.Count;
                build.assetNames = item.Value.ToArray();
                builds.Add(build);
            }
        }

        protected static List<string> GetDependenciesWithoutShared(string abfile)
        {
            List<string> assetNames = new List<string>();
            if (!ABDependencies.ContainsKey(abfile)) return assetNames;
            var assets = ABDependencies[abfile];
            foreach (var assetPath in assets)
            {
                if (assetPath.Contains(".prefab") || packedAssets.Contains(assetPath) || assetPath.EndsWith(".cs", StringComparison.CurrentCulture) || assetPath.EndsWith(".shader", StringComparison.CurrentCulture))
                {
                    continue;
                }
                if (allDependencies[assetPath].bundles.Count == 1)
                {
                    assetNames.Add(assetPath);
                }
            }
            return assetNames;
        }
        protected static List<string> GetFilesWithoutShared(string abfile)
        {
            List<string> assetNames = new List<string>();
            if (!ABDependencies.ContainsKey(abfile)) return assetNames;
            var assets = ABDependencies[abfile];
            foreach (var assetPath in assets)
            {
                if (packedAssets.Contains(assetPath)) continue;
                if (assetPath.EndsWith(".cs", StringComparison.CurrentCulture)) continue;
                assetNames.Add(assetPath);
            }
            return assetNames;
        }

        protected abstract void CollectDependencies();

        protected static List<string> GetFilesWithoutPacked(string searchPath, string searchPattern, SearchOption searchOption)
        {
            var files = GetFilesWithoutDirectories(searchPath, searchPattern, searchOption);
            var filesCount = files.Count;
            var removeAll = files.RemoveAll((string obj) =>
            {
                return packedAssets.Contains(obj);
            });
            XDebug.Log(string.Format("RemoveAll {0} size: {1}", removeAll, filesCount));

            return files;
        }

        protected static string BuildAssetBundleNameWithAssetPath(string assetPath)
        {
            return Path.Combine(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath)).Replace('\\', '/').ToLower();
        }

        protected static string StandardPath(string path)
        {
           return path.Replace('\\', '/').ToLower();
        }

        public string searchPath;
        public string searchPattern;
        public SearchOption searchOption = SearchOption.AllDirectories;
        public string bundleDir;
        public string bundleName;

        protected BuildRule()
        {

        }

        protected BuildRule(string path, string pattern, SearchOption option)
        {
            searchPath = path;
            searchPattern = pattern;
            searchOption = option;
        }

        public abstract void Build();

    }

    public class BuildAssetsWithAssetBundleName : BuildRule
    {
        public BuildAssetsWithAssetBundleName()
        {

        }

        public BuildAssetsWithAssetBundleName(string path, string pattern, SearchOption option, string assetBundleName) : base(path, pattern, option)
        {
        }

        protected override void CollectDependencies()
        {
            List<string> files = GetFilesWithoutDirectories(searchPath, searchPattern, searchOption);

            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                var dependencies = AssetDatabase.GetDependencies(item);
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, files.Count), item, i * 1f / files.Count))
                {
                    break;
                }
                string bundlePath = bundleDir + bundleName;
                if (!ABDependencies.ContainsKey(bundlePath))
                {
                    ABDependencies[bundlePath] = new List<string>();
                }

                foreach (var assetPath in dependencies)
                {
                    string pt = GetPackingTag(assetPath);
                    if (!string.IsNullOrEmpty(pt))
                    {
                        if (!ABAtlas.ContainsKey(pt))
                        {
                            ABAtlas[pt] = new DependenciesStruct();
                        }
                        var aa = ABAtlas[pt];
                        if (!aa.files.Contains(assetPath))
                        {
                            ABAtlas[pt].files.Add(assetPath);
                        }
                        aa.AddBundle(bundlePath);
                        aa.AddDir(bundleDir);
                    }

                    if (!ABDependencies[bundlePath].Contains(assetPath))
                    {
                        ABDependencies[bundlePath].Add(assetPath);
                    }

                    if (!allDependencies.ContainsKey(assetPath))
                    {
                        allDependencies[assetPath] = new DependenciesStruct();
                    }

                    var ds = allDependencies[assetPath];
                    ds.AddBundle(bundlePath);
                    ds.AddDir(bundleDir);
                    ds.AddFile(item);
                    ds.packgeTag = pt;
                }
            }
        }

        public override void Build()
        {
            string bundlePath = bundleDir + bundleName;
            var files = GetFilesWithoutShared(bundlePath);
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundlePath;
            build.assetNames = files.ToArray();
            builds.Add(build);
            packedAssets.AddRange(files);
        }
    }

    public class BuildAssetsWithDirectroyName : BuildRule
    {
        public BuildAssetsWithDirectroyName()
        {

        }

        public BuildAssetsWithDirectroyName(string path, string pattern, SearchOption option) : base(path, pattern, option)
        {
        }

        protected override void CollectDependencies()
        {
            List<string> files = GetFilesWithoutDirectories(searchPath, searchPattern, searchOption);
            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                var dependencies = AssetDatabase.GetDependencies(item);
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, files.Count), item, i * 1f / files.Count))
                {
                    break;
                }
                string bundlePath = StandardPath(bundleDir + Path.GetDirectoryName(item));
                if (!ABDependencies.ContainsKey(bundlePath))
                {
                    ABDependencies[bundlePath] = new List<string>();
                }
                foreach (var assetPath in dependencies)
                {
                    string pt = GetPackingTag(assetPath);
                    if (!string.IsNullOrEmpty(pt))
                    {
                        if (!ABAtlas.ContainsKey(pt))
                        {
                            ABAtlas[pt] = new DependenciesStruct();
                        }
                        var aa = ABAtlas[pt];
                        if (!aa.files.Contains(assetPath))
                        {
                            ABAtlas[pt].files.Add(assetPath);
                        }
                        aa.AddBundle(bundlePath);
                        aa.AddDir(bundleDir);
                    }

                    if (!ABDependencies[bundlePath].Contains(assetPath))
                    {
                        ABDependencies[bundlePath].Add(assetPath);
                    }

                    if (!allDependencies.ContainsKey(assetPath))
                    {
                        allDependencies[assetPath] = new DependenciesStruct();
                    }

                    var ds = allDependencies[assetPath];
                    ds.AddBundle(bundlePath);
                    ds.AddDir(bundleDir);
                    ds.AddFile(item);
                    ds.packgeTag = pt;
                }
            }
        }

        public override void Build()
        {
            var files = GetFilesWithoutPacked(searchPath, searchPattern, searchOption);

            Dictionary<string, List<string>> bundles = new Dictionary<string, List<string>>();
            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, files.Count), item, i * 1f / files.Count))
                {
                    break;
                }
                var path = StandardPath(bundleDir + Path.GetDirectoryName(item));
                if (!bundles.ContainsKey(path))
                {
                    bundles[path] = GetFilesWithoutShared(path);
                }
            }

            int count = 0;
            foreach (var item in bundles)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = string.Format("{0}_{1}", item.Key, item.Value.Count);
                build.assetNames = item.Value.ToArray();
                packedAssets.AddRange(build.assetNames);
                builds.Add(build);
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", count, bundles.Count), build.assetBundleName, count * 1f / bundles.Count))
                {
                    break;
                }
                count++;
            }
        }
    }

    public class BuildAssetsWithFilename : BuildRule
    {
        public BuildAssetsWithFilename()
        {

        }

        public BuildAssetsWithFilename(string path, string pattern, SearchOption option) : base(path, pattern, option)
        {
        }

        protected override void CollectDependencies()
        {
            throw new NotImplementedException();
        }

        public override void Build()
        {
            var files = GetFilesWithoutPacked(searchPath, searchPattern, searchOption);

            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", i, files.Count), item, i * 1f / files.Count))
                {
                    break;
                }
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = BuildAssetBundleNameWithAssetPath(item);
                var assetNames = GetDependenciesWithoutShared(item);
                assetNames.Add(item);
                build.assetNames = assetNames.ToArray();
                packedAssets.AddRange(assetNames);
                builds.Add(build);
            }
        }
    }

	public class BuildAssetsWithScenes : BuildRule
    {
        public BuildAssetsWithScenes()
        {

        }

        public BuildAssetsWithScenes(string path, string pattern, SearchOption option) : base(path, pattern, option)
        {

        }

        protected override void CollectDependencies()
        {
            List<string> files = GetFilesWithoutDirectories(searchPath, searchPattern, searchOption);

            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                var dependencies = AssetDatabase.GetDependencies(item);
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, files.Count), item, i * 1f / files.Count))
                {
                    break;
                }
                string bundlePath = StandardPath(bundleDir + Path.GetDirectoryName(item));
                if (!ABDependencies.ContainsKey(bundlePath))
                {
                    ABDependencies[bundlePath] = new List<string>();
                }

                foreach (var assetPath in dependencies)
                {
                    string pt = GetPackingTag(assetPath);
                    if (!string.IsNullOrEmpty(pt))
                    {
                        if (!ABAtlas.ContainsKey(pt))
                        {
                            ABAtlas[pt] = new DependenciesStruct();
                        }
                        var aa = ABAtlas[pt];
                        if (!aa.files.Contains(assetPath))
                        {
                            ABAtlas[pt].files.Add(assetPath);
                        }
                        aa.AddBundle(bundlePath);
                        aa.AddDir(bundleDir);
                    }

                    if (!ABDependencies[bundlePath].Contains(assetPath))
                    {
                        ABDependencies[bundlePath].Add(assetPath);
                    }

                    if (!allDependencies.ContainsKey(assetPath))
                    {
                        allDependencies[assetPath] = new DependenciesStruct();
                    }

                    var ds = allDependencies[assetPath];
                    ds.AddBundle(bundlePath);
                    ds.AddDir(bundleDir);
                    ds.AddFile(item);
                    ds.packgeTag = pt;
                }
            }
        }

        public override void Build()
        {
            var files = GetFilesWithoutPacked(searchPath, searchPattern, searchOption);

            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", i, files.Count), item, i * 1f / files.Count))
                {
                    break;
                }
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = StandardPath(bundleDir + Path.GetDirectoryName(item));
                build.assetNames = new [] { item };
                packedAssets.AddRange(build.assetNames);
                builds.Add(build);
            }
        }
    }

}