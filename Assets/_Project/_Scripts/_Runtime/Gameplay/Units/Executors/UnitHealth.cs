using System;
using UnityEngine;

namespace ACT.Scripts
{
    public class UnitHealth : IUnitHealth
    {
        public float Current { get; private set; }
        public float Max { get; private set; }
        public bool IsAlive => Current > 0;

        public UnitHealth(float maxHealth)
        {
            Max = maxHealth;
            Current = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            Current = Math.Max(0, Current - amount);
            if (Current <= 0)
            {
                Current = 0;
                //EventBus.RaiseUnitDied(_unit);
            }
        }
    }
}
