using System;

namespace ACT.Scripts
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
            _view.SetStats(10, _economyManager.BattleEarnings);
            
            if (e.IsPlayerWon)
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