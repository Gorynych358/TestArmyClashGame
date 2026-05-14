namespace ACT.Runtime.Gameplay.Units.Executors
{
    public class UnitAttacker : IAttackSystem
    {
        public void Attack(IDamageable targetUnit, float attackPower)
        {
            targetUnit.ApplyDamage(attackPower);
        }
    }
}
