using UnityEngine;
using VContainer.Unity;

namespace ACT.Scripts
{
    public sealed class GameBootstrap : IStartable
    {
        //Services:
        private readonly IEventBus _eventBus;

        //Gameplay:
        private readonly BattleManager _battleManager;
        private readonly SpatialGrid _spatialGrid;
        private readonly FormationGenerator _formationGenerator;
        private readonly UnitObjectPool _unitPool;
        //UI:
        private readonly BattleProgressView _battleProgressView;
        private readonly CoinsView _coinsView;
        private readonly FightButtonView _fightButtonView;
        private readonly BattleProgressPresenter _battleProgressPresenter;
        private readonly CoinsPresenter _coinsPresenter;
        private readonly FightButtonPresenter _fightButtonPresenter;

        public GameBootstrap(
            IEventBus eventBus,
            BattleManager battleManager,
            FormationGenerator formationGenerator,
            UnitObjectPool unitPool,
            SpatialGrid spatialGrid,
            BattleProgressView battleProgressView,
            CoinsView coinsView,
            FightButtonView fightButtonView,
            BattleProgressPresenter battleProgressPresenter,
            CoinsPresenter coinsPresenter,
            FightButtonPresenter fightButtonPresenter
            )
        {
            _eventBus = eventBus;
            _battleManager = battleManager;
            _formationGenerator = formationGenerator;
            _unitPool = unitPool;
            _spatialGrid = spatialGrid;
            _battleProgressView = battleProgressView; 
            _coinsView = coinsView; 
            _fightButtonView = fightButtonView; 
            _battleProgressPresenter = battleProgressPresenter; 
            _coinsPresenter = coinsPresenter; 
            _fightButtonPresenter = fightButtonPresenter; 
        }

        public void Start()
        {
            _battleProgressPresenter.BindView(_battleProgressView);
            _coinsPresenter.BindView(_coinsView);
            _fightButtonPresenter.BindView(_fightButtonView);
            _battleManager.Initialize(_unitPool, _formationGenerator, _eventBus, _spatialGrid);
            Debug.Log("GameBootstrap started and initialized all systems.");
        }
    }
}