using System;
using UnityEngine;
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

            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
            _eventBus.Subscribe<BattleStartEvent>(OnBattleStart);
            _eventBus.Subscribe<BattleCompleteEvent>(OnBattleComplete);
            _eventBus.Subscribe<ResumeButtonPressedEvent>(OnResumeClicked);

            _view.SetInteractable(false);
            _view.ShowAnimated();
            _view.ShowBackButton();
        }

        private void OnBattleReady(BattleReadyEvent _)
        {
            _view.ShowBackButton();
            _view.SetInteractable(true);
        }
        private void OnBattleStart(BattleStartEvent evt)
        {
            _view.ShowPauseButton();
        }

        private void OnBattleComplete(BattleCompleteEvent _)
        {
            _view.SetInteractable(false);
        }


        private void OnResumeClicked(ResumeButtonPressedEvent evt)
        {
            _view.SetInteractable(true);
        }

        private void OnBackClicked()
        {
            _eventBus.Publish(new BackButtonPressedEvent());
            _view.HideInstant();
        }

        private void OnPauseClicked()
        {
            _eventBus.Publish(new PauseButtonPressedEvent());
            _view.SetInteractable(false);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BattleReadyEvent>(OnBattleReady);
            _eventBus.Unsubscribe<BattleStartEvent>(OnBattleStart);
            _eventBus.Unsubscribe<BattleCompleteEvent>(OnBattleComplete);
            _eventBus.Unsubscribe<ResumeButtonPressedEvent>(OnResumeClicked);
        }
    }
}