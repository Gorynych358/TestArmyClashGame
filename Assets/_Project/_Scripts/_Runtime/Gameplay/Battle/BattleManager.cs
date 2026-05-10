using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace ACT.Scripts
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private Transform _battleField;
        [SerializeField] private Transform _poolStorage;
        [SerializeField] private FormationDataSO _defendersFormation;
        [SerializeField] private FormationDataSO _invadersFormation;

        [Header("Spawn Settings")]
        [SerializeField] private float _armyPower = 3000f;

        [SerializeField] private Vector3 _defendersSpawnPoint = new Vector3(-5, 0, 0);
        [SerializeField] private Vector3 _invadersSpawnPoint = new Vector3(5, 0, 0);

        private readonly List<Unit> _defenders = new();
        private readonly List<Unit> _invaders = new();

        private UnitObjectPool _pool;
        private RandomArmyCalculator _randomArmyCalculator;
        private FormationBuilder _formationBuilder;
        private ArmySpawner _armySpawner;
        private IEventBus _eventBus;
        private SpatialGrid _spatialGrid;
        private BattleSessionData _sessionData;
        private bool _battleActive = false;

        private CancellationTokenSource _cts;

        public void Initialize(UnitObjectPool pool, RandomArmyCalculator randomArmyCalculator, 
            FormationBuilder formationBuilder, ArmySpawner armySpawner, IEventBus eventBus, 
            SpatialGrid spatialGrid)
        {
            _cts = new CancellationTokenSource();
            _pool = pool;
            _randomArmyCalculator = randomArmyCalculator;
            _formationBuilder = formationBuilder;
            _armySpawner = armySpawner;
            _eventBus = eventBus;
            _spatialGrid = spatialGrid;
            print("BattleManager initialized with pool, formation generator, and event bus.");
        }


        private void UpdateGameplay()
        {
            if(!_battleActive)
                return;
            
            _spatialGrid.Clear();
            _spatialGrid.Build(_defenders, _invaders);

            for(int i = _defenders.Count - 1; i >= 0; i--)
                _defenders[i].Tick(Time.deltaTime);
                
            for(int i = _invaders.Count - 1; i >= 0; i--)
                _invaders[i].Tick(Time.deltaTime);
        }
    

        // --------------------------------------------------------------------
        // INIT BATTLE
        // --------------------------------------------------------------------
        private async UniTask InitNewBattleSessionAsync()
        {
            _battleActive = false;

            _sessionData = new BattleSessionData(_armyPower);
            Color defendersColor = RandomPastelColorGenerator.GeneratePastelColor();
            Color invadersColor = RandomPastelColorGenerator.GenerateContrastColor(defendersColor);
            _sessionData.SetColors(defendersColor, invadersColor);
            BuildArmies();

            await UniTask.Delay(500, cancellationToken:_cts.Token);

            // Показываем UI:
            _eventBus.Publish(new BattleReadyEvent());
        }

        // --------------------------------------------------------------------
        // UPDATE DEFENDERS FORMATION
        // --------------------------------------------------------------------
        private void OnUpdateDefendersFromation(ChangeDefendersFormationEvent _)
        {
            RebuildDefendersArmy();
            // Показываем UI:
            DOVirtual.DelayedCall(3, () => _eventBus.Publish(new BattleReadyEvent()));
            //_eventBus.Publish(new BattleReadyEvent());
            print("Start battle scene = " + this);
        }

        // --------------------------------------------------------------------
        // BATTLE START
        // --------------------------------------------------------------------
        private void OnBattleStart(FightButtonClickedEvent _)
        {
            if (_battleActive)
                return;
            _battleActive = true;
            _eventBus.Publish(new BattleStartEvent());
            print("Start battle scene = " + this);
        }

        // --------------------------------------------------------------------
        // BATTLE COMPLETE
        // --------------------------------------------------------------------
        private void OnBattleCompleteNextButtonClicked(BattleCompleteNextButtonClickEvent evt)
        {
            print("Before clearing scene = " + this);
            ClearArmy(ArmyTypes.Defenders);
            ClearArmy(ArmyTypes.Invaders);
            print("After clearing scene = " + this);
            InitNewBattleSessionAsync().Forget();
        }

        // --------------------------------------------------------------------
        // INITIAL ARMIES BUILD
        // --------------------------------------------------------------------
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

            print($"Defender power|count = {_sessionData.DefendersPower}|{_sessionData.DefendersCount}, inviders power|count = {_sessionData.InvadersPower}|{_sessionData.InvadersCount}");
        }

        private void RebuildDefendersArmy()
        {
            //Очищаем текущую армию защитников:
            ClearArmy(ArmyTypes.Defenders);
            //Создаём новую армию:
            float defendersPower;

            //Для защитников созадём армию +/- 20% рандома от базовой мощи армий:
            float armyPower = _sessionData.ArmyPower;
            float powerRange = armyPower * 0.2f;
            armyPower += UnityEngine.Random.Range(-powerRange, powerRange);
            List<UnitTypes> rndUnitTypes = _randomArmyCalculator.GenerateArmy(armyPower);
            FormationRuntimeData formation = _formationBuilder.BuildRandom(rndUnitTypes);
            
            var defenders = CreateArmy(ArmyTypes.Defenders, formation, out defendersPower);
            _defenders.AddRange(defenders);
            _sessionData.SetDefendersArmyStats(_defenders.Count, defendersPower);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData));
        }

        private List<Unit> CreateArmy(ArmyTypes armyType, IFormationData formation, out float resultPower)
        {
            Color color = _sessionData.DefendersColor;
            Vector3 spawnPoint = _defendersSpawnPoint;
            if(armyType == ArmyTypes.Invaders)
            {
                color = _sessionData.InvadersColor;
                spawnPoint = _invadersSpawnPoint;
            }
            
            var army = _armySpawner.SpawnArmy(armyType, 
                                            formation, 
                                            color, 
                                            _battleField, 
                                            spawnPoint,
                                            out resultPower);
            return army;
        }

        private void ClearArmy(ArmyTypes armyType)
        {
            List<Unit> army;
            //Возвращаем всех оставшихся юнитов в пул и очищаем список армии:
            if(armyType == ArmyTypes.Defenders)
                army = _defenders;
            else 
                army = _invaders;
            
            foreach (var unit in army)
            {
                if (unit != null && unit.gameObject != null)
                    _pool.Return(unit);
            }
            army.Clear();
        }

        // --------------------------------------------------------------------
        // UNIT DIED EVENT
        // --------------------------------------------------------------------
        private void OnUnitDied(UnitDiedEvent evt)
        {
            float power;
            int count;
            if(evt.Unit.ArmyType == ArmyTypes.Defenders)
            {
                _defenders.Remove(evt.Unit);
                count = _defenders.Count;
                power = _sessionData.DefendersPower - evt.Unit.PowerScore;
                _sessionData.SetDefendersArmyStats(count, power);
                //print($"Defender removed from defenders.list! defenders list count = {_defenders.Count}");  
            }
            else if(evt.Unit.ArmyType == ArmyTypes.Invaders)
            {
                _invaders.Remove(evt.Unit);
                count = _invaders.Count;
                power = _sessionData.InvadersPower - evt.Unit.PowerScore;
                _sessionData.SetInvadersArmyStats(count, power);
                //print($"Invader removed from invaders.list! invaders list count = {_invaders.Count}"); 
            }
                
            _pool.Return(evt.Unit);
            _eventBus.Publish(new ArmyStatsChangedEvent(_sessionData));
            CheckBattleEnd();
        }

        // --------------------------------------------------------------------
        // CHECK END OF BATTLE
        // --------------------------------------------------------------------
        private void CheckBattleEnd()
        {

            if (_defenders.Count == 0 && _invaders.Count == 0)
            {
                EndBattle(false); // ничья
                return;
            }

            if (_defenders.Count == 0)
            {
                EndBattle(false); // поражение игрока
                return;
            }

            if (_invaders.Count == 0)
            {
                EndBattle(true); // победа игрока
                return;
            }
        }

        // --------------------------------------------------------------------
        // END BATTLE
        // --------------------------------------------------------------------
        private void EndBattle(bool playerWon)
        {
            _battleActive = false;

            /*List<Unit> winners = playerWon ? _defenders : _invaders;

            // 1. Анимация победы
            foreach (var unit in winners)
            {
                if (unit != null && unit.gameObject.activeSelf)
                    unit.ChangeState(UnitStates.Victory);
            }*/

            // 2. UI результата
            print("Before BattleCompleteEvent scene = " + this);
            _eventBus.Publish(new BattleCompleteEvent(playerWon));
        }

        // --------------------------------------------------------------------
        // GET CLOSEST ENEMY
        // Ищем ближайшего врага
        // ------------------------------------------------------------
        public Unit GetClosestEnemy(IUnitContext requester)
        {
            if (requester is not Unit)
                return null;

            List<Unit> enemyList = requester.ArmyType switch
            {
                ArmyTypes.Defenders => _invaders,
                ArmyTypes.Invaders  => _defenders,
                _                   => null
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

        // ------------------------------------------------------------
        // Ищем локальных соседей через SpatialGrid
        // ------------------------------------------------------------
        public void FillNeighbors(IUnitContext requester, List<Unit> neighborsBuffer)
        {
            if (requester is not Unit)
                return;
            
            _spatialGrid.FillNeighbors(requester, neighborsBuffer);
        }

    #region MONOBEHAVIOUR
        private void Start()
        {
            _pool.InitializePool(10, _poolStorage);
            _eventBus.Subscribe<ChangeDefendersFormationEvent>(OnUpdateDefendersFromation);
            _eventBus.Subscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Subscribe<BattleCompleteNextButtonClickEvent>(OnBattleCompleteNextButtonClicked);
            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
            InitNewBattleSessionAsync().Forget();
        }

        private void Update()
        {
            UpdateGameplay();
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<ChangeDefendersFormationEvent>(OnUpdateDefendersFromation);
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Unsubscribe<BattleCompleteNextButtonClickEvent>(OnBattleCompleteNextButtonClicked);
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
        }

        private void OnDestroy()
        {
            _battleActive = false;
            //ClearArmy(ArmyTypes.Defenders);
            //ClearArmy(ArmyTypes.Invaders);
            _cts.Cancel();
            _cts.Dispose();
        }
    #endregion
    }
}
