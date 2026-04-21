using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ACT.Scripts
{
    public sealed class BattleProgressView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _progressBarFill;
        [SerializeField] private TextMeshProUGUI _defendersAmountText;
        [SerializeField] private TextMeshProUGUI _invadersAmountText;
        [SerializeField] private float _showDelayTime = 0f;

        public void Show()
        {
            _root.gameObject.SetActive(true);
            _root.localScale = new Vector2(0.3f, 0.3f);
            _root.DOScale(1f, 0.3f)
                .SetDelay(_showDelayTime)
                .SetEase(Ease.OutBack);
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
    }
}
