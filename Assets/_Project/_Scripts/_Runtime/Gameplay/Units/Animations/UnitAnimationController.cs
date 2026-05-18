using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Animations
{
    // ================================
    //  РЕАЛИЗАЦИЯ ДЛЯ UNITY ANIMATOR
    // ================================
    public class UnitAnimationController : IUnitAnimationController
    {
        private readonly Animator _animator;

        private readonly int _runBool     = Animator.StringToHash("IsRunning");
        private readonly int _deadBool    = Animator.StringToHash("IsDead");
        private readonly int _attackTrig  = Animator.StringToHash("AttackTrigger");

        public UnitAnimationController(Animator animator)
        {
            _animator = animator;
        }

        public void PlayIdle()    => _animator.SetBool(_runBool, false);
        public void PlayRun()     => _animator.SetBool(_runBool, true);
        public void PlayAttack()  => _animator.SetTrigger(_attackTrig);
        public void PlayDie()     => _animator.SetBool(_deadBool, true);

        public bool IsAnimationComplete(int hash)
        {
            var info = _animator.GetCurrentAnimatorStateInfo(0);

            if (info.shortNameHash == hash)
                return info.normalizedTime >= 1f && !_animator.IsInTransition(0);

            return true;
        }
    }
}