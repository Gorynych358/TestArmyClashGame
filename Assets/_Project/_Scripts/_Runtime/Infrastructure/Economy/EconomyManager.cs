namespace ACT.Scripts
{
    public sealed class EconomyManager : IEconomyManager
    {
        private readonly IEventBus _eventBus;

        public int Balance { get; private set; }
        public int BattleEarnings { get; private set; }

        public EconomyManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;

            Balance += amount;
            _eventBus.Publish(new EconomyChangedEvent());
        }

        public void SpendCoins(int amount)
        {
            if (amount <= 0 || amount > Balance)
                return;

            Balance -= amount;
            _eventBus.Publish(new EconomyChangedEvent());
        }

        public void BeginBattleSession()
        {
            BattleEarnings = 0;
        }

        public void AddBattleEarnings(int amount)
        {
            if (amount <= 0)
                return;

            BattleEarnings += amount;
            Balance += amount;

            _eventBus.Publish(new EconomyChangedEvent());
        }

        public int GetCoinsAmount() => Balance;
    }
}
