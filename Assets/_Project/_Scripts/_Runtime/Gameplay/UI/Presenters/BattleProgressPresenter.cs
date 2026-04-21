using System;
using UnityEngine;

namespace ACT.Scripts
{
    public sealed class BattleProgressPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private BattleProgressView _view;

        public BattleProgressPresenter(IEventBus eventBus) => _eventBus = eventBus;

        public void BindView(BattleProgressView view)
        {
            _view = view;
            _view.HideInstant();

            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
            
            _eventBus.Subscribe<ArmyCountChangedEvent>(OnArmyCountChanged);
        }

        private void OnBattleReady(BattleReadyEvent e)
        {
            _view.Show();
        }

        private void OnArmyCountChanged(ArmyCountChangedEvent e)
        {
            _view.SetCounts(e.DefendersCount, e.InvadersCount);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<ArmyCountChangedEvent>(OnArmyCountChanged);
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
        }
    }
}
