using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ACT.Scripts
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
            Debug.Log("Show change formation button: ");
            _view.SetInteractable(true);
            _view.Show();
        }

        private void OnChangeFormationClicked()
        {
            _view.SetInteractable(false);
            _eventBus.Publish(new ChangeDefendersFormationEvent());
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BattleReadyEvent>(OnInvadersFormationReady);
            _eventBus.Unsubscribe<FightButtonClickedEvent>(OnFightButtonClicked);
        }
    }
}