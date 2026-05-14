using System;

namespace ACT.Runtime.Gameplay.Units.Executors
{
    public interface IHealthSystem
    {
        float Current { get; }
        bool IsAlive { get; }
        void BindZeroHealtCallback(Action onCriticalDamageReceived);
        void Initialize(float maxHealth);
        void TakeDamage(float amount);
    }
}
