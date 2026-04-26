using System;

namespace ACT.Scripts
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
