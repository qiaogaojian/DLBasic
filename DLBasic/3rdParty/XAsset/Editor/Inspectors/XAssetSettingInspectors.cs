using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace XAsset.Editor
{
    [CustomEditor(typeof(XAssetSetting), true)]
    public class XAssetSettingInspectors : UnityEditor.Editor
    {
        private XAssetSetting owner { get { return (XAssetSetting)target; } }

        public override void OnInspectorGUI()
        {
            bool change = false;

            if (owner.Rule == null)
            {
                if (!string.IsNullOrEmpty(owner.RulePath))
                {
                    owner.Rule = AssetDatabase.LoadAssetAtPath<Object>(owner.RulePath);
                }
            }
            Object oldRule = owner.Rule;
            owner.Rule = EditorGUILayout.ObjectField("Rule", owner.Rule, typeof(Object), false);
            if (oldRule != owner.Rule && owner.Rule != null)
            {
                owner.RulePath = AssetDatabase.GetAssetPath(owner.Rule);
                change = true;
            }
            EditorGUILayout.LabelField("RulePath", owner.RulePath);

            if (owner.Manifest == null)
            {
                if (!string.IsNullOrEmpty(owner.ManifestPath))
                {
                    owner.Manifest = AssetDatabase.LoadAssetAtPath<Object>(owner.ManifestPath);
                }
            }
            Object oldManifest = owner.Manifest;
            owner.Manifest = EditorGUILayout.ObjectField("Manifest", owner.Manifest, typeof(Object), false);
            if (oldManifest != owner.Manifest && owner.Manifest != null)
            {
                owner.ManifestPath = AssetDatabase.GetAssetPath(owner.Manifest);
                change = true;
            }
            EditorGUILayout.LabelField("ManifestPath", owner.ManifestPath);

            //string ownerPath = AssetDatabase.GetAssetPath(owner);
            //string[] tmp = ownerPath.Split('/');
            //if (tmp.Length > 3)
            //{
            //    if (tmp[tmp.Length - 2] == "AssetSetting")
            //    {
            //        if (owner.RootPath != tmp[tmp.Length - 3])
            //        {
            //            owner.RootPath = tmp[tmp.Length - 3];
            //            change = true;
            //        }
            //        EditorGUILayout.LabelField("RootPath", owner.RootPath);
            //    }
            //    else
            //    {
            //        EditorGUILayout.LabelField("RootPath", "创建在AssetSetting下");
            //    }
            //}

            if (GUILayout.Button("Build Manifest"))
            {
                if (!string.IsNullOrEmpty(owner.RulePath) && !string.IsNullOrEmpty(owner.ManifestPath))
                {
                    AssetsMenuItem.BuildAssetManifest(owner.RulePath, owner.ManifestPath);
                }
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Build AssetBundles"))
            {
                if (!string.IsNullOrEmpty(owner.RulePath) && !string.IsNullOrEmpty(owner.ManifestPath))
                {
                    AssetsMenuItem.BuildAssetBundles(owner.RulePath, owner.ManifestPath, owner.RootPath);
                }
                GUIUtility.ExitGUI();
            }
        
            if (change) EditorUtility.SetDirty(owner);
        }
    }
}