using System;
namespace ACT.Scripts
{
    public sealed class FightButtonPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private FightButtonView _view;

        public FightButtonPresenter(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void BindView(FightButtonView view)
        {
            _view = view;
            _view.HideInstant();
            _view.SetInteractable(true);

            _view.BindClick(OnButtonClicked);

            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
        }

        private void OnBattleReady(BattleReadyEvent e)
        {
            _view.ShowWithSlide();
        }

        private void OnButtonClicked()
        {
            _view.HideWithSlide();
            _eventBus.Publish(new FightButtonClickedEvent());
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
        }
    }
}
