using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ACT.Runtime.Gameplay.UI.Views
{
    public sealed class CoinsView : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [Header("UI effects container")]
        [SerializeField] private UIVisualEffectsService _uiEffector;

        public void SetCoins(int amount)
        {
            _coinsText.text = amount.ToString();
            _coinsText.transform
                                .DOScale(1.2f, 0.1f)
                                .SetEase(Ease.OutBack)
                                .SetLink(gameObject)
                                .OnComplete(() =>
                                    _coinsText.transform
                                        .DOScale(1f, 0.1f)
                                        .SetLink(gameObject)
                                );
        }    
        
        public void PlayCoinFly(Vector3 worldPos)
        {
            var cam = Camera.main;
            var screenPos = cam.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRoot,
                screenPos,
                null,
                out var uiPos
            );
            _uiEffector.PlayCoinAnim(uiPos);
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}
