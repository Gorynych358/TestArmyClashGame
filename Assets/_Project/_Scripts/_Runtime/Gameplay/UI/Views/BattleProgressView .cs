using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ACT.Scripts
{
    public sealed class BattleProgressView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _progressBarFill;
        [SerializeField] private TextMeshProUGUI _defendersText;
        [SerializeField] private TextMeshProUGUI _invadersText;

        public void Show()
        {
            _root.gameObject.SetActive(true);
            _root.localScale = Vector3.zero;
            _root.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        }
        public void HideInstant()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetCounts(int defendersAlive, int invadersAlive)
        {
            _defendersText.text = defendersAlive.ToString();
            _invadersText.text = invadersAlive.ToString();
        }
    }
}
