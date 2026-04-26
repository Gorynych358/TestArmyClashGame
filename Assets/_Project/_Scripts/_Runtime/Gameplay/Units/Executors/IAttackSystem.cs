using UnityEngine;

namespace ACT.Scripts
{
    public interface IAttackSystem
    {
        void Attack(IDamageable targetUnit, float attackPower);
    }
}
