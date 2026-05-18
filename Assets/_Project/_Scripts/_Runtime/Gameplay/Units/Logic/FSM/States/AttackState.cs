using ACT.Runtime.Gameplay.Units.Animations;

namespace ACT.Runtime.Gameplay.Units.Logic.FSM.States
{
    public class AttackState : BaseUnitState
    {
        private float _cooldownTime;
        public AttackState(IUnitContext context) : base(context) { }

        public override void Enter()
        {
            _cooldownTime = UnityEngine.Random.Range(0, context.AttackCooldown); // Случайная задержка перед первой атакой
            context.PlayAttackAnim();
        }

        public override void Update(float deltaTime)
        {
            if (!context.CanAttack)
            {
                context.ChangeState(UnitStates.Chase);
                return;
            }

            // Ждём завершения анимации
            if (!context.IsAnimationComplete(AnimationHashes.Attack))
                return;
            
            _cooldownTime -= deltaTime;
            
            if(_cooldownTime <= 0)
            {
                _cooldownTime = context.AttackCooldown;
                context.PlayAttackAnim();
                context.Attack();
            }
        }
    }
}
