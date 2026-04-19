using System;
using System.Numerics;
using DG.Tweening;

namespace ACT.Scripts
{
    public sealed class CoinsPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly EconomyManager _economy;

        private CoinsView _view;

        public CoinsPresenter(IEventBus eventBus, EconomyManager economy)
        {
            _eventBus = eventBus;
            _economy = economy;
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

        private void OnUnitDied(UnitDiedEvent e)
        {
            if (e.Unit.ArmyType != ArmyTypes.Defenders)
                return;
            
            var worldPos = e.Unit.transform.position;
            _view.PlayCoinFly(worldPos);

            // Начисляем монеты после старта/по окончании анимации — здесь упрощённо сразу:
            _economy.AddCoins(10);
        }

        private void OnBattleReady(BattleReadyEvent e)
        {
            _view.Show();
        }

        private void OnEconomyChanged(EconomyChangedEvent e)
        {
            if (_view == null)
                return;

            _view.SetCoins(_economy.GetCoinsAmount());
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
            _eventBus.Unsubscribe<EconomyChangedEvent>(OnEconomyChanged);
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
        }
    }
}
