using System;
using Framework;
using UnityEngine;

namespace Framework
{
	public interface IManager
	{
		bool IsClaimBtnOn { get; }
		void OnInitialized();
		MainMenuButtons MainMenuButton { get; }
		void CheckScreenReady();

		bool IsUnlocked();

		void SetUnlocked();

		void OpenPanelLevelReach(Action action = null);
	}
}

