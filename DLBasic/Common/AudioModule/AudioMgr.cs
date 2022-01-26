using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XAsset;
using DG.Tweening;
namespace DLBasic.Common
{
    public class AudioMgr : MonoBehaviour
    {
        public enum PlayType
        {
            OnlyOne,
            NoLimit,
            OnShot
        }
        public enum AudioType
        {
            back,
            effect,
            guide
        }
        /// <summary>
        /// 当前背景音乐
        /// </summary>
        public string CurrBackMusic { get; set; }

        Dictionary<AudioType, float> voiceDic = new Dictionary<AudioType, float>();
        Dictionary<string, List<MAudioSource>> asDic = new Dictionary<string, List<MAudioSource>>();
        Dictionary<string, AudioType> name2Type = new Dictionary<string, AudioType>();
        Dictionary<AudioType, List<MAudioSource>> audioTypeDic = new Dictionary<AudioType, List<MAudioSource>>();
        List<MAudioSource> asList = new List<MAudioSource>();

        Dictionary<string, AudioClip> audioDic = new Dictionary<string, AudioClip>();
        Dictionary<string, Asset> AssetDic = new Dictionary<string, Asset>();
        Assets assets;
        public static AudioMgr Create(Assets assets)
        {
            GameObject go = new GameObject("AudioMgr");
            AudioMgr am = go.AddComponent<AudioMgr>();
            am.Init(assets);
            return am;
        }
        void Init(Assets assets)
        {
            this.assets = assets;
        }
        AudioClip GetAC(string name)
        {
            if (audioDic.ContainsKey(name))
            {
                return audioDic[name];
            }
            Asset at = assets.Load<AudioClip>(name);

            if (at == null || at.asset == null) return null;
            AssetDic[name] = at;
            AudioClip ac = at.asset as AudioClip;
            audioDic[name] = ac;
            return ac;
        }
        MAudioSource GetAS(string name, PlayType type, AudioType aType)
        {
            MAudioSource mas = null;

            if (type == PlayType.OnlyOne)
            {
                if (asDic.ContainsKey(name))
                {
                    if (asDic[name].Count > 0)
                    {
                        mas = asDic[name][0];
                        mas.mAS.volume = GetVoice(mas.aType);
                        return mas;
                    }
                }
            }

            for (int i = 0; i < asList.Count; i++)
            {
                if (!asList[i].IsPlay)
                {
                    mas = asList[i];
                    break;
                }
            }

            if (mas == null)
            {
                mas = new MAudioSource();
                mas.mAS = gameObject.AddComponent<AudioSource>();
                mas.mAS.playOnAwake = false;
                asList.Add(mas);
            }
            else
            {
                var ass = asDic[mas.clipName];
                ass.Remove(mas);
                audioTypeDic[mas.aType].Remove(mas);
                if (ass.Count == 0 && mas.clipName != name)
                {
                    audioDic.Remove(mas.clipName);
                    AssetDic[mas.clipName].Release();
                    AssetDic.Remove(mas.clipName);
                }
            }
            mas.clipName = name;
            mas.aType = aType;

            if (!asDic.ContainsKey(name))
            {
                asDic[name] = new List<MAudioSource>();
            }
            asDic[name].Add(mas);

            if (!audioTypeDic.ContainsKey(aType))
            {
                audioTypeDic[aType] = new List<MAudioSource>();
            }
            audioTypeDic[aType].Add(mas);
            mas.mAS.volume = GetVoice(mas.aType);

            return mas;
        }
        List<MAudioSource> GetAllAS(string name)
        {
            if (!asDic.ContainsKey(name)) return null;
            return asDic[name];
        }
        public float GetVoice(AudioType type)
        {
            if (!voiceDic.ContainsKey(type))
            {
                voiceDic[type] = 1;
            }
            return voiceDic[type];
        }
        public void SetVoice(AudioType type, float v)
        {
            voiceDic[type] = v;
            if (audioTypeDic.ContainsKey(type))
            {
                List<MAudioSource> list = audioTypeDic[type];

                for (int i = 0; i < list.Count; i++)
                {
                    MAudioSource mas = list[i];
                    if (mas.IsPlay)
                    {
                        mas.mAS.volume = v;
                    }
                }
            }
        }
        public void Play(string name, bool loop, PlayType type, AudioType aType)
        {
            name2Type[name] = aType;
            AudioClip ac = GetAC(name);
            if (ac == null)
            {
                XDebug.LogError("no audio:" + name);
                return;
            }
            MAudioSource _as = GetAS(name, type, aType);
            _as.mAS.clip = ac;
            _as.mAS.loop = loop;
            if (aType == AudioType.back)
            {
                CurrBackMusic = name;

            }
            if (type == PlayType.OnShot)
            {
                _as.mAS.PlayOneShot(_as.mAS.clip);
            }
            else
            {
                _as.mAS.Play();
            }
        }
        public void Stop(string name, float Fadeout = 0.0f)
        {
            List<MAudioSource> list = GetAllAS(name);
            if (list == null)
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                MAudioSource ma = list[i];
                if (ma.mAS.isPlaying)
                {
                    ma.mAS.DOFade(0, Fadeout).OnComplete(() =>
                    {
                        ma.mAS.Stop();
                    });
                }
            }
        }

        public void StopByType(AudioType audioType)
        {
            if (audioTypeDic.ContainsKey(audioType))
            {
                List<MAudioSource> list = audioTypeDic[audioType];

                for (int i = 0; i < list.Count; i++)
                {
                    MAudioSource mas = list[i];
                    if (mas.IsPlay)
                    {
                        mas.mAS.DOFade(0, 0.1f);
                    }
                }
            }
        }

        public void ClearNoUse()
        {
            for (int i = 0; i < asList.Count; i++)
            {
                MAudioSource mas = asList[i];
                if (!mas.IsPlay)
                {
                    asList.RemoveAt(i);
                    i--;
                    var ass = asDic[mas.clipName];
                    ass.Remove(mas);
                    audioTypeDic[mas.aType].Remove(mas);
                    if (ass.Count == 0)
                    {
                        audioDic.Remove(mas.clipName);
                        AssetDic[mas.clipName].Release();
                        AssetDic.Remove(mas.clipName);
                    }
                }
            }
        }
        public void DestroyAll()
        {
            foreach (var item in AssetDic)
            {
                item.Value.Release();
            }
            AssetDic.Clear();
            voiceDic.Clear();
            asDic.Clear();
            name2Type.Clear();
            audioTypeDic.Clear();
            asList.Clear();
            audioDic.Clear();
            GameObject.Destroy(this.gameObject);
        }
        public class MAudioSource
        {
            public string clipName;
            public AudioType aType;
            public AudioSource mAS;

            public bool IsPlay
            {
                get
                {
                    return mAS.isPlaying;
                }
            }
        }
    }
}