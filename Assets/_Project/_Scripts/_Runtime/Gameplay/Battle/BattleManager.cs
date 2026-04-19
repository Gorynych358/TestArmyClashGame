using System;
using UnityEngine;
using VContainer;
using System.Collections;
using System.Collections.Generic;

namespace ACT.Scripts
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private Transform _battleField;
        [SerializeField] private Transform _poolStorage;
        [SerializeField] private FormationDataSO _defenderFormation;
        [SerializeField] private FormationDataSO _inviderFormation;

        [Header("Spawn Settings")]
        [SerializeField] private int defendersCount = 10;
        [SerializeField] private int enemiesCount = 10;

        [SerializeField] private Vector3 defendersSpawnPoint = new Vector3(-5, 0, 0);
        [SerializeField] private Vector3 enemiesSpawnPoint = new Vector3(5, 0, 0);

        private readonly List<Unit> _defenders = new();
        private readonly List<Unit> _enemies = new();

        private UnitObjectPool _pool;
        private FormationGenerator _formationGenerator;
        private IEventBus _eventBus;
        private SpatialGrid _spatialGrid;
        private bool _battleActive = false;

        public void Initialize(UnitObjectPool pool, FormationGenerator formationGenerator, IEventBus eventBus, SpatialGrid spatialGrid)
        {
            _pool = pool;
            _formationGenerator = formationGenerator;
            _eventBus = eventBus;
            _spatialGrid = spatialGrid;
            print("BattleManager initialized with pool, formation generator, and event bus.");
        }

        private void Start()
        {
            _pool.InitializePool(10, _poolStorage);
            SpawnArmies(3000);
            _eventBus.Subscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
            _eventBus.Publish(new BattleReadyEvent());
            print($"Defender = {_defenders[0].name}, closest enemy = {GetClosestEnemy(_defenders[0])}");
        }

        private void Update()
        {
            if(!_battleActive)
                return;
            _spatialGrid.Clear();
            _spatialGrid.Build(_defenders, _enemies);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
        }

        // --------------------------------------------------------------------
        // BATTLE START
        // --------------------------------------------------------------------
        private void OnBattleStart(FightButtonClickedEvent evt)
        {
            if (_battleActive)
                return;
            _battleActive = true;
            _eventBus.Publish(new BattleStartEvent());
        }

        // --------------------------------------------------------------------
        // SPAWN ARMIES
        // --------------------------------------------------------------------
        private void SpawnArmies(float armyPower)
        {
            ClearArmies();

            float defendersPower;
            float invidersPower;

            var defenders = CreateArmy(ArmyTypes.Defenders, _defenderFormation, _battleField, defendersSpawnPoint, out defendersPower);
            _defenders.AddRange(defenders);

            var inviders = CreateArmy(ArmyTypes.Invaders, _inviderFormation, _battleField, enemiesSpawnPoint, out invidersPower);
            _enemies.AddRange(inviders);

            print($"Defender power/count = {defendersPower}/{_defenders.Count}, inviders power/count = {invidersPower}/{_enemies.Count}");
        }

        private List<Unit> CreateArmy(ArmyTypes armyType, FormationDataSO formationConfig, Transform parent, Vector3 origin, out float totalPower)
        {
            List<Unit> units;
            try
            {
                if (formationConfig != null)
                {
                    units = _formationGenerator.CreateArmy(armyType, formationConfig, parent, origin);
                }
                else
                {
                    units = _formationGenerator.CreateRandomArmy(armyType, armyType == ArmyTypes.Defenders ? defendersCount : enemiesCount, parent, origin);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Formation generation failed for {armyType}: {ex.Message}. Falling back to random spawn.");
                units = _formationGenerator.CreateRandomArmy(armyType, armyType == ArmyTypes.Defenders ? defendersCount : enemiesCount, parent, origin);
            }

            totalPower = 0f;
            foreach (var unit in units)
            {
                if (unit != null)
                    totalPower += unit.PowerScore;
            }

            return units;
        }

        private void ClearArmies()
        {
            //Возвращаем всех юнитов в пул и очищаем списки армий:
            foreach (var unit in _defenders)
                _pool.Return(unit);

            foreach (var unit in _enemies)
                _pool.Return(unit);
            
            _defenders.Clear();
            _enemies.Clear();
        }

        // --------------------------------------------------------------------
        // UNIT DIED EVENT
        // --------------------------------------------------------------------
        private void OnUnitDied(UnitDiedEvent evt)
        {
            //print($"Battle active = {_battleActive}, Unit {evt.Unit.name} is dead!");
            /*if (!_battleActive)
                return;*/

            _defenders.Remove(evt.Unit);
            _enemies.Remove(evt.Unit);

            _pool.Return(evt.Unit);

            CheckBattleEnd();
        }

        // --------------------------------------------------------------------
        // CHECK END OF BATTLE
        // --------------------------------------------------------------------
        private void CheckBattleEnd()
        {
            if (!_battleActive)
                return;

            if (_defenders.Count == 0 && _enemies.Count == 0)
            {
                StartCoroutine(EndBattle(false)); // ничья
                return;
            }

            if (_defenders.Count == 0)
            {
                StartCoroutine(EndBattle(false)); // поражение игрока
                return;
            }

            if (_enemies.Count == 0)
            {
                StartCoroutine(EndBattle(true)); // победа игрока
                return;
            }
        }

        // --------------------------------------------------------------------
        // END BATTLE
        // --------------------------------------------------------------------
        private IEnumerator EndBattle(bool playerWon)
        {
            _battleActive = false;

            List<Unit> winners = playerWon ? _defenders : _enemies;

            // 1. Анимация победы
            foreach (var unit in winners)
            {
                if (unit != null && unit.gameObject.activeSelf)
                    unit.ChangeState(UnitStates.Victory);
            }

            yield return new WaitForSeconds(2f);

            // 2. UI результата
            if(playerWon)
                print("FIGHT COMPLETE! \n Player won!");
            else
                print("FIGHT COMPLETE! \n Invaders won!");
            

            yield return new WaitForSeconds(1f);

            ClearArmies();
        }

        // --------------------------------------------------------------------
        // GET CLOSEST ENEMY
        // --------------------------------------------------------------------
        // ------------------------------------------------------------
        // Ищем ближайшего врага по ArmyType (чисто, без списков)
        // ------------------------------------------------------------
        public Unit GetClosestEnemy(IUnitContext requester)
        {
            if (requester is not Unit unit)
                return null;

            List<Unit> enemyList = requester.ArmyType switch
            {
                ArmyTypes.Defenders => _enemies,
                ArmyTypes.Invaders  => _defenders,
                _                   => null
            };

            if (enemyList == null)
                return null;

            Unit closest = null;
            float minDist = float.MaxValue;
            Vector3 pos = unit.Transform.position;

            foreach (var enemy in enemyList)
            {
                if (enemy == null || !enemy.IsAlive)
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
        public List<Unit> GetNeighbors(IUnitContext requester)
        {
            if (requester is not Unit unit)
                return new List<Unit>(0);

            return _spatialGrid.GetNeighbors(requester.Transform.position);
        }
    }
}
