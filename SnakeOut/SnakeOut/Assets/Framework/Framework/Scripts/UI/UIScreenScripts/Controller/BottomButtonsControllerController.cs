using System;
using Base.UI.Manager;
using BaseView;
using Framework.UI.Components;

namespace Framework.UI.Controller
{
	public class BottomButtonsControllerController : Behaviour<BottomButtonsControllerBottomButtonsView>
	{
		private BottomButtonsControllerBottomButtonsView m_View;

		protected override void Init()
		{
			base.Init();
			m_View = (BottomButtonsControllerBottomButtonsView)Prefab;
			m_View.CartToggle.onValueChanged.AddListener(OnCartToggleClicked);
			m_View.HomeToggle.onValueChanged.AddListener(OnHomeToggleClicked);
			m_View.AdsToggle.onValueChanged.AddListener(OnAdsToggleClicked);
			//UICartController.OnCrossBtnClick += OnCrossBtnClick;
		}

		private void OnCrossBtnClick()
		{
			m_View.HomeToggle.onValueChanged?.Invoke(true);
			m_View.HomeToggle.isOn = true;
		}

		public void OnCartToggleClicked(bool isOn)
		{
			UIPanelManager.Show(Panel.CART_SCREEN);
		}

		public void OnHomeToggleClicked(bool isOn)
		{
			UIPanelManager.TurnOffAll();
		}

		public void OnAdsToggleClicked(bool isOn)
		{
			UIPanelManager.Show(Panel.REMOVE_AD_SCREEN);
		}

		public override void ShowPanel(bool on)
		{
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}