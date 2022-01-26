using System;
using System.Collections;
using UnityEngine;

namespace XAsset
{
    public class Asset : Logger, IEnumerator
    {
        #region IEnumerator implementation

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        #endregion

        public int references { get; private set; }

        public string assetPath { get; protected set; }

        public System.Type assetType { get; protected set; }

        public virtual bool isDone { get { return true; } }

        private System.Action<Asset> completed;

        public void AddCompletedLisenter(System.Action<Asset> lisenter)
        {
            completed += lisenter;
        }

        public void RemoveCompletedLisenter(System.Action<Asset> lisenter)
        {
            completed -= lisenter;
        }

        public UnityEngine.Object asset { get; protected set; }

        protected Assets assets;

        internal Asset(string path, System.Type type,Assets assets)
        {
            assetPath = path;
            assetType = type;
            this.assets = assets;
        }

        internal void Load()
        {
            I("Load " + assetPath);
            OnLoad();
        }

        internal void Unload(bool immediate = false)
        {
            I("Unload " + assetPath);
            if (asset != null)
            {
                if (asset.GetType() != typeof(GameObject))
                {
                    Resources.UnloadAsset(asset);
                }
                asset = null;
            }
            OnUnload(immediate);
            assetPath = null;
        }

        protected virtual void OnLoad()
        {
#if UNITY_EDITOR
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#endif
        }

        internal bool Update()
        {
            if (isDone)
            {
                if (completed != null)
                {
                    completed.Invoke(this);
                    completed = null;
                }
                return false;
            }
            return true;
        }

        protected virtual void OnUnload(bool immediate = false)
        {

        }

        public void Retain()
        {
            Update();
            references++;
        }
        public void Release()
        {
            if (--references < 0)
            {
                XDebug.LogError("refCount < 0:"+ assetPath);
            }
        }
        public void Clear()
        {
            references = 0;
        }
    }

    public class BundleAsset : Asset
    {
        protected Bundle request;
        protected string bundlepath = null;
        protected bool haveSetBundlePath = false;

        internal BundleAsset(string path, System.Type type, Assets assets) : base(path, type, assets)
        {
            haveSetBundlePath = bundlepath != null;
           this.bundlepath = assets.GetBundleName(assetPath);
        }

        protected override void OnLoad()
        {
            request = assets.Bundles.Load(bundlepath);
            asset = request.LoadAsset(assets.GetAssetName(assetPath), assetType);
        }

        protected override void OnUnload(bool immediate = false)
        {
            if (immediate)
            {
                assets.Bundles.Clear(request);
                request = null;
                return;
            }
            if (request != null)
            {
                request.Release();
            }
            request = null;
        }
    }
    public class BundleAssetAsync : BundleAsset
    {
        AssetBundleRequest assetBundleRequest;

        internal BundleAssetAsync(string path, System.Type type, Assets assets) : base(path, type, assets)
        {

        }

        protected override void OnLoad()
        {
            request = assets.Bundles.LoadAsync(bundlepath);
        }

        protected override void OnUnload(bool immediate = false)
        {
            base.OnUnload();
            assetBundleRequest = null;
            loadState = 0;
        }

        int loadState;

        public override bool isDone
        {
            get
            {
                if (loadState == 2)
                {
                    return true;
                }

                if (request.error != null)
                {
                    return true;
                }

                for (int i = 0; i < request.dependencies.Count; i++) // ÒÀÀµÃ»ÓÐ´íÎó
                {
                    var dep = request.dependencies[i];
                    if (dep.error != null)
                    {
                        return true;
                    }
                }

                if (loadState == 1)
                {
                    if (assetBundleRequest.isDone)
                    {
                        asset = assetBundleRequest.asset;
                        loadState = 2;
                        return true;
                    }
                }
                else
                {
                    bool allReady = true;
                    if (!request.isDone)
                    {
                        allReady = false;
                    }

                    if (request.dependencies.Count > 0)
                    {
                        if (!request.dependencies.TrueForAll(bundle => bundle.isDone))
                        {
                            allReady = false;
                        }
                    }

                    if (allReady)
                    {                        
                        assetBundleRequest = request.LoadAssetAsync(System.IO.Path.GetFileName(assetPath), assetType);
                        if (assetBundleRequest == null)
                        {
                            loadState = 2;
                            return true;
                        }
                        loadState = 1;
                    }
                }
                return false;
            }
        }
    }
    public class ResourceAsset : Asset
    {
        public ResourceAsset(string path, System.Type type, Assets assets) : base(path, type, assets)
        {

        }
        protected override void OnLoad()
        {
            asset = Resources.Load(assetPath, assetType);
        }
    }
    public class ResourceAssetAsync : Asset
    {
        ResourceRequest resourceRequest;
        public ResourceAssetAsync(string path, System.Type type, Assets assets) : base(path, type, assets)
        {

        }
        protected override void OnLoad()
        {
            resourceRequest = Resources.LoadAsync(assetPath);
        }

        protected override void OnUnload(bool immediate = false)
        {
            base.OnUnload();
            resourceRequest = null;
            loadState = 0;
        }
        int loadState;
        public override bool isDone
        {
            get
            {
                if (loadState == 1)
                {
                    return true;
                }
                if (loadState == 0)
                {
                    if (resourceRequest.isDone)
                    {
                        asset = resourceRequest.asset;
                        loadState = 1;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
