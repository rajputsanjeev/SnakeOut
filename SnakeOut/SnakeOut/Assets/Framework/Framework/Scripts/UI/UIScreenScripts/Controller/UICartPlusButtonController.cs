using System;
using BaseView;
using Framework.UI.Components;
using ManuGames.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Controllers
{
	public class UICartPlusButtonController : Behaviour<UICartPlusButtonView>
	{
		private UICartPlusButtonView m_View;

		protected override void Init()
		{
			base.Init();
			m_View = (UICartPlusButtonView)Prefab;
			m_View.PlusButton = GetComponent<Button>();
			m_View.PlusButton.onClick.AddListener(PlusButtonPressed);
		}

		private void PlusButtonPressed()
		{
			UIPanelManager.Show(Base.UI.Manager.Panel.CART_SCREEN);
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