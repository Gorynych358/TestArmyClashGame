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

        private Vector2 _hiddenPos;
        private Vector2 _shownPos;

        private void Awake()
        {
            float screenHeight = _canvasRoot.rect.height;

            _shownPos = _root.anchoredPosition;
            _hiddenPos = new Vector2(_shownPos.x, screenHeight + _root.rect.height * 1.2f);

            _root.anchoredPosition = _hiddenPos;
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void HideInstant()
        {
            gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;
            _root.anchoredPosition = _hiddenPos;
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
            _root.anchoredPosition = _hiddenPos;

            _canvasGroup
                .DOFade(1f, 0.25f)
                .SetDelay(_showDelayTime)
                .SetEase(Ease.OutCubic)
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
                .DOFade(0f, 0.2f)
                .SetEase(Ease.InCubic)
                .SetLink(gameObject);

            _root
                .DOAnchorPos(_hiddenPos, _animDuration)
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