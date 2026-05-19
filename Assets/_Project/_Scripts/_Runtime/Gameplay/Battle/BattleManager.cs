using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using ACT.Runtime.GameEvents;
using ACT.Runtime.Gameplay.Units;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.Infrastructure;
using ACT.Runtime.Gameplay.Battle.Session;
using ACT.Runtime.GameEvents.UIEvents;

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
        [SerializeField] private Vector3 _defendersSpawnPoint = new(-5, 0, 0);
        [SerializeField] private Vector3 _invadersSpawnPoint = new(5, 0, 0);

        private readonly List<Unit> _defenders = new();
        private readonly List<Unit> _invaders = new();

        private ArmyPowerSettingsSO _armyPowerSettings;
        private UnitObjectPool _pool;
        private RandomArmyCalculator _randomArmyCalculator;
        private FormationBuilder _formationBuilder;
        private ArmySpawner _armySpawner;
        private IEventBus _eventBus;
        private SpatialGrid _spatialGrid;

        private BattleSession _sessionData;
        private CancellationTokenSource _cts;

        private bool _battleActive;

    #region INITIALIZATION

        public void Initialize(
            ArmyPowerSettingsSO armyPowerSettingsSO,
            UnitObjectPool pool,
            RandomArmyCalculator randomArmyCalculator,
            FormationBuilder formationBuilder,
            ArmySpawner armySpawner,
            IEventBus eventBus,
            SpatialGrid spatialGrid)
        {
            _cts = new CancellationTokenSource();

            _armyPowerSettings = armyPowerSettingsSO;
            _pool = pool;
            _pool.InitializePool(10, _poolStorage);
            _randomArmyCalculator = randomArmyCalculator;
            _formationBuilder = formationBuilder;
            _armySpawner = armySpawner;
            _spatialGrid = spatialGrid;
            _eventBus = eventBus;
            if(Application.isEditor)
                Debug.Log("BattleManager initialized!");
        }

    #endregion

    #region GAME LOOP

        public void StartBattle()
        {
            AddListeners();
            InitNewBattleSessionAsync().Forget();
        }

        private void AddListeners()
        {
            if (_eventBus == null)
            {
                Debug.LogWarning("EventBus is not assigned in BattleManager! Check if the game starts from the correct scene.");
                return;
            }
            _eventBus.Subscribe<ChangeFormationClickedEvent>(OnUpdateDefendersFormation);
            _eventBus.Subscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Subscribe<BattleCompleteNextButtonClickEvent>(OnBattleCompleteNextButtonClicked);
            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
        }

        public void UpdateGameplay(float deltaTime)
        {
            if (!_battleActive)
                return;

            _spatialGrid.Clear();
            _spatialGrid.Build(_defenders, _invaders);

            for (int i = _defenders.Count - 1; i >= 0; i--)
                _defenders[i].Tick(deltaTime);

            for (int i = _invaders.Count - 1; i >= 0; i--)
                _invaders[i].Tick(deltaTime);
        }

    #endregion

    #region BATTLE SESSION

        private async UniTask InitNewBattleSessionAsync()
        {
            _battleActive = false;
            
            _sessionData = new BattleSession(_armyPowerSettings.ArmyPower);

            Color defendersColor = RandomPastelColorGenerator.GeneratePastelColor();
            Color invadersColor = RandomPastelColorGenerator.GenerateContrastColor(defendersColor);
            _sessionData.SetColors(defendersColor, invadersColor);

            BuildArmies();

            await UniTask.Delay(500, cancellationToken: _cts.Token);

            _eventBus.Publish(new BattleReadyEvent());
        }
    #endregion

    #region FORMATION & ARMY BUILDING

        private void OnUpdateDefendersFormation(ChangeFormationClickedEvent _)
        {
            RebuildDefendersArmy();
            //Без особой необходимости, симулируем нагрузку от генерации:
            DOVirtual.DelayedCall(0.8f, () =>
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

            //Устанавливаем мощность и количество юнитов в армии захватчиков:
            _sessionData.SetArmyStats(ArmyTypes.Invaders, _invaders.Count, invadersPower);
            
            RebuildDefendersArmy();

            if(Application.isEditor)
            {
                var data = _sessionData.GetCurrentData();
                Debug.Log($"Defenders: {data.DefendersPower}|{data.DefendersCount}, " +
                    $"Invaders: {data.InvadersPower}|{data.InvadersCount}");
            }
        }

        private void RebuildDefendersArmy()
        {
            ClearArmy(ArmyTypes.Defenders);
            //Армию защитников выбираем рандомно +/- 20% от целевой мощности армии выбранной в главном меню:
            float basePower = _sessionData.ArmyPower;
            float range = basePower * 0.2f;
            float randomizedPower = basePower + Random.Range(-range, range);

            List<UnitTypes> rndUnitTypes = _randomArmyCalculator.GenerateArmy(randomizedPower);
            FormationRuntimeData formation = _formationBuilder.BuildRandom(rndUnitTypes);

            var defenders = CreateArmy(ArmyTypes.Defenders, formation, out float defendersPower);
            _defenders.AddRange(defenders);

            //Устанавливаем мощность и количество юнитов в армии защитников:
            _sessionData.SetArmyStats(ArmyTypes.Defenders, _defenders.Count, defendersPower);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData.GetCurrentData()));
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

            //Фиксируем начальное состояние армий:
            _sessionData.FixInitialArmyStats();
            
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
            var data = _sessionData.GetCurrentData();
            if (evt.Unit.ArmyType == ArmyTypes.Defenders)
            {
                _defenders.Remove(evt.Unit);
                newCount = _defenders.Count;
                newPower = data.DefendersPower - evt.Unit.PowerScore;
            }
            else
            {
                _invaders.Remove(evt.Unit);
                newCount = _invaders.Count;
                newPower = data.InvadersPower - evt.Unit.PowerScore;
            }
            
            newPower = Mathf.Max(0.0f, newPower);
            _sessionData.SetArmyStats(evt.Unit.ArmyType, newCount, newPower);
            
            _pool.Return(evt.Unit);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData.GetCurrentData()));

            CheckBattleEnd();
        }

        private void CheckBattleEnd()
        {
            if (_defenders.Count > 0 && _invaders.Count > 0)
                return;

            //Битва окончена:
            _battleActive = false;
            //Заканчиваем сессию:
            _sessionData.SessionComplete();
            
            _eventBus.Publish(new BattleCompleteEvent(_sessionData.GetFinalData()));
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
        private void OnDisable() => ClearListeners();  
    #endregion

    #region CLEANUP
        public void DisposeScene()
        {
            _battleActive = false;
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }

            ClearListeners();

            ClearArmy(ArmyTypes.Defenders);
            ClearArmy(ArmyTypes.Invaders);
        }

        private void ClearListeners()
        {
            _eventBus.Unsubscribe<ChangeFormationClickedEvent>(OnUpdateDefendersFormation);
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