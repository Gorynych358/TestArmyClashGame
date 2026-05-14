using UnityEngine;
using VContainer.Unity;
using ACT.Runtime.Infrastructure.SceneManagement;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.MainMenu.Views;
using ACT.Runtime.MainMenu.Presenters;

namespace ACT.Runtime.MainMenu
{
    public sealed class MainMenuBootstrap : IStartable
    {
        //Services:
        private readonly ISceneTransitionManager _sceneTransitionManager;
        private readonly IEventBus _eventBus;
        
        //UI:
        private readonly PlayButtonView _playButtonView;
        private readonly PlayButtonPresenter _playButtonPresenter;
        
        //Main menu logic layer
        private readonly MainMenuManager _mainMenuManager;

        public MainMenuBootstrap(
            ISceneTransitionManager sceneTransition,
            IEventBus eventBus,
            MainMenuManager mainMenuManager,
            PlayButtonView playButtonView,
            PlayButtonPresenter playButtonPresenter
            )
        {
            _sceneTransitionManager = sceneTransition;
            _eventBus = eventBus;
            _mainMenuManager = mainMenuManager;
            _playButtonView = playButtonView; 
            _playButtonPresenter = playButtonPresenter; 
        }

        public void Start()
        {
            _playButtonPresenter.BindView(_playButtonView);
            _mainMenuManager.Initialize(_sceneTransitionManager, _eventBus);
            Debug.Log("MainMenuBootstrap started and initialized all systems.");
        }
    }
}