using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace ACT.Runtime.Gameplay.UI.Views
{
    public sealed class BattleCompleteView : MonoBehaviour
    {
        [Header("UI elements references")]
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private GameObject _victoryBanner;
        [SerializeField] private GameObject _loseBanner;

        [SerializeField] private TextMeshProUGUI _victoryEnemyKilledText;
        [SerializeField] private TextMeshProUGUI _victoryGoldEarnedText;
        [SerializeField] private TextMeshProUGUI _looseGoldEarnedText;

        [SerializeField] private UIVisualEffectsService _uiEffector;
        [SerializeField] private Button _continueButton;

        [Header("Animation Settings")]
        [SerializeField] private float _showDelayTime = 0.2f;
        [SerializeField] private float _animDuration = 0.45f;

        private Vector2 _hiddenLeftPos;
        private Vector2 _shownPos;
        private Vector2 _hiddenRightPos;

        public void HideInstant()
        {
            gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;
            _root.anchoredPosition = _hiddenLeftPos;
        }

        public void SetStats(int enemyKilled, int goldEarned)
        {
            _victoryEnemyKilledText.text = enemyKilled.ToString();
            _victoryGoldEarnedText.text = goldEarned.ToString();
            _looseGoldEarnedText.text = goldEarned.ToString();
        }

        public void BindClick(Action onClick)
        {
            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void ShowVictory()
        {
            _victoryBanner.SetActive(true);
            _loseBanner.SetActive(false);

            ShowInternal();
            _uiEffector.PlayConfetti();
        }

        public void ShowLose()
        {
            _victoryBanner.SetActive(false);
            _loseBanner.SetActive(true);

            ShowInternal();
        }

        private void ShowInternal()
        {
            gameObject.SetActive(true);

            float screenWidth = _canvasRoot.rect.width;
            float centerY = 0f;

            _hiddenLeftPos = new Vector2(-screenWidth, centerY);
            _shownPos = new Vector2(0f, centerY);
            _hiddenRightPos = new Vector2(screenWidth, centerY);

            _canvasGroup.alpha = 0f;
            _root.anchoredPosition = _hiddenLeftPos;

            _canvasGroup
                .DOFade(1f, 0.3f)
                .SetDelay(_showDelayTime)
                .SetLink(gameObject);

            _root
                .DOAnchorPos(_shownPos, _animDuration)
                .SetDelay(_showDelayTime)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }

        public void HideAnimated()
        {
            _canvasGroup
                .DOFade(0f, 0.25f)
                .SetLink(gameObject);

            _root
                .DOAnchorPos(_hiddenRightPos, _animDuration)
                .SetEase(Ease.InBack)
                .SetLink(gameObject)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}