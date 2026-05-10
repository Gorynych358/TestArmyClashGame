using VContainer.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace ACT.Scripts
{
    public sealed class AppEntryPoint : IStartable
    {
        private readonly ISceneTransitionManager _sceneTransitionManager;
        private readonly AudioLibrary _audioLibrary;
        private readonly ISoundManager _soundManager;

        public AppEntryPoint(
            ISceneTransitionManager sceneTransition, 
            AudioLibrary audioLibrary,
            ISoundManager soundManager)
        {
            _sceneTransitionManager = sceneTransition;
            _audioLibrary = audioLibrary;
            _soundManager = soundManager;
        }

        public void Start()
        {
            if(Application.isEditor)
                Debug.Log("Game initialized!");
            else
                Application.targetFrameRate = 60;
            
            _soundManager.PlayMusic(_audioLibrary.GetClip("BackgroundMusicLoop"));
            _sceneTransitionManager.LoadMainMenu().Forget();
        }
    }
}
