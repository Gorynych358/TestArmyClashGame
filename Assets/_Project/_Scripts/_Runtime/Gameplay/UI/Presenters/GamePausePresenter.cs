using System;
using ACT.Runtime.GameEvents.UIEvents;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.Gameplay.UI.Presenters
{
    // ============================================================================
    //  GamePausePresenter.cs
    // ============================================================================
    public sealed class GamePausePresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private GamePauseView _view;

        public GamePausePresenter(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void BindView(GamePauseView view)
        {
            _view = view;

            _view.HideInstant();

            _view.BindContinue(OnContinueClicked);
            _view.BindMainMenu(OnMainMenuClicked);

            _eventBus.Subscribe<PauseButtonPressedEvent>(OnPausePressed);
        }

        private void OnPausePressed(PauseButtonPressedEvent evt)
        {
            _view.ShowAnimated();
        }

        private void OnContinueClicked()
        {
            _view.HideAnimated();
            _eventBus.Publish(new ResumeButtonPressedEvent());
        }

        private void OnMainMenuClicked()
        {
            _view.HideAnimated();
            _eventBus.Publish(new BackButtonPressedEvent());
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<PauseButtonPressedEvent>(OnPausePressed);
        }
    }
}