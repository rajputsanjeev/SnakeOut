using Framework.UI.Components;
using UnityEngine;
using BaseView;
using Base.UI.Manager;

namespace Framework.UI.Controllers
{
	public class UISaleController : Behaviour<UISaleView>
	{
		public Panel SeasonPanel;

		private UISaleView m_View;
		private PackManager _manager;

		protected override void Init()
		{
			base.Init();
			m_View = (UISaleView)Prefab;
			_manager = GetComponent<PackManager>();
			_manager.MainMenuButton.AssgineAction(OpenCurrentPanel);
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
		}

		public override bool IsShow()
		{
			if (_manager == null || _manager.ActivePack == null)
			{
				return false;
			}
			return (_manager.MainMenuButton.IsLevelReach() && IsOfferScreen);
		}
	}
}