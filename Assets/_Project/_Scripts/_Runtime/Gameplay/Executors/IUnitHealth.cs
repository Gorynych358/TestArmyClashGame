using UnityEngine;

namespace ACT.Scripts
{
    public interface IUnitHealth
    {
        float Current { get; }
        float Max { get; }
        bool IsAlive { get; }

        void TakeDamage(float amount);
    }
}
