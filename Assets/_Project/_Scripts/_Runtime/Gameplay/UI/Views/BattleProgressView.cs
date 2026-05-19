using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ACT.Runtime.Gameplay.UI.Views
{
    public sealed class BattleProgressView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform _progressBarFill;
        [SerializeField] private TextMeshProUGUI _defendersAmountText;
        [SerializeField] private TextMeshProUGUI _invadersAmountText;
        [Header("UI references")]
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [Header("Animation settings")]
        [SerializeField] private float _showDelayTime = 0f;
        [SerializeField] private float _showDuration = 0.35f;
        [SerializeField] private float _animDuration = 0.25f;
        private const float SHOW_TIME = 0.3f;
        private Vector2 _shownPos;
        private Vector2 _hiddenPos;

        private void Awake()
        {
            _shownPos = _root.anchoredPosition;
            float top = _canvasRoot.rect.yMax;
            // Смещаем панельку с прогрессом выше, чтобы гарантированно не видно было:
            _hiddenPos = new Vector2(_shownPos.x, top + _root.rect.height * 1.2f);
            _root.anchoredPosition = _hiddenPos;
        }

        public void Show()
        {
            _root.gameObject.SetActive(true);
            _root.anchoredPosition = _hiddenPos;
            _root.DOAnchorPos(_shownPos, _showDuration)
                    .SetDelay(_showDelayTime)
                    .SetEase(Ease.OutBack)
                    .SetLink(gameObject);
        }
        public void HideInstant()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetStats(
            int defendersAlive,
            int invadersAlive,
            float fillScaleX)
        {
            // Обновляем текст
            _defendersAmountText.text = defendersAlive.ToString();
            _invadersAmountText.text = invadersAlive.ToString();

            // Масштабируем только зелёную заливку
            _progressBarFill
                .DOScaleX(fillScaleX, _animDuration)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}
