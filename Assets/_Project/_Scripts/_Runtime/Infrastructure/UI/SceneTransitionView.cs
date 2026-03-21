using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public async UniTask FadeIn()
        {
            await Fade(fadeGroup, 0f, 1f, fadeDuration);
        }

        public async UniTask FadeOut()
        {
            await Fade(fadeGroup, 1f, 0f, fadeDuration);
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
            float duration)
        {
            float t = 0f;
            group.alpha = from;
            group.blocksRaycasts = true;

            while (t < duration)
            {
                t += Time.deltaTime;
                group.alpha = Mathf.Lerp(from, to, t / duration);
                await UniTask.Yield();
            }

            group.alpha = to;
            group.blocksRaycasts = false;
        }
    }
}
