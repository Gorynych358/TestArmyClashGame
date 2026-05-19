using ACT.Runtime.GameEvents.UIEvents;
using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Infrastructure.Audio;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.Infrastructure.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ACT.Runtime.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        private ISceneTransitionManager _sceneTransitionManager;
        private ISoundManager _soundManager;
        private IEventBus _eventBus;
        private GameplayStates _currentState = GameplayStates.Initialization;
        private BattleManager _battleManager;
        public void Initialize(BattleManager battleManager, IEventBus eventBus, ISoundManager soundManager, ISceneTransitionManager sceneManager)
        {
            _battleManager = battleManager;
            _eventBus = eventBus;
            _soundManager = soundManager;
            _sceneTransitionManager = sceneManager;
        }
        
        public void StartGameplay()
        {
            AddListeners();
            _battleManager.StartBattle();
            _currentState = GameplayStates.Battle;
        }

        
        private void Update()
        {
            if(_currentState == GameplayStates.Battle)
                _battleManager.UpdateGameplay(Time.deltaTime);
        }

        private void AddListeners()
        {
            _eventBus.Subscribe<BackButtonPressedEvent>(OnBackButtonPressed);
            _eventBus.Subscribe<PauseButtonPressedEvent>(OnPausePressed);
            _eventBus.Subscribe<ResumeButtonPressedEvent>(OnResumePressed);
        }

        private void RemoveListeners()
        {
            _eventBus.Unsubscribe<BackButtonPressedEvent>(OnBackButtonPressed);
            _eventBus.Unsubscribe<PauseButtonPressedEvent>(OnPausePressed);
            _eventBus.Unsubscribe<ResumeButtonPressedEvent>(OnResumePressed);
        }

        private void OnPausePressed(PauseButtonPressedEvent _)
        {
            Time.timeScale = 0;
            _soundManager.PauseMusic();
            _currentState = GameplayStates.Pause;
        }

        private void OnResumePressed(ResumeButtonPressedEvent _)
        {
            Time.timeScale = 1;
            _soundManager.ResumeMusic();
            _currentState = GameplayStates.Battle;
        }
        private void OnBackButtonPressed(BackButtonPressedEvent _)
        {
            _currentState = GameplayStates.StopGame;
            MoveToMainMenuScene(); 
        }

        // --------------------------------------------------------------------
        // LOAD GAMEPLAY SCENE
        // --------------------------------------------------------------------
        private void MoveToMainMenuScene()
        {
            Time.timeScale = 1;
            _soundManager.ResumeMusic();
            RemoveListeners();
            _battleManager.DisposeScene();
            _sceneTransitionManager.LoadMainMenu().Forget();
        }


        private void OnDestroy() 
        {
            RemoveListeners();
            _battleManager.DisposeScene();
        }
    }
}
