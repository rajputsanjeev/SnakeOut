using ManuGames.UI.Components;
using UnityEngine;
using BaseView;
using System;
using Base.UI.Components;
using Framework;

namespace Framework.UI.Controllers
{
	public class UICartController : Behaviour<UICartView>
	{
		public static Action OnCrossBtnClick { set; get; }
		private UICartView m_View;
		private UIPanelComponent _uIPanelComponent { get; set; }

		protected override void Init()
		{
			base.Init();
			m_View = (UICartView)Prefab;
			_uIPanelComponent = GetComponent<UIPanelComponent>();
			m_View.CrossBtn.onClick.AddListener(OnCartCrossBtnClick);
			m_View.BackgroundBtn.onClick.AddListener(OnCartCrossBtnClick);
			//IAPController.OnPurchaseProcced += OnPurchaseProcced;
		}

		private void OnCartCrossBtnClick()
		{
			OnCrossBtnClick?.Invoke();
			_uIPanelComponent.Hide();
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