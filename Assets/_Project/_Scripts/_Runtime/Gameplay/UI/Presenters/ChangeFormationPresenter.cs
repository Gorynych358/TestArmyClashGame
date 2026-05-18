using UnityEngine;
using System;
using ACT.Runtime.GameEvents;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.GameEvents.UIEvents;

namespace ACT.Runtime.Gameplay.UI.Presenters
{
    // ===========================
    // PRESENTER
    // ===========================
    public sealed class ChangeFormationPresenter : IDisposable
    {
        private readonly IEventBus _eventBus;
        private ChangeFormationView _view;

        public ChangeFormationPresenter(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void BindView(ChangeFormationView view)
        {
            _view = view;
            _view.HideInstant();
            _view.SetInteractable(true);

            _view.BindClick(OnChangeFormationClicked);
            
            _eventBus.Subscribe<BattleReadyEvent>(OnInvadersFormationReady);
            _eventBus.Subscribe<FightButtonClickedEvent>(OnFightButtonClicked);
        }

        private void OnFightButtonClicked(FightButtonClickedEvent _)
        {
            _view.HideAnimated();
        }

        private void OnInvadersFormationReady(BattleReadyEvent _)
        {
            _view.SetInteractable(true);
            _view.Show();
        }

        private void OnChangeFormationClicked()
        {
            _view.SetInteractable(false);
            _eventBus.Publish(new ChangeFormationClickedEvent());
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BattleReadyEvent>(OnInvadersFormationReady);
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnFightButtonClicked);
        }
    }
}