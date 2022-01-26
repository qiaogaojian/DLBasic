using UnityEditor;
using UnityEngine;

namespace XAsset.Editor
{
    public class XEditorUtility : Utility
    { 
        [InitializeOnLoadMethod]
        static void Init()
        { 
			//XDebug.Log("Init->activeBundleMode: " + ActiveBundleMode);
        } 

    }
}