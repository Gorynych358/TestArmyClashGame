namespace ACT.Runtime.Infrastructure.Economy
{
    public interface IEconomyManager
    {
        int Balance { get; }
        int BattleEarnings { get; }

        void AddCoins(int amount);          // покупки, бонусы, награды вне боя
        void SpendCoins(int amount);
        void BeginBattleSession();          // обнулить доход за бой
        void AddBattleEarnings(int amount); // доход за бой

        int GetCoinsAmount();
    }
}