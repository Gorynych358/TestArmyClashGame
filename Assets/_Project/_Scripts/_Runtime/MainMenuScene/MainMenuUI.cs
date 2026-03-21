using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace ACT.Scripts
{
    public class MainMenuUI : MonoBehaviour
    {
        private ISceneTransitionManager _sceneManager;

        [Inject]
        public void Construct(ISceneTransitionManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public void OnClickPlayButton()
        {
            _sceneManager.LoadGameplay().Forget();
        }
    }
}
