using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XAsset
{
    public sealed class Assets : MonoBehaviour
    {
        public static Assets Creat(string path = "")
        {
            GameObject go = new GameObject("Assets_" + path);
            Assets re = go.AddComponent<Assets>();
            re.Init(path);
            return re;
        }

        bool initialized = false;
        Manifest manifest = new Manifest();
        public string[] allAssetNames { get { return manifest.allAssets; } }
        public string[] allBundleNames { get { return manifest.allBundles; } }
        public string GetBundleName(string assetPath) { return manifest.GetBundleName(assetPath); }
        public string GetAssetName(string assetPath) { return manifest.GetAssetName(assetPath); }

        Bundles bundles = null;
        public Bundles Bundles
        {
            get
            {
                return bundles;
            }
        }

        string rootPath = "";

        void Init(string path)
        {
            if (initialized)
                return;

            rootPath = path;
            initialized = true;
#if UNITY_EDITOR
            if (Utility.ActiveBundleMode)
            {
                InitializeBundle();
            }
#else
			InitializeBundle();
#endif
        }
        #region DefaultLoad
        string dataPath = null;
        string hotLocalPath = null;
        string GetHotLocalPath()
        {
            if (string.IsNullOrEmpty(hotLocalPath))
            {
                hotLocalPath = Path.Combine(Path.Combine(Application.persistentDataPath, XAsset.Utility.AssetBundlesOutputPath), XAsset.Utility.GetPlatformName(), rootPath) + "/";
            }
            return hotLocalPath;
        }
        string XAssetGetBundlePath(string bundleName)
        {
            string re = null;
            string file = Path.Combine(GetHotLocalPath(), bundleName);
            if (File.Exists(file))
            {
                re = GetHotLocalPath();
            }
            else
            {
                re = null;
            }

            if (string.IsNullOrEmpty(re))
            {
                re = dataPath;
            }

            return re;

        }
        #endregion
        public Asset Load<T>(string path) where T : Object
        {
            return Load(path, typeof(T));
        }

        public Asset Load(string path, System.Type type)
        {
            return LoadInternal(path, type, false);
        }

        public Asset LoadAsync<T>(string path)
        {
            return LoadAsync(path, typeof(T));
        }

        public Asset LoadAsync(string path, System.Type type)
        {
            return LoadInternal(path, type, true);
        }

        public Bundle LoadBundle(string path)
        {
#if UNITY_EDITOR
            if (Utility.ActiveBundleMode)
            {
                return bundles.Load(path);
            }
            else
            {
                return null;
            }
#else
			return bundles.Load(path);
#endif
        }
        public Bundle LoadBundleByAssetPath(string path)
        {
#if UNITY_EDITOR
            if (Utility.ActiveBundleMode)
            {
                return bundles.Load(GetBundleName(path));
            }
            else
            {
                return null;
            }
#else
			return bundles.Load(GetBundleName(path));
#endif
        }
        public Bundle LoadBundleAsyncByAssetPath(string path)
        {
#if UNITY_EDITOR
            if (Utility.ActiveBundleMode)
            {
                return bundles.LoadAsync(GetBundleName(path));
            }
            else
            {
                return null;
            }
#else
			return bundles.LoadAsync(GetBundleName(path));
#endif
        }

        public void Unload(Asset asset)
        {
            if (asset == null) return;
            asset.Release();
        }

        public bool ContainsAsset(string path)
        {
#if UNITY_EDITOR
            if (Utility.ActiveBundleMode)
            {
                return manifest.ContainsAsset(path);
            }
            else
            {
                return File.Exists(path);
            }
#else
			return manifest.ContainsAsset(path);
#endif
        }
        void InitializeBundle()
        {
            string relativePath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformName(), rootPath);
            dataPath =
#if UNITY_EDITOR
                relativePath + "/";
#else
				Path.Combine(Application.streamingAssetsPath, relativePath) + "/"; 
#endif
            bundles = new Bundles(rootPath, XAssetGetBundlePath);

            var bundle = bundles.Load("manifest");
            if (bundle != null)
            {
                var asset = bundle.LoadAsset<TextAsset>("Manifest.txt");
                if (asset != null)
                {
                    using (var reader = new StringReader(asset.text))
                    {
                        manifest.Load(reader);
                        reader.Close();
                    }
                    bundle.Release();
                    Resources.UnloadAsset(asset);
                    asset = null;
                }
            }
        }

        Asset CreateAssetRuntime(string path, System.Type type, bool asyncMode)
        {
            if (!manifest.ContainsAsset(path))
                return null;

            if (asyncMode)
                return new BundleAssetAsync(path, type, this);
            return new BundleAsset(path, type, this);
        }

        const string ResourcesSplit = "Resources";
        const char PathPoint = '.';

        Asset LoadInternal(string path, System.Type type, bool asyncMode, string bundlepath = null)
        {
            int index = path.IndexOf(ResourcesSplit);

            if (index >= 0)
            {
                path = path.Substring(index + 10);
                index = path.IndexOf(PathPoint);
                if (index >= 0)
                {
                    path = path.Remove(index);
                }
            }
            Asset asset = assets.Find(obj => { return obj.assetPath == path; });
            if (asset == null)
            {
                if (path.StartsWith("Assets/"))
                {
#if UNITY_EDITOR
                    if (Utility.ActiveBundleMode)
                    {
                        asset = CreateAssetRuntime(path, type, asyncMode);
                    }
                    else
                    {
                        asset = new Asset(path, type, this);
                    }
#else
				    asset = CreateAssetRuntime (path, type, asyncMode);
#endif
                }
                else
                {
                    if (asyncMode)
                    {
                        asset = new ResourceAssetAsync(path, type, this);
                    }
                    else
                    {
                        asset = new ResourceAsset(path, type, this);
                    }
                }
                if (asset == null) return new Asset(path, type, this);
                assets.Add(asset);
                asset.Load();
            }
            asset.Retain();
            return asset;
        }

        public readonly List<Asset> assets = new List<Asset>();

        System.Collections.IEnumerator gc = null;
        System.Collections.IEnumerator GC()
        {
            yield return 0;
            yield return Resources.UnloadUnusedAssets();
        }
        public void Update()
        {
            bool removed = false;
            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                if (!asset.Update() && asset.references <= 0)
                {
                    asset.Unload();
                    asset = null;
                    assets.RemoveAt(i);
                    i--;
                    removed = true;
                }
            }
            if (removed)
            {
                if (gc != null)
                {
                    StopCoroutine(gc);
                }
                gc = GC();
                StartCoroutine(gc);
            }

            if (bundles != null) bundles.Update();
        }
        public void ClearAndDestroy()
        {
            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                asset.Unload();
            }
            if (bundles != null) bundles.ClearAll();
            GameObject.Destroy(this.gameObject);
            Resources.UnloadUnusedAssets();
        }

        public void Clear(Asset asset)
        {
            if (assets.Contains(asset))
            {
                asset.Unload(true);
                assets.Remove(asset);
                asset = null;

            }
        }
    }
}
