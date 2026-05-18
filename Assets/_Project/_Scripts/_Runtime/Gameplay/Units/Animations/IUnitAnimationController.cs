namespace ACT.Runtime.Gameplay.Units.Animations
{
    // ================================
    //  Специализированный интерфейс для анимаций юнитов
    // ================================
    public interface IUnitAnimationController
    {
        void PlayIdle();
        void PlayRun();
        void PlayAttack();
        void PlayDie();

        bool IsAnimationComplete(int hash);
    }
}