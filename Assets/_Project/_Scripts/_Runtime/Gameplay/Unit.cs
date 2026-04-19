using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ACT.Scripts
{
    public class Unit : MonoBehaviour, IUnitContext, IDamageable
    {

        [SerializeField] Transform _modelRoot;
    #region PUBLIC PROPERTIES
        public ArmyTypes ArmyType{get;set;}
        public Transform ModelRoot => _modelRoot;
        public Transform Transform => transform;
        public UnitTypes UnitType => _config.UnitType;
        public float UnitSize => _config.SizeMod.SizeScaleFactor;
        public float PowerScore => _config.PowerScore;
        
        public float MoveSpeed => _config.FinalSPEED;
        public float AttackCooldown => _config.FinalATKSPD;
        public int MaxHealth => _config.FinalHP;

        public bool CanAttack { get; set; }

        public float AttackDistance => 2f;

        public bool IsAlive => _healthSystem.IsAlive;

        public IUnitContext CurrentTarget { get; set; }
        public Vector3 MoveDirection{get;set;}
    #endregion
    #region PRIVATE FIELDS
        private UnitConfigSO _config;
        private IEventBus _eventBus;
        private Dictionary<UnitStates, IState> _states;
        private BattleManager _battleManager;
        private ICommandSystem _commandSystem;
        private IUnitHealth _healthSystem;
        private IAttacker _attacker;
        private IUnitMover _mover;

        private StateMachine _stateMachine;

        private IdleState _idleState;
        private ChaseState _chaseState;
        private AttackState _attackState;
        private DieState _dieState;
        private VictoryState _victoryState;

        private bool _isBattleStarted;
    #endregion

        [Inject]
        public void Construct(
            IEventBus eventBus,
            BattleManager battleManager,
            ICommandSystem commandSystem
            )
        {
            _eventBus = eventBus;
            _battleManager = battleManager;
            _commandSystem = commandSystem;
        }

        public void Initialize(UnitConfigSO config)
        {
            _config = config;
            _healthSystem = new UnitHealth(_config.FinalHP);
            _attacker = new UnitAttacker();
            _mover = new UnitMover();
            
            _stateMachine = new StateMachine();

             _states = new Dictionary<UnitStates, IState>
            {
                [UnitStates.Idle] = new IdleState(this),
                [UnitStates.Chase] = new ChaseState(this),
                [UnitStates.Attack] = new AttackState(this),
                [UnitStates.Die] = new DieState(this),
                [UnitStates.Victory] = new VictoryState(this)
            };

            ChangeState(UnitStates.Idle);
        }

        private void OnBattleStarted(BattleStartEvent evt)
        {
            _isBattleStarted = true;
        }

        public void Update()
        {
            if(!_isBattleStarted)
                return;
            
            _commandSystem.Update(this);
           

            if (!_healthSystem.IsAlive)
            {
                //ChangeState(UnitStates.Die);
                DispatchDeadEvent();
                //this.transform.GetComponentInChildren<Material>().color = new Color32(0,0,0,128);
                return;
            }
            //print($"Unit name = {this.name}, Move direction = {MoveDirection}, current state = {_stateMachine.GetCurrent()}");
            _stateMachine.Update();
        }

        public void ChangeState(UnitStates type)
        {
            _stateMachine.ChangeState(_states[type]);
        }

        public void Move(Vector3 direction) => _mover.Move(this, direction, _config.FinalSPEED);

        public void Attack() => _attacker.Attack(this, _config.FinalATK);

        public void DispatchDeadEvent()
        {
            _eventBus.Publish(new UnitDiedEvent(this));
        }

        private void OnEnable() 
        {
            _eventBus.Subscribe<BattleStartEvent>(OnBattleStarted);
        }
        private void OnDisable() 
        {
            _eventBus.Unsubscribe<BattleStartEvent>(OnBattleStarted);
        }

        public void ApplyDamage(float damage)
        {
            _healthSystem.TakeDamage(damage);
            print($"Unit with name {this.name.ToUpper()} take damage with strength {damage}, max health = {_healthSystem.Max} current health = {_healthSystem.Current}");
        }
    }
}
