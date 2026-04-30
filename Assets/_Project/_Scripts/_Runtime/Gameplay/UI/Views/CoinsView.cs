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
        private Vector2 _shownPos;
        private Vector2 _hiddenPos;

        private void Awake()
        {
            _shownPos = _root.anchoredPosition;
            float top = _canvasRoot.rect.yMax;
            // Смещаем панельку с завоёванными монетками выше, чтобы гарантированно было не видно:
            _hiddenPos = new Vector2(_shownPos.x, top + _root.rect.height * 1.2f);
            _root.anchoredPosition = _hiddenPos;
        }

        public void Show()
        {
            _root.gameObject.SetActive(true);
            _root.DOAnchorPos(_shownPos, 0.35f)
                    .SetDelay(_showDelayTime)
                    .SetEase(Ease.OutCubic)
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
