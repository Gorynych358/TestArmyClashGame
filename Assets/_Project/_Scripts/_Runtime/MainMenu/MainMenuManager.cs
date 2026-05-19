using UnityEngine;
using Cysharp.Threading.Tasks;
using ACT.Runtime.GameEvents;
using ACT.Runtime.Infrastructure.SceneManagement;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.GameEvents.UIEvents;

namespace ACT.Runtime.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        private ISceneTransitionManager _sceneManager;
        private IEventBus _eventBus;
        public void Initialize(ISceneTransitionManager sceneManager, IEventBus eventBus)
        {
            _sceneManager = sceneManager;
            _eventBus = eventBus;
            _eventBus.Subscribe<PlayButtonClickedEvent>(OnPlayButtonClicked);
        }

        private void OnDestroy()
        {
            if(_eventBus != null)
                _eventBus.Unsubscribe<PlayButtonClickedEvent>(OnPlayButtonClicked);
        }

        // --------------------------------------------------------------------
        // LOAD GAMEPLAY SCENE
        // --------------------------------------------------------------------
        private void OnPlayButtonClicked(PlayButtonClickedEvent evt)
        {
            _sceneManager.LoadGameplay().Forget();
        }
    }
}
