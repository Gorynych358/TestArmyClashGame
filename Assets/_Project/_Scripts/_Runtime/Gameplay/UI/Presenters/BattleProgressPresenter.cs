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
            
            _eventBus.Subscribe<ArmyStatsChangedEvent>(OnArmyStatsChanged);
        }
        
        private void OnBattleReady(BattleReadyEvent evt)
        {
            _view.Show();
        }

        private void OnArmyStatsChanged(ArmyStatsChangedEvent evt)
        {
            BattleSessionData data = evt.SessionData;

            float defendersPower = data.DefendersPower;
            float invadersPower = data.InvadersPower;

            float total = defendersPower + invadersPower;

            float fill = total > 0f
                ? defendersPower / total
                : 0.5f;

            _view.SetStats(data.DefendersCount, data.InvadersCount, fill);
        }

        private (float defendersPower, float invadersPower) 
            RecalculatePowers(float defendersPower, float invadersPower )
        {
            
            return (defendersPower, invadersPower);
        }
        public void Dispose()
        {
            _eventBus.Unsubscribe<ArmyStatsChangedEvent>(OnArmyStatsChanged);
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
        }
    }
}
