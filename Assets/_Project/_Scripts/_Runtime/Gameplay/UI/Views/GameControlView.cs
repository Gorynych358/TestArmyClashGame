// ============================================================================
//  GameControlView.cs
// ============================================================================

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ACT.Runtime.Gameplay.UI.Views
{
    public sealed class GameControlView : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _pauseButton;

        [Header("Animation settings")]
        [SerializeField] private float _fadeDuration = 0.25f;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _backButton.gameObject.SetActive(true);
            _pauseButton.gameObject.SetActive(false);
        }

        public void HideInstant()
        {
            gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;
        }

        public void SetInteractable(bool value)
        {
            _backButton.interactable = value;
            _pauseButton.interactable = value;
        }

        public void ShowAnimated()
        {
            gameObject.SetActive(true);

            _canvasGroup.alpha = 0f;
            _canvasGroup
                .DOFade(1f, _fadeDuration)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
        }

        public void ShowBackButton()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1;
            _backButton.gameObject.SetActive(true);
            _pauseButton.gameObject.SetActive(false);
        }

        public void ShowPauseButton()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1;
            _backButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(true);
        }

        public void BindBack(Action onClick)
        {
            _backButton.onClick.RemoveAllListeners();
            _backButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void BindPause(Action onClick)
        {
            _pauseButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.AddListener(() => onClick?.Invoke());
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}