using System;
using ACT.Runtime.GameEvents;
using ACT.Runtime.GameEvents.UIEvents;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.Gameplay.UI.Presenters
{
    // ============================================================================
    //  GameControlPresenter.cs
    // ============================================================================
    public sealed class GameControlPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private GameControlView _view;

        public GameControlPresenter(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void BindView(GameControlView view)
        {
            _view = view;

            _view.HideInstant();
            _view.BindBack(OnBackClicked);
            _view.BindPause(OnPauseClicked);

            _eventBus.Subscribe<BattleStartEvent>(OnBattleStart);
            _eventBus.Subscribe<ResumeButtonPressedEvent>(OnResume);

            _view.ShowAnimated();
            _view.ShowBackButton();
        }

        private void OnBattleStart(BattleStartEvent evt)
        {
            _view.ShowPauseButton();
        }

        private void OnResume(ResumeButtonPressedEvent evt)
        {
            _view.ShowPauseButton();
        }

        private void OnBackClicked()
        {
            _eventBus.Publish(new BackButtonPressedEvent());
            _view.HideInstant();
        }

        private void OnPauseClicked()
        {
            _eventBus.Publish(new PauseButtonPressedEvent());
            _view.HideInstant();
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BattleStartEvent>(OnBattleStart);
            _eventBus.Unsubscribe<ResumeButtonPressedEvent>(OnResume);
        }
    }
}