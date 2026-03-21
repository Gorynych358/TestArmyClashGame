using UnityEngine;

namespace ACT.Scripts
{
    public interface ISoundManager
    {
        void PlaySound(AudioClip clip);
        void PlayMusic(AudioClip clip);
        void StopSound(bool withFade);
        void StopMusic(bool withFade);
        void ApplySettings();
    }
}
