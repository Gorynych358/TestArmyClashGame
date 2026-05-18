using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

namespace ACT.Runtime.MainMenu.Views
{
    public sealed class SelectArmyPowerView : MonoBehaviour
    {
        [Header("UI")]
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private RectTransform _root;
		[SerializeField] private Slider _armyPowerSlider;
		[SerializeField] private TextMeshProUGUI _resultPowerTF;
		[SerializeField] private TextMeshProUGUI _warningTF;

		[Header("Animation")]
		[SerializeField] private float _showDuration = 0.45f;
		[SerializeField] private float _hideDuration = 0.25f;

        public Slider Slider => _armyPowerSlider;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _warningTF.alpha = 0;
        }
        
        // -----------------------------
		//  CALLBACK BINDING 
		// -----------------------------

		public void BindPowerSlider(Action<float> callback)
		{
			_armyPowerSlider.onValueChanged.RemoveAllListeners();
			_armyPowerSlider.onValueChanged.AddListener(value => callback(value));
		}

		// -----------------------------
		//  UI UPDATE METHODS
		// -----------------------------

		public void SetSliderRange(int min, int max, int defaultValue)
		{
			_armyPowerSlider.minValue = min;
			_armyPowerSlider.maxValue = max;
			_armyPowerSlider.value = defaultValue;
		}

		public void SetPowerValue(int value)
		{
			_resultPowerTF.text = value.ToString();
		}

		public void ShowWarning()
		{
			_warningTF
				.DOFade(1f, 0.3f)
				.SetEase(Ease.OutCubic)
				.SetLink(gameObject);
		}

		public void HideWarning()
		{
			_warningTF
				.DOFade(0f, 0.3f)
				.SetEase(Ease.OutCubic)
				.SetLink(gameObject);
		}

        // -----------------------------
		//  SHOW / HIDE
		// -----------------------------

        public void Show()
        {
            gameObject.SetActive(true);

            _canvasGroup
                .DOFade(1f, _showDuration)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
        }

        public void Hide()
        {
            _canvasGroup
                .DOFade(0f, _hideDuration)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
        }
    }
}
