using Framework.UI.Components;
using UnityEngine;
using BaseView;
using System;

namespace Framework.UI.Controllers
{
	public class UIPrivilegePurchaseScreenController : Behaviour<UIPrivilegePurchaseScreenView>
	{
		public static Action OnPrivilegePurchase;
		private UIPrivilegePurchaseScreenView m_View;
		private IAPCustomHolder iAPCustomHolder;

		protected override void Init()
		{
			base.Init();
			m_View = (UIPrivilegePurchaseScreenView)Prefab;
			iAPCustomHolder = GetComponent<IAPCustomHolder>();
			iAPCustomHolder.OnPurchaseComplete += OnPrivilagePurchase;
		}


		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
		}

		public override void BackMainMenu()
		{
			base.BackMainMenu();
			UIPanelManager.Show(Base.UI.Manager.Panel.PRIVILAGE, true);
		}

		private void OnPrivilagePurchase()
		{
			OnPrivilegePurchase?.Invoke();
			UIPanelManager.Show(Base.UI.Manager.Panel.PRIVILAGE, true);
		}

		public void OnDestroy()
		{
			iAPCustomHolder.OnPurchaseComplete -= OnPrivilagePurchase;
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}