using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ACT.Runtime.MainMenu.Views
{
    public sealed class PlayButtonView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _button;

        public void Show()
        {
            _canvasGroup = _root.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _root.gameObject.SetActive(true);
            _root.localScale = new Vector2(0.3f, 0.3f);
            _root.DOScale(1f, 0.4f)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
            _canvasGroup.DOFade(1f, 0.4f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);
        }
        public void HideInstant()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetInteractable(bool value)
        {
            _button.interactable = value;
        }

        public void BindClick(Action onClick)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClick?.Invoke());
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}
