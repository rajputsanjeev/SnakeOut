using Base.UI.Controller;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class UICheckScreenReady : MonoBehaviour
	{
		public Button _claimButton;
		public int _depthSearch;

		private IManager _manager;
		private UIBaseController _baseController;

		public void CheckScreenReady()
		{
			_manager = GetComponent<IManager>();
			_baseController = GetComponent<UIBaseController>();

			var isActive = _claimButton.IsActiveWithParents(_depthSearch);

			if (_manager.MainMenuButton.IsLevelReach() && !_manager.IsUnlocked() || (_claimButton.interactable && _claimButton.IsActiveWithParents(_depthSearch)) || _manager.IsClaimBtnOn)
			{
				_baseController.OpenCurrentPanel();
				_manager.SetUnlocked();
			}
		}
	}
}
