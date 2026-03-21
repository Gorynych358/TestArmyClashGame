using System.Collections.Generic;
using UnityEngine;

namespace ACT.Scripts
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Configs/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        [System.Serializable]
        public struct AudioClipData
        {
            public string name;
            public AudioClip clip;
        }

        public AudioClipData[] clips;

        private Dictionary<string, AudioClip> _clipDict;

        private void OnEnable()
        {
            // Явная проверка и инициализация
            if (clips == null)
            {
                clips = new AudioClipData[0]; // Пустой массив, чтобы избежать NRE
            }
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            _clipDict = new Dictionary<string, AudioClip>();
            foreach (var data in clips)
            {
                if (!string.IsNullOrEmpty(data.name) && data.clip != null)
                {
                    _clipDict[data.name] = data.clip;
                }
            }
        }

        public AudioClip GetClip(string name)
        {
            return _clipDict.TryGetValue(name, out var clip) ? clip : null;
        }

        public bool HasClip(string name)
        {
            return _clipDict.ContainsKey(name);
        }
    }
}
