namespace ACT.Scripts
{
    public class UnitAttacker : IAttacker
    {
        public void Attack(IDamageable targetUnit, float attackPower)
        {
            targetUnit.ApplyDamage(attackPower);
        }
    }
}
