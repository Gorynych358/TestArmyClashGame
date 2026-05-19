using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ACT.Runtime.Gameplay.UI.Views
{
    // ============================================================================
    //  GamePauseView.cs
    // ============================================================================

    public sealed class GamePauseView : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _mainMenuButton;

        [Header("Animation settings")]
        [SerializeField] private float _showDelayTime = 0.15f;
        [SerializeField] private float _animDuration = 0.35f;

        private Vector2 _hiddenLeftPos;
        private Vector2 _shownPos;
        private Vector2 _hiddenRightPos;

        private void Awake()
        {
            float screenWidth = _canvasRoot.rect.width;
            float centerY = 0f;

            _hiddenLeftPos = new Vector2(-screenWidth, centerY);
            _shownPos = new Vector2(0f, centerY);
            _hiddenRightPos = new Vector2(screenWidth, centerY);

            _root.anchoredPosition = _hiddenLeftPos;
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void HideInstant()
        {
            gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;
            _root.anchoredPosition = _hiddenLeftPos;
        }

        public void BindContinue(Action onClick)
        {
            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void BindMainMenu(Action onClick)
        {
            _mainMenuButton.onClick.RemoveAllListeners();
            _mainMenuButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void ShowAnimated()
        {
            gameObject.SetActive(true);

            _canvasGroup.alpha = 0f;
            _root.anchoredPosition = _hiddenLeftPos;

            _canvasGroup
                .DOFade(1f, _animDuration)
                .SetUpdate(true)
                .SetDelay(_showDelayTime)
                .SetLink(gameObject);

            _root
                .DOAnchorPos(_shownPos, _animDuration)
                .SetUpdate(true)
                .SetDelay(_showDelayTime)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }

        public void HideAnimated()
        {
            _canvasGroup
                .DOFade(0f, _animDuration)
                .SetUpdate(true)
                .SetLink(gameObject);

            _root
                .DOAnchorPos(_hiddenRightPos, _animDuration)
                .SetUpdate(true)
                .SetEase(Ease.InCirc)
                .SetLink(gameObject)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}