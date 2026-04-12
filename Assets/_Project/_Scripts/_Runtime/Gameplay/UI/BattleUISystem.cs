using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ACT.Scripts
{
    public class BattleUISystem : MonoBehaviour
    {
        [SerializeField] private GameObject _fightPanel;
        [SerializeField] private Button _fightButton;

        private IFightButtonPresenter _presenter;

        public void Construct(IFightButtonPresenter presenter)
        {
            _presenter = presenter;
            HideInstant();

            if (_fightButton != null)
                _fightButton.onClick.AddListener(() => _presenter?.OnFightClicked());
        }

        private void OnDestroy()
        {
            if (_fightButton != null)
                _fightButton.onClick.RemoveAllListeners();
        }

        public void HideInstant()
        {
            if (_fightPanel != null)
                _fightPanel.SetActive(false);
        }

        public void Show()
        {
            if (_fightPanel != null)
                _fightPanel.SetActive(true);
        }
    }

    public interface IFightButtonPresenter
    {
        void OnFightClicked();
    }

    public class BattleUIFightButtonPresenter : IFightButtonPresenter
    {
        private readonly BattleUISystem _view;
        private readonly IEventBus _eventBus;

        public BattleUIFightButtonPresenter(BattleUISystem view, IEventBus eventBus)
        {
            _view = view;
            _eventBus = eventBus;

            _view.Construct(this);
            _eventBus.Subscribe<BattleReadyEvent>(OnBattleReady);
        }

        private void OnBattleReady(BattleReadyEvent evt)
        {
            _view.Show();
        }

        public void OnFightClicked()
        {
            _eventBus.Publish(new BattleStartEvent());
        }
    }
}
