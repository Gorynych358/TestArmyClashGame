using System;
using UnityEngine;
using System.Collections.Generic;

namespace ACT.Scripts
{
    public class BattleManager
    {
        public Transform BattleField { get; set; }
        public Transform PoolStorage { get; set; }
        public FormationDataSO DefenderFormation { get; set; }
        public FormationDataSO InvaderFormation { get; set; }

        private UnitObjectPool _pool;
        private FormationGenerator _formationGenerator;
        private IEventBus _eventBus;

        private readonly List<Unit> _defenders = new();
        private readonly List<Unit> _enemies = new();

        [Header("Spawn Settings")]
        [SerializeField] private int defendersCount = 10;
        [SerializeField] private int enemiesCount = 10;

        [SerializeField] private Vector3 defendersSpawnPoint = new Vector3(-5, 0, 0);
        [SerializeField] private Vector3 enemiesSpawnPoint = new Vector3(5, 0, 0);

        private bool _battleActive = false;

        public void Initialize(UnitObjectPool pool, FormationGenerator formationGenerator, IEventBus eventBus)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _formationGenerator = formationGenerator ?? throw new ArgumentNullException(nameof(formationGenerator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _eventBus.Subscribe<BattleStartEvent>(OnBattleStart);
            _eventBus.Subscribe<UnitDeadEvent>(HandleUnitDied);

            if (PoolStorage != null)
            {
                _pool.InitializePool(10, PoolStorage);
            }
            else
            {
                Debug.LogWarning("BattleManager: PoolStorage is not assigned before Initialize.");
            }
        }

        public void GenerateArmies()
        {
            if (BattleField == null)
            {
                Debug.LogWarning("BattleManager: BattleField is not assigned before GenerateArmies.");
            }

            SpawnArmies(3000);
            _eventBus.Publish(new BattleReadyEvent());

            if (_defenders.Count > 0)
                Debug.Log($"Defender = {_defenders[0].name}, closest enemy = {GetClosestEnemy(_defenders[0])}");
        }

        // --------------------------------------------------------------------
        // BATTLE START
        // --------------------------------------------------------------------
        private void OnBattleStart(BattleStartEvent evt)
        {
            if (_battleActive)
                return;

            _battleActive = true;
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

            var inviders = CreateArmy(ArmyTypes.Inviders, _inviderFormation, _battleField, enemiesSpawnPoint, out invidersPower);
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
                    units = CreateRandomArmy(armyType, armyType == ArmyTypes.Defenders ? defendersCount : enemiesCount, parent, origin);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Formation generation failed for {armyType}: {ex.Message}. Falling back to random spawn.");
                units = CreateRandomArmy(armyType, armyType == ArmyTypes.Defenders ? defendersCount : enemiesCount, parent, origin);
            }

            totalPower = 0f;
            foreach (var unit in units)
            {
                if (unit != null)
                    totalPower += unit.PowerScore;
            }

            return units;
        }

        private List<Unit> CreateRandomArmy(ArmyTypes armyType, int count, Transform parent, Vector3 origin)
        {
            var units = new List<Unit>();
            Vector3 facingDirection = armyType == ArmyTypes.Inviders ? Vector3.left : Vector3.right;

            for (int i = 0; i < count; i++)
            {
                var unit = _pool.Get((UnitTypes)UnityEngine.Random.Range(0, 11), parent);
                unit.transform.position = origin + new Vector3(0, 0, i * 2.5f);
                unit.transform.rotation = Quaternion.LookRotation(facingDirection, Vector3.up);
                unit.name = $"{armyType}_{i}_{unit.UnitType}";
                units.Add(unit);
            }

            return units;
        }

        private void ClearArmies()
        {
            _defenders.Clear();
            _enemies.Clear();
        }

        // --------------------------------------------------------------------
        // UNIT DIED EVENT
        // --------------------------------------------------------------------
        private void HandleUnitDied(UnitDeadEvent evt)
        {
            print($"Battle active = {_battleActive}, Unit {evt.Unit.name} is dead!");
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
                EndBattle(false); // ничья
                return;
            }

            if (_defenders.Count == 0)
            {
                EndBattle(false); // поражение игрока
                return;
            }

            if (_enemies.Count == 0)
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

            List<Unit> winners = playerWon ? _defenders : _enemies;

            // 1. Анимация победы
            foreach (var unit in winners)
            {
                if (unit != null && unit.gameObject.activeSelf)
                    unit.ChangeState(UnitStates.Victory);
            }

            // 2. UI результата
            //BattleUI.Instance.ShowResult(playerWon);

            // 3. Возврат всех юнитов в пул
            foreach (var unit in _defenders)
                _pool.Return(unit);

            foreach (var unit in _enemies)
                _pool.Return(unit);

            ClearArmies();
        }

        // --------------------------------------------------------------------
        // GET CLOSEST ENEMY
        // --------------------------------------------------------------------
        public Unit GetClosestEnemy(IUnitContext requester)
        {
            var myUnit = requester as Unit;
            if (myUnit == null)
                return null;

            bool isDefender = _defenders.Contains(myUnit);
            var enemyList = isDefender ? _enemies : _defenders;

            Unit closest = null;
            float minDist = Mathf.Infinity;

            foreach (var enemy in enemyList)
            {
                if (enemy == null || !enemy.gameObject.activeSelf)
                    continue;

                float dist = Vector3.Distance(myUnit.Transform.position, enemy.Transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }

            return closest;
        }
    }
}
