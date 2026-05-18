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
        private readonly SelectArmyPowerView _selectArmyPowerView;
        private readonly PlayButtonPresenter _playButtonPresenter;
        private readonly SelectArmyPowerPresenter _selectArmyPowerPresenter;
        
        //Main menu logic layer
        private readonly MainMenuManager _mainMenuManager;

        public MainMenuBootstrap(
            ISceneTransitionManager sceneTransition,
            IEventBus eventBus,
            MainMenuManager mainMenuManager,
            PlayButtonView playButtonView,
            PlayButtonPresenter playButtonPresenter,
            SelectArmyPowerView selectArmyPowerView,
            SelectArmyPowerPresenter selectArmyPowerPresenter
            )
        {
            _sceneTransitionManager = sceneTransition;
            _eventBus = eventBus;
            _mainMenuManager = mainMenuManager;
            _playButtonView = playButtonView; 
            _playButtonPresenter = playButtonPresenter; 
            _selectArmyPowerView = selectArmyPowerView;
            _selectArmyPowerPresenter = selectArmyPowerPresenter;
        }

        public void Start()
        {
            _playButtonPresenter.BindView(_playButtonView);
            _selectArmyPowerPresenter.BindView(_selectArmyPowerView);
            _mainMenuManager.Initialize(_sceneTransitionManager, _eventBus);
            if(Application.isEditor)
            {
                Debug.Log("MainMenuBootstrap started -> all systems initialized.");
            }
        }
    }
}