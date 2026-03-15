using VContainer.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ACT.Scripts
{
    public sealed class EntryPoint : IStartable
    {
        private readonly ISceneTransitionManager _sceneTransitionManager;
        private readonly AudioLibrary _audioLibrary;
        private readonly ISoundManager _soundManager;

        public EntryPoint(
            ISceneTransitionManager scenes, 
            AudioLibrary audioLibrary,
            ISoundManager soundManager)
        {
            _sceneTransitionManager = scenes;
            _audioLibrary = audioLibrary;
            _soundManager = soundManager;
        }

        public void Start()
        {
            //The entry point of the application. 
            //Global services initializtion, 
            //remote data loading, authentication etc.
            Application.targetFrameRate = 60;
            if(Application.isEditor)
                Debug.Log("Game initialized!");
            _soundManager.PlayMusic(_audioLibrary.GetClip("BackgroundMusicLoop"));
            _sceneTransitionManager.LoadMainMenu().Forget();
        }
    }
}
