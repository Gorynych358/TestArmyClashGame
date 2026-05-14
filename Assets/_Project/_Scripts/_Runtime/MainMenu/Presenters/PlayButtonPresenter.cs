using ACT.Runtime.GameEvents;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.MainMenu.Views;

namespace ACT.Runtime.MainMenu.Presenters
{
    public sealed class PlayButtonPresenter
    {
        private readonly IEventBus _eventBus;
        private PlayButtonView _view;

        public PlayButtonPresenter(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void BindView(PlayButtonView view)
        {
            _view = view;
            _view.Show();
            _view.SetInteractable(true);

            _view.BindClick(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _view.SetInteractable(false);
            _eventBus.Publish(new PlayButtonClickedEvent());
        }
    }
}
