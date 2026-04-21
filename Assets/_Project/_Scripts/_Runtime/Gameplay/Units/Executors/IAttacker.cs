using UnityEngine;

namespace ACT.Scripts
{
    public interface IAttacker
    {
        void Attack(IDamageable targetUnit, float attackPower);
    }
}
