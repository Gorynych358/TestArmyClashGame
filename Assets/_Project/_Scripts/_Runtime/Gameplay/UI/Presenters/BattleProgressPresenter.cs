using System;
using ACT.Runtime.GameEvents;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.Gameplay.UI.Presenters
{
    public sealed class BattleProgressPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private BattleProgressView _view;
        private bool _isViewShown = false;

        public BattleProgressPresenter(IEventBus eventBus) => _eventBus = eventBus;

        public void BindView(BattleProgressView view)
        {
            _view = view;
            _view.HideInstant();
            _isViewShown = false;
            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
            _eventBus.Subscribe<ArmyStatsChangedEvent>(OnArmyStatsChanged);
            _eventBus.Subscribe<BattleCompleteEvent>(OnBattleComplete);
        }

        private void OnBattleReady(BattleReadyEvent evt)
        {
            if(!_isViewShown)
            {
                _isViewShown = true;
                _view.Show();
            }
        }

        private void OnArmyStatsChanged(ArmyStatsChangedEvent evt)
        {
            var data = evt.SessionData;

            float defendersPower = data.DefendersPower;
            float invadersPower = data.InvadersPower;

            float total = defendersPower + invadersPower;

            float fill = total > 0f
                ? defendersPower / total
                : 0.5f;

            _view.SetStats(data.DefendersCount, data.InvadersCount, fill);
        }

        private void OnBattleComplete(BattleCompleteEvent _)
        {
            _isViewShown = false;
            _view.HideInstant();
        }
        
        public void Dispose()
        {
            _eventBus.Unsubscribe<ArmyStatsChangedEvent>(OnArmyStatsChanged);
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
            _eventBus.Unsubscribe<BattleCompleteEvent>(OnBattleComplete);
        }
    }
}
