namespace ACT.Scripts
{

    public sealed class EconomyManager : IEconomyManager
    {
        private readonly IEventBus _eventBus;

        public int Coins { get; private set; }

        public EconomyManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;

            Coins += amount;
            _eventBus.Publish(new EconomyChangedEvent());
        }

        public void SpendCoins(int amount)
        {
            if (amount <= 0 || amount > Coins)
                return;

            Coins -= amount;
            _eventBus.Publish(new EconomyChangedEvent());
        }

        public int GetCoinsAmount() => Coins;
    }
}
