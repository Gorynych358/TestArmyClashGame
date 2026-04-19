using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ACT.Scripts
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
            print("MainMenuManager initialized with scene transition manager and event bus.");
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
