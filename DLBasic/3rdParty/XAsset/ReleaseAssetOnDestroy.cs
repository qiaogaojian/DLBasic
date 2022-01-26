using System.Collections.Generic;
using UnityEngine;

namespace XAsset
{
    public class ReleaseAssetOnDestroy : MonoBehaviour
    { 
        Dictionary<string, Asset> assets = new Dictionary<string, Asset>();
        public static ReleaseAssetOnDestroy Register(GameObject go, Asset asset)
        {
            ReleaseAssetOnDestroy component = Init(go);
            if (!component.assets.ContainsKey(asset.assetPath))
            {
                component.assets[asset.assetPath] = asset;
            }
            return component;
        }
        public static T Load<T>(GameObject go, string path, Assets assets) where T : UnityEngine.Object
        {
            ReleaseAssetOnDestroy component = Init(go);
            if (component.assets.ContainsKey(path))
            {
                return (T)(component.assets[path].asset);
            }
            Asset ass = assets.Load<T>(path);
            if (ass.asset == null) return default(T);
            component.assets[path] = ass;
            return (T)(ass.asset);
        }
        static ReleaseAssetOnDestroy Init(GameObject go)
        {
            ReleaseAssetOnDestroy component = go.GetComponent<ReleaseAssetOnDestroy>();
            if (component == null)
            {
                component = go.AddComponent<ReleaseAssetOnDestroy>();
            }
            return component;
        }
        private void OnDestroy()
        {
            foreach (var item in assets)
            {
                item.Value.Release();
            }
            assets.Clear();
        }
    }
}
