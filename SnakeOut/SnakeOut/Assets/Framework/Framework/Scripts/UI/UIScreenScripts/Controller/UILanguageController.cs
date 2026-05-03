using System;
using Base.UI.Manager;
using BaseView;
using I2.Loc;
using ManuGames.UI.Components;
using UnityEngine;

namespace ManuGames.UI.Controllers
{
	public class UILanguageController : Behaviour<UILanguageView>
	{
		private UILanguageView m_View;
		public GameObject[] CheckMark;

		protected override void Init()
		{
			base.Init();
			m_View = (UILanguageView)Prefab;
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
			SetLanguage();
		}

		public void SetLanguage()
		{
			var currentLanguage = LocalizationManager.CurrentLanguage;
			var languages = LocalizationManager.GetAllLanguages();

			int value = languages.IndexOf(currentLanguage);

			for (var i = 0; i < CheckMark.Length; i++)
			{
				if (value == i)
				{
					CheckMark[value].SetActive(true);
				}
				else
				{
					CheckMark[i].SetActive(false);
				}
			}
		}

		public override void BackMainMenu()
		{
			base.BackMainMenu();
			UIPanelManager.Show(Panel.SETTING_SCREEN, true);
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}