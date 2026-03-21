using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ACT.Scripts
{
    public sealed class SoundManager : ISoundManager
    {
        private readonly AudioSource _sfx;
        private readonly AudioSource _music;
        private readonly AudioSettings _settings;

        private const float FadeTime = 0.5f;

        public SoundManager(AudioRoot audioRoot)
        {
            _sfx = audioRoot.Sfx;
            _music = audioRoot.Music;
            _settings = audioRoot.AudioSettings;
            ApplySettings();
        }

        public void PlaySound(AudioClip clip)
        {
            if (!_settings.SoundEnabled || clip == null) return;
            _sfx.volume = _settings.SfxVolume;
            _sfx.clip = clip;
            _sfx.Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (!_settings.SoundEnabled || clip == null) return;
            _music.volume = _settings.MusicVolume;
            _music.loop = true;
            _music.clip = clip;
            _music.Play();
        }

        public void StopSound(bool withFade) =>
            Stop(_sfx, withFade);

        public void StopMusic(bool withFade) =>
            Stop(_music, withFade);

        public void ApplySettings()
        {
            _sfx.mute = !_settings.SoundEnabled;
            _music.mute = !_settings.SoundEnabled;
        }

        private void Stop(AudioSource source, bool fade)
        {
            if (!fade) { source.Stop(); return; }
            FadeOut(source).Forget();
        }

        private async UniTaskVoid FadeOut(AudioSource source)
        {
            float start = source.volume;
            float t = 0f;

            while (t < FadeTime)
            {
                t += Time.deltaTime;
                source.volume = Mathf.Lerp(start, 0, t / FadeTime);
                await UniTask.Yield();
            }

            source.Stop();
            source.volume = start;
        }
    }
}
