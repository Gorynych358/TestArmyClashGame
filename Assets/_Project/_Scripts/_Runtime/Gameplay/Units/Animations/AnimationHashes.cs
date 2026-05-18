using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Animations
{
    // ================================
    //  Класс для хранения хэшей анимационных состояний юнитов
    // ================================
    public static class AnimationHashes
    {
        public static readonly int Idle    = Animator.StringToHash("Idle");
        public static readonly int Run     = Animator.StringToHash("Run");
        public static readonly int Attack  = Animator.StringToHash("Attack");
        public static readonly int Die     = Animator.StringToHash("Die");
    }        
}