using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ACT.Runtime.Gameplay.UI.Views
{
    // ===========================
    // VIEW
    // ===========================
    public sealed class ChangeFormationView : MonoBehaviour
    {
        [Header("Change fromation button")]
        [SerializeField] private Button _updateButton;
        [Header("UI references")]
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;
        [Header("Animation settings")]
        [SerializeField] private float _showDelayTime = 0.3f;
        [SerializeField] private float _showDuration = 0.25f;
        [SerializeField] private float _hideDuration = 0.25f;

        private Vector2 _shownPos;
        private Vector2 _hiddenPos;
        private bool _isShown = false;

        private void Awake()
        {
            _shownPos = _root.anchoredPosition;
            float bottom = _canvasRoot.rect.yMin;
            // Смещаем кнопку ещё ниже, чтобы гарантированно была скрыта:
            _hiddenPos = new Vector2(_shownPos.x, bottom - _root.rect.height * 1.2f);
            _root.anchoredPosition = _hiddenPos;
            this.gameObject.SetActive(false);
        }

        public void BindClick(System.Action callback)
        {
            _updateButton.onClick.RemoveAllListeners();
            _updateButton.onClick.AddListener(() => callback());
        }

        public void Show()
        {
            if(_isShown)
                return;
            
            _isShown = true;
            
            this.gameObject.SetActive(true);

            _canvasGroup.alpha = 0f;

            _canvasGroup.DOFade(1f, _showDuration)
                    .SetDelay(_showDelayTime)
                    .SetLink(this.gameObject);
            _root.DOAnchorPosY(_shownPos.y, _showDuration)
                    .SetEase(Ease.OutBack)
                    .SetDelay(_showDelayTime)
                    .SetLink(this.gameObject);
        }

        public void HideInstant()
        {
            _isShown = false;
            this.gameObject.SetActive(false);
        }

        public void SetInteractable(bool value)
        {
            _updateButton.interactable = value;
        }

        public void HideAnimated()
        {
            _isShown = false;
            _canvasGroup
                .DOFade(0f, _hideDuration)
                .SetLink(this.gameObject)
                .OnComplete(() =>
                {
                    this.gameObject.SetActive(false);
                });
        }

        private void OnDisable()
        {
            _isShown = false;
            DOTween.Kill(this.gameObject);
        }
    }
}