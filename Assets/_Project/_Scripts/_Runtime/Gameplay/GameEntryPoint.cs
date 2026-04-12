using UnityEngine;
using VContainer;

namespace ACT.Scripts
{
    public class GameEntryPoint : MonoBehaviour
    {
        [Header("Battle References")]
        [SerializeField] private Transform _battleField;
        [SerializeField] private Transform _poolStorage;
        [SerializeField] private FormationDataSO _defenderFormation;
        [SerializeField] private FormationDataSO _invaderFormation;

        [Header("Spawn Settings")]
        [SerializeField] private int _defendersCount = 10;
        [SerializeField] private int _enemiesCount = 10;
        [SerializeField] private Vector3 _defendersSpawnPoint = new Vector3(-5, 0, 0);
        [SerializeField] private Vector3 _enemiesSpawnPoint = new Vector3(5, 0, 0);

        private BattleManager _battleManager;
        private UnitObjectPool _pool;
        private FormationGenerator _formationGenerator;
        private IEventBus _eventBus;

        [Inject]
        public void Construct(
            BattleManager battleManager,
            UnitObjectPool pool,
            FormationGenerator formationGenerator,
            IEventBus eventBus)
        {
            _battleManager = battleManager;
            _pool = pool;
            _formationGenerator = formationGenerator;
            _eventBus = eventBus;
        }

        private void Start()
        {
            if (_battleManager == null || _pool == null || _formationGenerator == null || _eventBus == null)
            {
                Debug.LogError("GameEntryPoint: missing dependencies.");
                return;
            }

            _battleManager.BattleField = _battleField;
            _battleManager.PoolStorage = _poolStorage;
            _battleManager.DefenderFormation = _defenderFormation;
            _battleManager.InvaderFormation = _invaderFormation;
            _battleManager.DefendersCount = _defendersCount;
            _battleManager.EnemiesCount = _enemiesCount;
            _battleManager.DefendersSpawnPoint = _defendersSpawnPoint;
            _battleManager.EnemiesSpawnPoint = _enemiesSpawnPoint;

            _battleManager.Initialize(_pool, _formationGenerator, _eventBus);
            _battleManager.GenerateArmies();
        }
    }
}
