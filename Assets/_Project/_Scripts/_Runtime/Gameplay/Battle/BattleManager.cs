using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using ACT.Runtime.GameEvents;
using ACT.Runtime.Gameplay.Units;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.Gameplay.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform _battleField;
        [SerializeField] private Transform _poolStorage;

        [Header("Formations")]
        [SerializeField] private FormationDataSO _defendersFormation;
        [SerializeField] private FormationDataSO _invadersFormation;

        [Header("Spawn Settings")]
        [SerializeField] private float _armyPower = 3000f;
        [SerializeField] private Vector3 _defendersSpawnPoint = new(-5, 0, 0);
        [SerializeField] private Vector3 _invadersSpawnPoint = new(5, 0, 0);

        private readonly List<Unit> _defenders = new();
        private readonly List<Unit> _invaders = new();

        private UnitObjectPool _pool;
        private RandomArmyCalculator _randomArmyCalculator;
        private FormationBuilder _formationBuilder;
        private ArmySpawner _armySpawner;
        private IEventBus _eventBus;
        private SpatialGrid _spatialGrid;

        private BattleSessionData _sessionData;
        private CancellationTokenSource _cts;

        private bool _battleActive;

    #region INITIALIZATION

        public void Initialize(
            UnitObjectPool pool,
            RandomArmyCalculator randomArmyCalculator,
            FormationBuilder formationBuilder,
            ArmySpawner armySpawner,
            IEventBus eventBus,
            SpatialGrid spatialGrid)
        {
            _cts = new CancellationTokenSource();

            _pool = pool;
            _pool.InitializePool(10, _poolStorage);
            _randomArmyCalculator = randomArmyCalculator;
            _formationBuilder = formationBuilder;
            _armySpawner = armySpawner;
            _spatialGrid = spatialGrid;
            _eventBus = eventBus;
            print($"BattleManager: EventBus is null == {_eventBus == null}");
            AddListeners();
            if(Application.isEditor)
                print("BattleManager initialized!");
        }

    #endregion

    #region GAME LOOP

        private void StartGameplay()
        {
            InitNewBattleSessionAsync().Forget();
        }

        private void AddListeners()
        {
            if (_eventBus == null)
            {
                Debug.LogWarning("EventBus is not assigned in BattleManager! Check if the game starts from the correct scene.");
                return;
            }
            _eventBus.Subscribe<ChangeDefendersFormationEvent>(OnUpdateDefendersFormation);
            _eventBus.Subscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Subscribe<BattleCompleteNextButtonClickEvent>(OnBattleCompleteNextButtonClicked);
            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
        }

        private void UpdateGameplay()
        {
            if (!_battleActive)
                return;

            _spatialGrid.Clear();
            _spatialGrid.Build(_defenders, _invaders);

            for (int i = _defenders.Count - 1; i >= 0; i--)
                _defenders[i].Tick(Time.deltaTime);

            for (int i = _invaders.Count - 1; i >= 0; i--)
                _invaders[i].Tick(Time.deltaTime);
        }

    #endregion

    #region BATTLE SESSION

        private async UniTask InitNewBattleSessionAsync()
        {
            _battleActive = false;

            _sessionData = new BattleSessionData(_armyPower);

            Color defendersColor = RandomPastelColorGenerator.GeneratePastelColor();
            Color invadersColor = RandomPastelColorGenerator.GenerateContrastColor(defendersColor);
            _sessionData.SetColors(defendersColor, invadersColor);

            BuildArmies();

            await UniTask.Delay(500, cancellationToken: _cts.Token);

            _eventBus.Publish(new BattleReadyEvent());
        }

    #endregion

    #region FORMATION & ARMY BUILDING

        private void OnUpdateDefendersFormation(ChangeDefendersFormationEvent _)
        {
            RebuildDefendersArmy();

            DOVirtual.DelayedCall(1.0f, () =>
                _eventBus.Publish(new BattleReadyEvent())
            );
        }

        private void BuildArmies()
        {
            float invadersPower;

            List<UnitTypes> rndUnitTypes = _randomArmyCalculator.GenerateArmy(_sessionData.ArmyPower);
            FormationRuntimeData formation = _formationBuilder.BuildRandom(rndUnitTypes);

            var invaders = CreateArmy(ArmyTypes.Invaders, formation, out invadersPower);
            _invaders.AddRange(invaders);

            _sessionData.SetInvadersArmyStats(_invaders.Count, invadersPower);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData));

            RebuildDefendersArmy();

            print($"Defenders: {_sessionData.DefendersPower}|{_sessionData.DefendersCount}, " +
                  $"Invaders: {_sessionData.InvadersPower}|{_sessionData.InvadersCount}");
        }

        private void RebuildDefendersArmy()
        {
            ClearArmy(ArmyTypes.Defenders);

            float basePower = _sessionData.ArmyPower;
            float range = basePower * 0.2f;
            float randomizedPower = basePower + Random.Range(-range, range);

            List<UnitTypes> rndUnitTypes = _randomArmyCalculator.GenerateArmy(randomizedPower);
            FormationRuntimeData formation = _formationBuilder.BuildRandom(rndUnitTypes);

            var defenders = CreateArmy(ArmyTypes.Defenders, formation, out float defendersPower);
            _defenders.AddRange(defenders);

            _sessionData.SetDefendersArmyStats(_defenders.Count, defendersPower);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData));
        }

        private List<Unit> CreateArmy(ArmyTypes armyType, IFormationData formation, out float resultPower)
        {
            Color color = armyType == ArmyTypes.Defenders
                ? _sessionData.DefendersColor
                : _sessionData.InvadersColor;

            Vector3 spawnPoint = armyType == ArmyTypes.Defenders
                ? _defendersSpawnPoint
                : _invadersSpawnPoint;

            return _armySpawner.SpawnArmy(
                armyType,
                formation,
                color,
                _battleField,
                spawnPoint,
                out resultPower
            );
        }

    #endregion

    #region BATTLE FLOW

        private void OnBattleStart(FightButtonClickedEvent _)
        {
            if (_battleActive)
                return;

            _battleActive = true;
            _eventBus.Publish(new BattleStartEvent());
        }

        private void OnBattleCompleteNextButtonClicked(BattleCompleteNextButtonClickEvent _)
        {
            ClearArmy(ArmyTypes.Defenders);
            ClearArmy(ArmyTypes.Invaders);

            InitNewBattleSessionAsync().Forget();
        }

        private void OnUnitDied(UnitDiedEvent evt)
        {
            float newPower;
            int newCount;

            if (evt.Unit.ArmyType == ArmyTypes.Defenders)
            {
                _defenders.Remove(evt.Unit);
                newCount = _defenders.Count;
                newPower = _sessionData.DefendersPower - evt.Unit.PowerScore;
                _sessionData.SetDefendersArmyStats(newCount, newPower);
            }
            else
            {
                _invaders.Remove(evt.Unit);
                newCount = _invaders.Count;
                newPower = _sessionData.InvadersPower - evt.Unit.PowerScore;
                _sessionData.SetInvadersArmyStats(newCount, newPower);
            }

            _pool.Return(evt.Unit);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData));

            CheckBattleEnd();
        }

        private void CheckBattleEnd()
        {
            if (_defenders.Count == 0 && _invaders.Count == 0)
            {
                EndBattle(false);
                return;
            }

            if (_defenders.Count == 0)
            {
                EndBattle(false);
                return;
            }

            if (_invaders.Count == 0)
            {
                EndBattle(true);
                return;
            }
        }

        private void EndBattle(bool playerWon)
        {
            _battleActive = false;
            _eventBus.Publish(new BattleCompleteEvent(playerWon));
        }

    #endregion

    #region UTILITY METHODS

        public Unit GetClosestEnemy(IUnitContext requester)
        {
            if (requester is not Unit)
                return null;

            List<Unit> enemyList = requester.ArmyType switch
            {
                ArmyTypes.Defenders => _invaders,
                ArmyTypes.Invaders => _defenders,
                _ => null
            };

            if (enemyList == null)
                return null;

            Unit closest = null;
            float minDist = float.MaxValue;
            Vector3 pos = requester.Transform.position;

            foreach (var enemy in enemyList)
            {
                if (enemy == null || !enemy.IsAttackTarget)
                    continue;

                float dist = (enemy.Transform.position - pos).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }

            return closest;
        }

        public void FillNeighbors(IUnitContext requester, List<Unit> neighborsBuffer)
        {
            if (requester is Unit)
                _spatialGrid.FillNeighbors(requester, neighborsBuffer);
        }

    #endregion

    #region MONOBEHAVIOUR API
        private void Start()
        {
            StartGameplay();
        }

        private void OnEnable() 
        {
            AddListeners();
        }

        private void OnDisable()
        {
            ClearListeners();
        }

        private void Update()
        {
            UpdateGameplay();
        }

        private void OnDestroy()
        {
            CleanUpScene();
        }
    #endregion

    #region CLEANUP
        private void CleanUpScene()
        {
            _battleActive = false;
            _cts.Cancel();

            ClearListeners();

            ClearArmy(ArmyTypes.Defenders);
            ClearArmy(ArmyTypes.Invaders);

            _cts.Dispose();
        }

        private void ClearListeners()
        {
            _eventBus.Unsubscribe<ChangeDefendersFormationEvent>(OnUpdateDefendersFormation);
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Unsubscribe<BattleCompleteNextButtonClickEvent>(OnBattleCompleteNextButtonClicked);
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
        }

        private void ClearArmy(ArmyTypes armyType)
        {
            List<Unit> army = armyType == ArmyTypes.Defenders ? _defenders : _invaders;

            foreach (var unit in army)
            {
                if (unit != null && unit.gameObject != null)
                    _pool.Return(unit);
            }

            army.Clear();
        }

    #endregion
    }
}