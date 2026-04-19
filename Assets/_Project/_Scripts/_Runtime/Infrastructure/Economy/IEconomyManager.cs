namespace ACT.Scripts
{
    public interface IEconomyManager
    {
        int Coins { get; }

        void AddCoins(int amount);
        int GetCoinsAmount();
        void SpendCoins(int amount);
    }
}