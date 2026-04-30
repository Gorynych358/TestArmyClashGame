using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ACT.Scripts
{
    public sealed class BattleProgressView : MonoBehaviour
    {
        [SerializeField] private RectTransform _canvasRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _progressBarFill;
        [SerializeField] private TextMeshProUGUI _defendersAmountText;
        [SerializeField] private TextMeshProUGUI _invadersAmountText;
        [SerializeField] private float _showDelayTime = 0f;
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
            _root.DOAnchorPos(_shownPos, SHOW_TIME)
                    .SetDelay(_showDelayTime)
                    .SetEase(Ease.OutBack)
                    .SetLink(gameObject);
        }
        public void HideInstant()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetCounts(int defendersAlive, int invadersAlive)
        {
            _defendersAmountText.text = defendersAlive.ToString();
            _invadersAmountText.text = invadersAlive.ToString();
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}
