using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace ACT.Scripts
{
    public sealed class SceneTransitionView : MonoBehaviour
    {
        [Header("Fade")]
        [SerializeField] private CanvasGroup fadeGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Loading")]
        [SerializeField] private CanvasGroup loadingGroup;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text loadingText;

        private CancellationTokenSource _cts;
        private bool _isFading;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public CancellationToken Token => _cts.Token;

        public UniTask FadeIn() => SafeFade(0f, 1f);

        public UniTask FadeOut() => SafeFade(1f, 0f);

        private async UniTask SafeFade(float from, float to)
        {
            if (_isFading)
                return;

            _isFading = true;
            try
            {
                await Fade(fadeGroup, from, to, fadeDuration, Token);
            }
            finally
            {
                _isFading = false;
            }
        }

        public void ShowLoading()
        {
            loadingGroup.alpha = 1f;
            loadingGroup.blocksRaycasts = true;
            UpdateProgress(0f);
        }

        public void HideLoading()
        {
            loadingGroup.alpha = 0f;
            loadingGroup.blocksRaycasts = false;
        }

        public void UpdateProgress(float progress)
        {
            progressBar.value = progress;
            loadingText.text = $"Загрузка... {Mathf.RoundToInt(progress * 100f)}%";
        }

        private async UniTask Fade(
            CanvasGroup group,
            float from,
            float to,
            float duration,
            CancellationToken token)
        {
            float t = 0f;
            group.alpha = from;
            group.blocksRaycasts = true;

            while (t < duration)
            {
                token.ThrowIfCancellationRequested();

                t += Time.deltaTime;
                group.alpha = Mathf.Lerp(from, to, t / duration);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            group.alpha = to;
            group.blocksRaycasts = false;
        }
    }
}