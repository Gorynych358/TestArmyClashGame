using System;

namespace ACT.Scripts
{
    public sealed class CoinsPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly IEconomyManager _economy;
        private readonly GameEconomyProfileSO _economyProfile;

        private CoinsView _view;

        public CoinsPresenter(IEventBus eventBus, IEconomyManager economy, GameEconomyProfileSO economyProfile)
        {
            _eventBus = eventBus;
            _economy = economy;
            _economyProfile = economyProfile;
        }

        public void BindView(CoinsView view)
        {
            _view = view;
            _view.HideInstant();
            _view.SetCoins(_economy.GetCoinsAmount());

            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
            _eventBus.Subscribe<EconomyChangedEvent>(OnEconomyChanged);
            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
        }

        private void OnBattleReady(BattleReadyEvent e)
        {
            _economy.BeginBattleSession();
            _view.Show();
            _view.SetCoins(_economy.GetCoinsAmount());
        }

        private void OnUnitDied(UnitDiedEvent e)
        {
            if (e.Unit.ArmyType != ArmyTypes.Defenders)
                return;

            var worldPos = e.Unit.transform.position;
            _view.PlayCoinFly(worldPos);

            _economy.AddBattleEarnings(_economyProfile.CoinsPerKill);
        }

        private void OnEconomyChanged(EconomyChangedEvent e)
        {
            if (_view == null)
                return;

            _view.SetCoins(_economy.BattleEarnings);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
            _eventBus.Unsubscribe<EconomyChangedEvent>(OnEconomyChanged);
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
        }
    }
}