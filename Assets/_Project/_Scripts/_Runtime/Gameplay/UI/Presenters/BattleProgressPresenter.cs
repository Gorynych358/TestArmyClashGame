using System;
using DG.Tweening;
using UnityEngine;

namespace ACT.Scripts
{
    public sealed class BattleProgressPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private BattleProgressView _view;

        private int _defendersAlive = 0;
        private int _invadersAlive = 0;

        public BattleProgressPresenter(IEventBus eventBus) => _eventBus = eventBus;

        public void BindView(BattleProgressView view)
        {
            _view = view;
            _view.HideInstant();
            _view.SetCounts(_defendersAlive, _invadersAlive);

            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
            
            _eventBus.Subscribe<UnitDiedEvent>(OnUnitDied);
        }

        private void OnBattleReady(BattleReadyEvent e)
        {
            _view.Show();
        }

        private void OnUnitDied(UnitDiedEvent e)
        {
            if (e.Unit.ArmyType == ArmyTypes.Defenders)
                _defendersAlive = Mathf.Max(0, _defendersAlive - 1);
            else
                _invadersAlive = Mathf.Max(0, _invadersAlive - 1);

            _view.SetCounts(_defendersAlive, _invadersAlive);
            _eventBus.Publish(new ArmyCountChangedEvent(_defendersAlive, _invadersAlive));
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<UnitDiedEvent>(OnUnitDied);
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
        }
    }
}
