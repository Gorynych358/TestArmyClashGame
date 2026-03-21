using UnityEngine;

namespace ACT.Scripts
{
    public sealed class AudioRoot : MonoBehaviour
    {
        [SerializeField] private AudioSettings audioSettings;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        public AudioSettings AudioSettings => audioSettings;
        public AudioSource Music => musicSource;
        public AudioSource Sfx => sfxSource;
    }
}
