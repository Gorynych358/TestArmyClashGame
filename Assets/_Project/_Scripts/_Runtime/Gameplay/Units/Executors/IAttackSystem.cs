namespace ACT.Runtime.Gameplay.Units.Executors
{
    public interface IAttackSystem
    {
        void Attack(IDamageable targetUnit, float attackPower);
    }
}
