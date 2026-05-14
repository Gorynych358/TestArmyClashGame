using UnityEngine;
using VContainer.Unity;
using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Gameplay.UI.Presenters;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Gameplay.Units;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.Gameplay
{
    public sealed class GameplayBootstrap : IStartable
    {
        //Services:
        private readonly IEventBus _eventBus;

        //Gameplay:
        private readonly BattleManager _battleManager;
        private readonly SpatialGrid _spatialGrid;
        private readonly RandomArmyCalculator _randomArmyCalculator;
        private readonly FormationBuilder _formationBuilder;
        private readonly ArmySpawner _armySpawner;
        private readonly UnitObjectPool _unitPool;
        //UI:
        private readonly BattleProgressView _battleProgressView;
        private readonly CoinsView _coinsView;
        private readonly FightButtonView _fightButtonView;
        private readonly ChangeFormationView _changeFormationView;
        private readonly BattleCompleteView _battleCompleteView;
        private readonly BattleProgressPresenter _battleProgressPresenter;
        private readonly CoinsPresenter _coinsPresenter;
        private readonly FightButtonPresenter _fightButtonPresenter;
        private readonly ChangeFormationPresenter _changeFormationPresenter;
        private readonly BattleCompletePresenter _battleCompletePresenter;

        public GameplayBootstrap(
            IEventBus eventBus,
            BattleManager battleManager,
            RandomArmyCalculator randomArmyCalculator, 
            FormationBuilder formationBuilder, 
            ArmySpawner armySpawner,
            UnitObjectPool unitPool,
            SpatialGrid spatialGrid,
            BattleProgressView battleProgressView,
            CoinsView coinsView,
            FightButtonView fightButtonView,
            ChangeFormationView changeFormationView,
            BattleCompleteView battleCompleteView,
            BattleProgressPresenter battleProgressPresenter,
            CoinsPresenter coinsPresenter,
            FightButtonPresenter fightButtonPresenter,
            ChangeFormationPresenter changeFormationPresenter,
            BattleCompletePresenter battleCompletePresenter
            )
        {
            _eventBus = eventBus;
            _battleManager = battleManager;
            _randomArmyCalculator = randomArmyCalculator;
            _formationBuilder = formationBuilder;
            _armySpawner = armySpawner;
            _unitPool = unitPool;
            _spatialGrid = spatialGrid;
            _battleProgressView = battleProgressView; 
            _coinsView = coinsView; 
            _fightButtonView = fightButtonView; 
            _changeFormationView = changeFormationView;
            _battleCompleteView = battleCompleteView;
            _battleProgressPresenter = battleProgressPresenter; 
            _coinsPresenter = coinsPresenter; 
            _fightButtonPresenter = fightButtonPresenter; 
            _changeFormationPresenter = changeFormationPresenter;
            _battleCompletePresenter = battleCompletePresenter;
        }

        public void Start()
        {
            _battleProgressPresenter.BindView(_battleProgressView);
            _coinsPresenter.BindView(_coinsView);
            _fightButtonPresenter.BindView(_fightButtonView);
            _changeFormationPresenter.BindView(_changeFormationView);
            _battleCompletePresenter.BindView(_battleCompleteView);
            Debug.Log("GameplayBootstrap initialized." +
            " Event bus = " + (_eventBus != null ? "injected" : "null") + 
            ", BattleManager = " + (_battleManager != null ? "injected" : "null") + 
            ", RandomArmyCalculator = " + (_randomArmyCalculator != null ? "injected" : "null") + 
            ", FormationBuilder = " + (_formationBuilder != null ? "injected" : "null") + 
            ", ArmySpawner = " + (_armySpawner != null ? "injected" : "null") + 
            ", UnitPool = " + (_unitPool != null ? "injected" : "null") + 
            ", SpatialGrid = " + (_spatialGrid != null ? "injected" : "null"));
            _battleManager.Initialize(_unitPool, _randomArmyCalculator,
            _formationBuilder, _armySpawner, _eventBus, _spatialGrid);
            if(Application.isEditor)
            {
                Debug.Log("GameBootstrap started and initialized all systems.");
                Debug.Log("BattleManager initialized with all dependencies.");
            }
        }
    }
}