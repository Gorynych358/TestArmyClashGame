using VContainer.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ACT.Runtime.Infrastructure.SceneManagement;
using ACT.Runtime.Infrastructure.Audio;

namespace ACT.Runtime.Infrastructure
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
                Debug.Log("Application initialized => Start background music => Load main menu scene!");
            
            ApplyGlobalSettings();
            
            _soundManager.PlayMusic(_audioLibrary.GetClip("BackgroundMusicLoop"));
            _sceneTransitionManager.LoadMainMenu().Forget();
        }

        private void ApplyGlobalSettings()
        {
            if (Application.isEditor)
                return;

            // --- Экран ---
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.fullScreen = true;

            // --- FPS / VSync ---
            QualitySettings.vSyncCount = 0;      // Unity-VSync OFF
            Application.targetFrameRate = 60;    // стабильный FPS

            // --- Сенсор ---
            Input.multiTouchEnabled = false;
            Input.simulateMouseWithTouches = false;

            // --- Ориентация ---
            Screen.orientation = ScreenOrientation.Portrait;

            // --- Физика не используется, отключаем ---
            Physics.simulationMode = SimulationMode.Script;
            Physics2D.simulationMode = SimulationMode2D.Script;

            // --- Тайминги ---
            Time.fixedDeltaTime = 1f;      // минимальная нагрузка
            Time.maximumDeltaTime = 0.5f;  // защита от фризов

            // --- Фоновые загрузки ---
            Application.backgroundLoadingPriority = ThreadPriority.High;
        }
    }
}
