using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ACT.Scripts
{
    public sealed class FightButtonView : MonoBehaviour
    {
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _button;
        [SerializeField] private float _showDelayTime = 0.3f;

        private Vector2 _shownPos;
        private Vector2 _hiddenPos;

        private void Awake()
        {
            _shownPos = _root.anchoredPosition;
            float bottom = _canvasRoot.rect.yMin;
            // Смещаем кнопку ещё ниже, чтобы она точно скрылась
            _hiddenPos = new Vector2(_shownPos.x, bottom - _root.rect.height * 1.2f);
            _root.anchoredPosition = _hiddenPos;
        }

        public void Show()
        {
            
            _root.gameObject.SetActive(true);
            _root.localScale = Vector3.zero;
            _root.DOScale(1f, 0.25f)
                    .SetDelay(_showDelayTime)
                    .SetEase(Ease.OutBack);
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

        public void HideWithSlide()
        {
            _root.DOAnchorPos(_hiddenPos, 0.35f)
                .SetEase(Ease.InBack);
        }

        public void ShowWithSlide()
        {
            _root.gameObject.SetActive(true);
            _root.DOAnchorPos(_shownPos, 0.35f)
                .SetEase(Ease.OutBack);
        }

        private void OnDisable()
        {
            _root.DOKill();
        }
    }
}
