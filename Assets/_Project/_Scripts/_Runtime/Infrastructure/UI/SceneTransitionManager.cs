using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;

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
            var token = _view.Token;

            await _view.FadeIn().AttachExternalCancellation(token);

            _view.ShowLoading();

            var op = SceneManager.LoadSceneAsync(scene);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                token.ThrowIfCancellationRequested();

                _view.UpdateProgress(op.progress / 0.9f);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _view.UpdateProgress(1f);
            await UniTask.Delay(300, cancellationToken: token);

            op.allowSceneActivation = true;

            await UniTask.WaitUntil(() => op.isDone, cancellationToken: token);

            _view.HideLoading();

            await _view.FadeOut().AttachExternalCancellation(token);
        }
    }
}