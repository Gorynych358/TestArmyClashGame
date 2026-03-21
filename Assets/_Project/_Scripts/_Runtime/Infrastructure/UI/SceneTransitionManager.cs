using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ACT.Scripts
{
    public sealed class SceneTransitionManager : ISceneTransitionManager
    {
        private readonly SceneTransitionView _view;

        public SceneTransitionManager(SceneTransitionView view)
        {
            _view = view;
        }

        public UniTask LoadMainMenu() =>
            LoadScene(Scenes.MainMenu);

        public UniTask LoadGameplay() =>
            LoadScene(Scenes.Gameplay);

        private async UniTask LoadScene(string scene)
        {
            await _view.FadeIn();
            _view.ShowLoading();

            var op = SceneManager.LoadSceneAsync(scene);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                _view.UpdateProgress(op.progress / 0.9f);
                await UniTask.Yield();
            }

            _view.UpdateProgress(1);
            await UniTask.Delay(500);

            op.allowSceneActivation = true;
            while (!op.isDone) await UniTask.Yield();

            _view.HideLoading();
            await _view.FadeOut();
        }
    }
}
