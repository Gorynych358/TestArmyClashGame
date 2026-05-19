using UnityEngine;
using VContainer.Unity;
using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Gameplay.UI.Presenters;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Gameplay.Units;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.Infrastructure;
using ACT.Runtime.Infrastructure.SceneManagement;
using ACT.Runtime.Infrastructure.Audio;

namespace ACT.Runtime.Gameplay
{
    public sealed class GameplayBootstrap : IStartable
    {
        //Services:
        private readonly ISceneTransitionManager _sceneTransitionManager;
        private readonly IEventBus _eventBus;
        private readonly ISoundManager _soundManager;
        //Gameplay scene manager:
        private readonly GameplayManager _gameplayManager;
        //Battle manager:
        private readonly BattleManager _battleManager;
        private readonly SpatialGrid _spatialGrid;
        private readonly RandomArmyCalculator _randomArmyCalculator;
        private readonly FormationBuilder _formationBuilder;
        private readonly ArmySpawner _armySpawner;
        private readonly UnitObjectPool _unitPool;
        private readonly ArmyPowerSettingsSO _armyPowerSettings;
        //UI Views:
        private readonly GameControlView _gameControlView;
        private readonly GamePauseView _gamePauseView;
        private readonly BattleProgressView _battleProgressView;
        private readonly CoinsView _coinsView;
        private readonly FightButtonView _fightButtonView;
        private readonly ChangeFormationView _changeFormationView;
        private readonly BattleCompleteView _battleCompleteView;
        //UI Presenters:
        private readonly GameControlPresenter _gameControlPresenter;
        private readonly GamePausePresenter _gamePausePresenter;
        private readonly BattleProgressPresenter _battleProgressPresenter;
        private readonly CoinsPresenter _coinsPresenter;
        private readonly FightButtonPresenter _fightButtonPresenter;
        private readonly ChangeFormationPresenter _changeFormationPresenter;
        private readonly BattleCompletePresenter _battleCompletePresenter;

        public GameplayBootstrap(
            ISceneTransitionManager sceneTransition,
            ArmyPowerSettingsSO armyPowerSettingsSO,
            IEventBus eventBus,
            ISoundManager soundManager,
            GameplayManager gameplayManager,
            BattleManager battleManager,
            RandomArmyCalculator randomArmyCalculator, 
            FormationBuilder formationBuilder, 
            ArmySpawner armySpawner,
            UnitObjectPool unitPool,
            SpatialGrid spatialGrid,
            //Views/Presenters
            GameControlView gameControlView,
            GamePauseView gamePauseView,
            BattleProgressView battleProgressView,
            CoinsView coinsView,
            FightButtonView fightButtonView,
            ChangeFormationView changeFormationView,
            BattleCompleteView battleCompleteView,
            GameControlPresenter gameControlPresenter,
            GamePausePresenter gamePausePresenter,
            BattleProgressPresenter battleProgressPresenter,
            CoinsPresenter coinsPresenter,
            FightButtonPresenter fightButtonPresenter,
            ChangeFormationPresenter changeFormationPresenter,
            BattleCompletePresenter battleCompletePresenter
            )
        {
            _sceneTransitionManager = sceneTransition;
            _soundManager = soundManager;
            _armyPowerSettings = armyPowerSettingsSO;
            _eventBus = eventBus;
            _gameplayManager = gameplayManager;
            _battleManager = battleManager;
            _randomArmyCalculator = randomArmyCalculator;
            _formationBuilder = formationBuilder;
            _armySpawner = armySpawner;
            _unitPool = unitPool;
            _spatialGrid = spatialGrid;
            _gameControlView = gameControlView;
            _gamePauseView = gamePauseView;
            _battleProgressView = battleProgressView; 
            _coinsView = coinsView; 
            _fightButtonView = fightButtonView; 
            _changeFormationView = changeFormationView;
            _battleCompleteView = battleCompleteView;
            _gameControlPresenter = gameControlPresenter;
            _gamePausePresenter = gamePausePresenter;
            _battleProgressPresenter = battleProgressPresenter; 
            _coinsPresenter = coinsPresenter; 
            _fightButtonPresenter = fightButtonPresenter; 
            _changeFormationPresenter = changeFormationPresenter;
            _battleCompletePresenter = battleCompletePresenter;
        }

        //Ручная инициализация всех зависимостей геймплейной сцены:
        public void Start()
        {
            _gameControlPresenter.BindView(_gameControlView);
            _gamePausePresenter.BindView(_gamePauseView);
            _battleProgressPresenter.BindView(_battleProgressView);
            _coinsPresenter.BindView(_coinsView);
            _fightButtonPresenter.BindView(_fightButtonView);
            _changeFormationPresenter.BindView(_changeFormationView);
            _battleCompletePresenter.BindView(_battleCompleteView);
            _battleManager.Initialize(_armyPowerSettings, _unitPool, _randomArmyCalculator,
            _formationBuilder, _armySpawner, _eventBus, _spatialGrid);
            _gameplayManager.Initialize(_battleManager, _eventBus, _soundManager, _sceneTransitionManager);
            if(Application.isEditor)
            {
                Debug.Log("GameBootstrap started -> all systems initialized. -> " + 
                            "BattleManager initialized with all dependencies. -> " +
                            "GameplayManager initialized and ready to start battle.");
            }
            _gameplayManager.StartGameplay();
        }
    }
}