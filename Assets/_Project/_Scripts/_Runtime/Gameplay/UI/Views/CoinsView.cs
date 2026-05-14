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
        [Header("Animation settings")]
        [SerializeField] private float _showTime = 0.35f;
        [SerializeField] private float _showDelayTime = 0.1f;
        private Vector2 _shownPos;
        private Vector2 _hiddenPos;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _shownPos = _root.anchoredPosition;
            // Смещаем панельку с завоёванными монетками выше, чтобы гарантированно было не видно:
            _hiddenPos = new Vector2(_shownPos.x, _shownPos.y - _root.rect.height * 1.2f);
            _root.anchoredPosition = _hiddenPos;
        }

        public void Show()
        {
            _root.gameObject.SetActive(true);
            _root.DOAnchorPos(_shownPos, _showTime)
                    .SetDelay(_showDelayTime)
                    .SetEase(Ease.OutBack)
                    .SetLink(gameObject);
            _canvasGroup.DOFade(1, _showTime)
                    .SetDelay(_showDelayTime)
                    .SetLink(gameObject);
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
