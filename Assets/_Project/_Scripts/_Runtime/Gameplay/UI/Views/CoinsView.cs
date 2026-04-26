using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ACT.Scripts
{
    public sealed class CoinsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private UIVisualEffectsService _uiEffector;
        [SerializeField] private float _showDelayTime = 0.1f;

        public void Show()
        {
            _root.gameObject.SetActive(true);
            _root.anchoredPosition = new Vector2(400f, _root.anchoredPosition.y);
            _root.DOAnchorPosX(0f, 0.35f)
                    .SetDelay(_showDelayTime)
                    .SetEase(Ease.OutCubic);
        }
        public void HideInstant()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetCoins(int amount)
        {
            _coinsText.text = amount.ToString();
            _coinsText.transform
                                .DOScale(1.2f, 0.1f)
                                .SetEase(Ease.OutBack)
                                .OnComplete(() =>
                                    _coinsText.transform.DOScale(1f, 0.1f)
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
            _root.DOKill();
            _canvasRoot.DOKill();
        }
    }
}
