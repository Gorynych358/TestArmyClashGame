using UnityEngine;
using DG.Tweening;
using VContainer;

namespace ACT.Scripts
{
    public class UIVisualEffectsService : MonoBehaviour
    {
        [SerializeField] private RectTransform _effectorContainer;

        private CoinObjectPool _coinPool;

        [Inject]
        private void Construct(CoinObjectPool coinPool)
        {
            _coinPool = coinPool;
        }

        public void PlayCoinAnim(Vector2 startPos, Vector2 endPos)
        {
            var coin = _coinPool.Get();
            coin.SetParent(_effectorContainer, false);

            var rect = coin;
            var canvasGroup = coin.GetComponent<CanvasGroup>();

            rect.anchoredPosition = startPos;
            endPos.y = startPos.y + 160;
            rect.localScale = new Vector3(0.3f, 0.3f);
            canvasGroup.alpha = 1f;

            Sequence seq = DOTween.Sequence();

            seq.Append(rect.DOScale(1.0f, 0.5f).SetEase(Ease.OutQuad));
            seq.Join(rect.DOAnchorPos(endPos, 1.0f).SetEase(Ease.OutBack));
            seq.Join(canvasGroup.DOFade(0f, 0.2f).SetDelay(0.8f));

            seq.OnComplete(() =>
            {
                _coinPool.Return(rect);
            });
        }
    }
}
