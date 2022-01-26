using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XAsset
{
    [CreateAssetMenu(fileName = "XAssetSetting", menuName = "XAsset/XAssetSetting")]
    public class XAssetSetting : ScriptableObject
    {
        public string RulePath;
        public string ManifestPath;
        public string RootPath = "";

#if UNITY_EDITOR
        [System.NonSerialized]
        public Object Rule;
        [System.NonSerialized]
        public Object Manifest;
#endif

    }
}