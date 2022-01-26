using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XAsset
{
    public static class AssetExtensions
    {
        public static T LoadXAsset<T>(this GameObject go, string path, Assets assets) where T : UnityEngine.Object
        {
            return ReleaseAssetOnDestroy.Load<T>(go, path, assets);
        }
        public static void ReleaseAssetDestroy(this GameObject go, Asset asset)
        {
            ReleaseAssetOnDestroy.Register(go, asset);
        }
    }
}