using System.Collections.Generic;
using UnityEngine;
using VContainer;
using ACT.Runtime.GameEvents;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Gameplay.Units.Executors;
using ACT.Runtime.Gameplay.Units.Logic;
using ACT.Runtime.Gameplay.Units.Logic.FSM;
using ACT.Runtime.Gameplay.Units.Logic.FSM.States;
using ACT.Runtime.Gameplay.Units.Animations;
using ACT.Runtime.Gameplay.Units.UnitConfigurationSystem;

namespace ACT.Runtime.Gameplay.Units
{
    public class Unit : MonoBehaviour, IUnitContext, IDamageable
    {
        [SerializeField] private Transform _modelRoot;

    #region PUBLIC PROPERTIES (IUnitContext)
        public ArmyTypes ArmyType { get; set; }
        public Transform ModelRoot => _modelRoot;
        public Transform Transform => transform;

        public UnitTypes UnitType => _config.UnitType;
        public float UnitSize => _config.SizeMod.SizeScaleFactor;
        public float PowerScore => _config.PowerScore;

        public float MoveSpeed => _config.FinalSPEED;
        public float AttackCooldown => _config.FinalATKSPD;
        public float AttackDistance => 2f; // временно захардкожено

        public int MaxHealth => _config.FinalHP;

        public bool CanAttack { get; set; } = false;
        public bool IsAttackTarget { get; private set; } = true;
        public IUnitContext CurrentTarget { get; set; } = null;
        public Vector3 MoveDirection { get; set; } = Vector3.zero;
    #endregion

    #region PRIVATE FIELDS
        private UnitConfigSO _config;

        private IUnitAnimationController _animController;
        private IEventBus _eventBus;
        private BattleManager _battleManager;
        private ICommandSystem _commandSystem;

        private IAttackSystem _attacker;
        private IMoveSystem _mover;
        private IHealthSystem _healthSystem;

        private StateMachine _stateMachine;
        private Dictionary<UnitStates, IState> _states;

        private bool _isActive;
        private Animator _animator;
    #endregion

    #region DEPENDENCY INJECTION
        [Inject]
        public void Construct(
            IEventBus eventBus,
            BattleManager battleManager,
            ICommandSystem commandSystem,
            IAttackSystem attackSystem,
            IMoveSystem moveSystem)
        {
            _eventBus = eventBus;
            _battleManager = battleManager;
            _commandSystem = commandSystem;
            _attacker = attackSystem;
            _mover = moveSystem;
        }
    #endregion

    #region INITIALIZATION
        public void Initialize(UnitConfigSO config)
        {
            _config = config;

            _healthSystem = new UnitHealth();
            _healthSystem.Initialize(_config.FinalHP);
            _healthSystem.BindZeroHealtCallback(OnCriticalDamageReceived);

            _animator = GetComponentInChildren<Animator>();
            _animController = new UnitAnimationController(_animator);

            _stateMachine = new StateMachine();
            _states = new Dictionary<UnitStates, IState>
            {
                [UnitStates.Idle]    = new IdleState(this),
                [UnitStates.Chase]   = new ChaseState(this),
                [UnitStates.Attack]  = new AttackState(this),
                [UnitStates.Die]     = new DieState(this),
                [UnitStates.Victory] = new VictoryState(this)
            };

            ChangeState(UnitStates.Idle);
        }
    #endregion

    #region RESET UNIT
        public void ResetUnit()
        {
            // Сброс Animator
            _animator.Rebind();
            _animator.Update(0f);
            // Сброс состояния
            IsAttackTarget = true;
            CurrentTarget = null;
            MoveDirection = Vector3.zero;
            ChangeState(UnitStates.Idle);
            // Сброс здоровья
            _healthSystem.Initialize(_config.FinalHP);
        }
    #endregion

    #region UNITY LIFECYCLE
        private void OnEnable()
        {
            _eventBus.Subscribe<BattleStartEvent>(OnBattleStarted);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<BattleStartEvent>(OnBattleStarted);
        }
    #endregion

    #region GAME LOOP
        private void OnBattleStarted(BattleStartEvent evt)
        {
            _isActive = true;
        }

        public void Tick(float deltaTime)
        {
            if (_isActive)
                _commandSystem.Update(this);
            
            _stateMachine.Update(deltaTime);
        }
    #endregion
        
    #region ANIMATIONS (IUnitContext)
        public void PlayIdleAnim() => _animController.PlayIdle();
        public void PlayRunAnim() => _animController.PlayRun();
        public void PlayAttackAnim() => _animController.PlayAttack();
        public void PlayDieAnim() => _animController.PlayDie();

        public bool IsAnimationComplete(int hash) => _animController.IsAnimationComplete(hash);
    #endregion

    #region BEHAVIOR (IUnitContext)
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
            _attacker.Attack(CurrentTarget as Unit, _config.FinalATK);
        }
    #endregion

    #region DAMAGE & DEATH
        public void ApplyDamage(float damage)
        {
            _healthSystem.TakeDamage(damage);
        }

        private void OnCriticalDamageReceived()
        {
            // Юнит остаётся на сцене до завершения анимации смерти
            IsAttackTarget = false;
            _isActive = false;

            ChangeState(UnitStates.Die);
        }

        public void DispatchDeadEvent()
        {
            _eventBus.Publish(new UnitDiedEvent(this));
        }
    #endregion
    }
}