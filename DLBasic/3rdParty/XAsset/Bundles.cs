using System.Collections.Generic;
using UnityEngine;

namespace XAsset
{
    public delegate string OverrideDataPathDelegate(string bundleName);

    public class Bundles
    {
        event OverrideDataPathDelegate overrideBaseDownloadingURL;
        public AssetBundleManifest manifest { get; private set; }
        public Bundles(string abmanifest, OverrideDataPathDelegate loadcall)
        {
            overrideBaseDownloadingURL = loadcall;
            if (string.IsNullOrEmpty(abmanifest)) return;
            var request = LoadInternal(abmanifest, false);
            if (request == null || request.error != null)
            {
                return;
            }
            manifest = request.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        public string GetDataPath(string bundleName)
        {
            if (overrideBaseDownloadingURL != null)
            {
                foreach (OverrideDataPathDelegate method in overrideBaseDownloadingURL.GetInvocationList())
                {
                    string res = method(bundleName);
                    if (res != null)
                        return res;
                }
            }
            Debug.LogError("No Find AssetBundle:" + bundleName);
            return null;
        }
        public Bundle Load(string assetBundleName)
        {
            return LoadInternal(assetBundleName, false);
        }
        public Bundle LoadAsync(string assetBundleName)
        {
            return LoadInternal(assetBundleName, true);
        }
        public void Unload(Bundle bundle)
        {
            bundle.Release();
        }
        void UnloadDependencies(Bundle bundle)
        {
            foreach (var item in bundle.dependencies)
            {
                item.Release();
            }
            bundle.dependencies.Clear();
        }
        void LoadDependencies(Bundle bundle, string assetBundleName, bool asyncRequest)
        {
            if (manifest == null) return;
            var dependencies = manifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length > 0)
            {
                foreach (var item in dependencies)
                {
                    bundle.dependencies.Add(LoadInternal(item, asyncRequest));
                }
            }
        }
        Bundle LoadInternal(string assetBundleName, bool asyncRequest)
        {
            var url = GetDataPath(assetBundleName) + assetBundleName;
            Bundle bundle;
            if (!bundles.TryGetValue(assetBundleName, out bundle))
            {
                var hash = manifest == null ? new Hash128(1, 0, 0, 0) : manifest.GetAssetBundleHash(assetBundleName);
                if (bundle == null)
                {
                    if (url.StartsWith("http://") ||
                        url.StartsWith("https://") ||
                        url.StartsWith("file://") ||
                        url.StartsWith("ftp://"))
                    {
                        bundle = new BundleWWW(url, hash);
                    }
                    else
                    {
                        if (asyncRequest)
                        {
                            bundle = new BundleAsync(url, hash);
                        }
                        else
                        {
                            bundle = new Bundle(url, hash);
                        }
                    }
                    bundle.name = assetBundleName;
                    bundles.Add(assetBundleName, bundle);
                    bundle.Load();
                    LoadDependencies(bundle, assetBundleName, asyncRequest);
                }
            }
            bundle.Retain();
            return bundle;
        }
        public Dictionary<string, Bundle> bundles = new Dictionary<string, Bundle>();
        internal void Update()
        {
            List<Bundle> bundleToDestroy = new List<Bundle>();
            foreach (var item in bundles)
            {
                if (item.Value.isDone && item.Value.references <= 0)
                {
                    bundleToDestroy.Add(item.Value);
                }
            }

            for (int i = 0; i < bundleToDestroy.Count; i++)
            {
                var bundle = bundleToDestroy[i];
                bundles.Remove(bundle.name);
                bundle.Unload();
                UnloadDependencies(bundle);
                bundle = null;
            }
            bundleToDestroy.Clear();
        }
        public void ClearAll()
        {
            foreach (var item in bundles)
            {
                item.Value.Unload();
            }
            bundles.Clear();
        }
        public void Clear(Bundle bundle)
        {
            if (bundles.ContainsKey(bundle.name))
            {
                bundles.Remove(bundle.name);
                bundle.Unload();
                UnloadDependencies(bundle);
                bundle = null;
            }
        }
    }
}
