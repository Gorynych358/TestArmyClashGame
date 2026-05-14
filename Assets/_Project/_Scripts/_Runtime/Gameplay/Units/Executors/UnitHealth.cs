using System;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Executors
{
    public class UnitHealth : IHealthSystem
    {
        public float Current { get; private set; }
        public bool IsAlive => Current > 0;

        private Action _criticalDamageCallback;

        public void Initialize(float maxHealth)
        {
            Current = maxHealth;
        }

        public void BindZeroHealtCallback(Action zeroHealthCallback)
        {
            _criticalDamageCallback = zeroHealthCallback;
        }

        public void TakeDamage(float amount)
        {
            Current = Math.Max(0, Current - amount);
            if (Current <= 0)
            {
                Current = 0;
                _criticalDamageCallback?.Invoke();
            }
        }
    }
}
