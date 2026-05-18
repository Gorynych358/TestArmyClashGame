using System;
using ACT.Runtime.Infrastructure;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.MainMenu.Views;
using UnityEngine;

namespace ACT.Runtime.MainMenu.Presenters
{
    public sealed class SelectArmyPowerPresenter
	{
		private readonly ArmyPowerSettingsSO _settings;
		private SelectArmyPowerView _view;

		private const int WARNING_THRESHOLD = 30000;

		public SelectArmyPowerPresenter(ArmyPowerSettingsSO settings)
		{
			_settings = settings;
		}

		public void BindView(SelectArmyPowerView view)
		{
			_view = view;

			// Настройка слайдера
			_view.SetSliderRange(_settings.MinPower, _settings.MaxPower, _settings.ArmyPower);
			_view.SetPowerValue(_settings.ArmyPower);

			// Эталонный MVP: Presenter передаёт callback, View сама подписывается на UI
			_view.BindPowerSlider(OnPowerChanged);

			_view.Show();
		}

		private void OnPowerChanged(float value)
		{
			int power = Mathf.RoundToInt(value);

			_settings.ArmyPower = power;
			_view.SetPowerValue(power);

			if (power > WARNING_THRESHOLD)
				_view.ShowWarning();
			else
				_view.HideWarning();
		}
	}
}
