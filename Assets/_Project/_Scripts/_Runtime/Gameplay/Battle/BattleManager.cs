using System;
using UnityEngine;
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
        private readonly List<Unit> _invaders = new();

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
            _eventBus.Subscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Subscribe<BattleCompleteNextButtonClickEvent>(OnBattleCompleteNextButtonClicked);
            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
            StartCoroutine(InitNewBattle());
        }

        private void Update()
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

        private void OnDisable()
        {
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnBattleStart);
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
        }

        // --------------------------------------------------------------------
        // INIT BATTLE
        // --------------------------------------------------------------------
        private IEnumerator InitNewBattle()
        {
            _battleActive = false;

           SpawnArmies(3000);

            yield return new WaitForSeconds(1f);

            // Показываем UI:
            _eventBus.Publish(new BattleReadyEvent());
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
        // BATTLE COMPLETE
        // --------------------------------------------------------------------
        private void OnBattleCompleteNextButtonClicked(BattleCompleteNextButtonClickEvent evt)
        {
            ClearArmies();
            StartCoroutine(InitNewBattle());
        }

        // --------------------------------------------------------------------
        // SPAWN ARMIES
        // --------------------------------------------------------------------
        private void SpawnArmies(float armyPower)
        {
            float defendersPower;
            float invidersPower;

            Color armyColor = RandomPastelColorGenerator.GenerateTeamColor(true);
            var defenders = CreateArmy(ArmyTypes.Defenders, _defenderFormation, armyColor, _battleField, defendersSpawnPoint, out defendersPower);
            _defenders.AddRange(defenders);

            armyColor = RandomPastelColorGenerator.GenerateTeamColor(false);
            var inviders = CreateArmy(ArmyTypes.Invaders, _inviderFormation, armyColor, _battleField, enemiesSpawnPoint, out invidersPower);
            _invaders.AddRange(inviders);

            if(_defenders.Count > 0 && _invaders.Count > 0)
                _eventBus.Publish(new ArmyCountChangedEvent(_defenders.Count, _invaders.Count));
            
            print($"Defender power/count = {defendersPower}/{_defenders.Count}, inviders power/count = {invidersPower}/{_invaders.Count}");
        }

        private List<Unit> CreateArmy(ArmyTypes armyType, FormationDataSO formationConfig, Color color, Transform parent, Vector3 origin, out float totalPower)
        {
            List<Unit> units;
            try
            {
                if (formationConfig != null)
                {
                    units = _formationGenerator.CreateArmy(armyType, formationConfig, color, parent, origin);
                }
                else
                {
                    units = _formationGenerator.CreateRandomArmy(armyType, armyType == ArmyTypes.Defenders ? defendersCount : enemiesCount, color, parent, origin);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Formation generation failed for {armyType}: {ex.Message}. Falling back to random spawn.");
                units = _formationGenerator.CreateRandomArmy(armyType, armyType == ArmyTypes.Defenders ? defendersCount : enemiesCount, color, parent, origin);
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
            print($"Clear armies! _defenders.Count = {_defenders.Count}, _invaders.Count = {_invaders.Count}");
            //Возвращаем всех юнитов в пул и очищаем списки армий:
            
            foreach (var unit in _defenders)
            {
                if (unit != null && unit.gameObject != null)
                    _pool.Return(unit);
            }
            _defenders.Clear();
            
            
            foreach (var unit in _invaders)
            {
                if (unit != null && unit.gameObject != null)
                    _pool.Return(unit);
            }
            _invaders.Clear();
        }

        // --------------------------------------------------------------------
        // UNIT DIED EVENT
        // --------------------------------------------------------------------
        private void OnUnitDied(UnitDiedEvent evt)
        {
            if(evt.Unit.ArmyType == ArmyTypes.Defenders)
            {
                _defenders.Remove(evt.Unit);
                print($"Defender removed from defenders.list! defenders list count = {_defenders.Count}");  
            }
            else if(evt.Unit.ArmyType == ArmyTypes.Invaders)
            {
                _invaders.Remove(evt.Unit);
                print($"Invader removed from invaders.list! invaders list count = {_invaders.Count}"); 
            }
                
            _pool.Return(evt.Unit);
            _eventBus.Publish(new ArmyCountChangedEvent(_defenders.Count, _invaders.Count));
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
            _eventBus.Publish(new BattleCompleteEvent(playerWon));
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
                ArmyTypes.Defenders => _invaders,
                ArmyTypes.Invaders  => _defenders,
                _                   => null
            };
            print("GCE " + enemyList);
            if (enemyList == null)
                return null;

            Unit closest = null;
            float minDist = float.MaxValue;
            Vector3 pos = unit.Transform.position;

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
        public List<Unit> GetNeighbors(IUnitContext requester)
        {
            if (requester is not Unit unit)
                return new List<Unit>(0);

            return _spatialGrid.GetNeighbors(requester.Transform.position);
        }
    }
}
