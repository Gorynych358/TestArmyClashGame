using UnityEngine;

namespace ACT.Scripts
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "Configs/Audio/Settings")]
    
    public sealed class AudioSettings : ScriptableObject
    {
        [Range(0, 1)] public float MusicVolume = 1f;
        [Range(0, 1)] public float SfxVolume = 1f;
        public bool SoundEnabled = true;
    }
}