using ManuGames.UI.Components;
using UnityEngine;
using BaseView;
using Framework;
using Framework.UI.Components;
using System;

namespace Framework.UI.Controllers
{
	public class UISpinWheelController : Behaviour<UISpinWheelView>
	{
		private UISpinWheelView m_View;
		private FortuneWheelManager _manager;

		protected override void Init()
		{
			base.Init();
			m_View = (UISpinWheelView)Prefab;
			_manager = GetComponent<FortuneWheelManager>();
		}

		private void Start()
		{
			_manager.OnRefreshWheel += RefreshClaimText;
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
		}

		private void RefreshClaimText()
		{
			if (_manager.MainMenuButton != null)
			{
				var isClaimable = _manager.GetRemainReward() > 0;
				_manager.MainMenuButton.RefreshCircle(isClaimable, _manager.GetRemainReward().ToString());
			}
		}

		private void OnDestroy()
		{
			_manager.OnRefreshWheel -= RefreshClaimText;
		}

		public override bool IsShow()
		{
			return (_manager.MainMenuButton.IsLevelReach() && IsOfferScreen);
		}
	}
}