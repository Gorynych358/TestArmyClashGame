using System;
using ACT.Runtime.GameEvents;
using ACT.Runtime.GameEvents.UIEvents;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Infrastructure.Economy;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.Gameplay.UI.Presenters
{
    public sealed class BattleCompletePresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly IEconomyManager _economyManager;
        private BattleCompleteView _view;

        public BattleCompletePresenter(IEventBus eventBus, IEconomyManager economyManager)
        {
            _eventBus = eventBus;
            _economyManager = economyManager;
            _eventBus.Subscribe<BattleCompleteEvent>(OnFightComplete);
        }

        public void BindView(BattleCompleteView view)
        {
            _view = view;
            _view.BindClick(OnNextButtonClicked);
            _view.HideInstant();
        }

        private void OnFightComplete(BattleCompleteEvent e)
        {
            var data = e.SessionData;
            bool isPlayerWon;
            int enemyKilled;
            if(data.DefendersCount > 0)
            {
                isPlayerWon = true;
                enemyKilled = data.InitialInvadersCount;
            }
            else
            {
                isPlayerWon = false;
                enemyKilled = data.InitialInvadersCount - data.InvadersCount;
            }
            _view.SetStats(enemyKilled, _economyManager.BattleEarnings);
            
            if (isPlayerWon)
                _view.ShowVictory();
            else
                _view.ShowLose();
        }

        private void OnNextButtonClicked()
        {
            _view.HideAnimated();
            _eventBus.Publish(new BattleCompleteNextButtonClickEvent());
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BattleCompleteEvent>(OnFightComplete);
        }
    }
}