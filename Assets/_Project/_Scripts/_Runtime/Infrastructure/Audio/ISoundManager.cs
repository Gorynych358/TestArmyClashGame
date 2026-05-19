using UnityEngine;

namespace ACT.Runtime.Infrastructure.Audio
{
    public interface ISoundManager
    {
        void PlaySound(AudioClip clip);
        void PlayMusic(AudioClip clip);
        void PauseMusic();
        void ResumeMusic();
        void StopSound(bool withFade);
        void StopMusic(bool withFade);
        void ApplySettings();
    }
}
