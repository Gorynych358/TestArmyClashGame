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
        public bool IsAttackTarget{get;private set;}
        public IUnitContext CurrentTarget { get; set; }
        public Vector3 MoveDirection{get;set;}
        public float AttackDistance => 2f;
    #endregion
    #region PRIVATE FIELDS
        private UnitConfigSO _config;
        private IEventBus _eventBus;
        private Dictionary<UnitStates, IState> _states;
        private BattleManager _battleManager;
        private ICommandSystem _commandSystem;
        private IAttackSystem _attacker;
        private IMoveSystem _mover;
        private IHealthSystem _healthSystem;

        private StateMachine _stateMachine;

        private IdleState _idleState;
        private ChaseState _chaseState;
        private AttackState _attackState;
        private DieState _dieState;
        private VictoryState _victoryState;

        private bool _isActive = false;

        private Animator _animator;
    #endregion

        [Inject]
        public void Construct(
            IEventBus eventBus,
            BattleManager battleManager,
            ICommandSystem commandSystem,
            IAttackSystem attackSystem,
            IMoveSystem moveSystem
            )
        {
            _eventBus = eventBus;
            _battleManager = battleManager;
            _commandSystem = commandSystem;
            _attacker = attackSystem;
            _mover = moveSystem;

            _healthSystem = new UnitHealth();
            //Передаём колбэк критического урона в HealtSystem:
            _healthSystem.BindZeroHealtCallback(OnCriticalDamageReceived);
            _animator = this.GetComponentInChildren<Animator>();
            _stateMachine = new StateMachine();
             _states = new Dictionary<UnitStates, IState>
            {
                [UnitStates.Idle] = new IdleState(this),
                [UnitStates.Chase] = new ChaseState(this),
                [UnitStates.Attack] = new AttackState(this),
                [UnitStates.Die] = new DieState(this),
                [UnitStates.Victory] = new VictoryState(this)
            };
        }

        public void BindConfig(UnitConfigSO config) => _config = config;

        public void Initialize()
        {
            _healthSystem.Initialize(_config.FinalHP);
            IsAttackTarget = true;
            CurrentTarget = null;
            MoveDirection = Vector3.zero;
            ChangeState(UnitStates.Idle);
        }

        private void OnBattleStarted(BattleStartEvent evt)
        {
            _isActive = true;
        }

        public void Tick(float deltaTime)
        {
            if(_isActive)
                _commandSystem.Update(this);
            //print($"Unit name = {this.name}, Move direction = {MoveDirection}, current state = {_stateMachine.GetCurrent()}");
            _stateMachine.Update(deltaTime);
        }

        public void ChangeState(UnitStates type)
        {
            _stateMachine.ChangeState(_states[type]);
        }

        public void Move(Vector3 direction, float deltaTime)
        {
            _mover.Move(this, direction, _config.FinalSPEED, deltaTime);
        }

        public void Attack()
        {
            //print($"Unit with name {this.name.ToUpper()} attacks unit with name {((Unit)CurrentTarget).name.ToUpper()} ");
            _attacker.Attack(this, _config.FinalATK);
        }

        private void OnCriticalDamageReceived()
        {
            IsAttackTarget = false;
            _isActive = false;
            ChangeState(UnitStates.Die);
        }

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
            //print($"Unit with name {this.name.ToUpper()} take damage with strength {damage}, max health = {_healthSystem.Max} current health = {_healthSystem.Current}");
        }
    }
}
