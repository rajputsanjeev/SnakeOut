using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
	public class MainMenuButtons : MonoBehaviour
	{
		public int PlayerLevel => GetCurrentLevelAbstract.Instance.GetLevel();
		public TextMeshProUGUI Text;
		public Button MainButton;
		public RectTransform ClaimRemainAmountRect;
		public TextMeshProUGUI ClaimRemainAmount;

		private int _requiredLevel;
		private bool _isShowWhenLevelNotReach;
		private RectTransform _currentRect;

		private void Awake()
		{
			if (MainButton == null) MainButton = GetComponent<Button>();
			if (Text == null) Text = GetComponentInChildren<TextMeshProUGUI>();
			_currentRect = GetComponentInParent<RectTransform>();
		}

		public void SetRequiredLevel(int requiredLevel, bool isShowWhenLevelNotReach, bool isConfig = true, UnityAction unityAction = null)
		{
			if (MainButton == null) MainButton = GetComponent<Button>();

			this._requiredLevel = requiredLevel;
			this._isShowWhenLevelNotReach = isShowWhenLevelNotReach;
			var isLevelReached = IsLevelReach();

			if (!_isShowWhenLevelNotReach)
			{
				if (!isLevelReached)
				{
					MainButton.gameObject.SetActive(false);
				}
				else
				{
					MainButton.gameObject.SetActive(isConfig);
				}
			}
			else
			{
				MainButton.gameObject.SetActive(isConfig);
			}
			if (!isLevelReached)
			{
				SetText("Unlock " + _requiredLevel);
				ClaimRemainAmountRect.gameObject.SetActive(false);
				MainButton.onClick.AddListener(ShowToastMessage);
			}
			else if (unityAction != null)
			{
				AssgineAction(unityAction);
			}
		}

		private void ShowToastMessage()
		{
			ToastMessage.Instance.Show("Unlock at " + _requiredLevel);
		}

		public void SetText(string text)
		{
			Text.text = text;
		}

		public void SetButtonInteractable(bool isInteractable)
		{
			MainButton.interactable = isInteractable;
		}

		public bool IsLevelReach()
		{
			var isLevelReach = (PlayerLevel >= _requiredLevel);
			return isLevelReach;
		}

		public void AssgineAction(UnityAction unityAction)
		{
			if (MainButton == null) MainButton = GetComponent<Button>();
			if (MainButton != null)
			{
				MainButton.onClick.AddListener(unityAction);
			}
		}

		public void EnableButton(bool isactive)
		{
			SetButtonInteractable(isactive);
			gameObject.SetActive(isactive);
		}

		public void RefreshCircle(bool isActive, string text)
		{
			if (ClaimRemainAmountRect != null)
			{
				ClaimRemainAmountRect.gameObject.SetActive(isActive);
				if (!string.IsNullOrEmpty(text)) ClaimRemainAmount.SetText(text);
			}
		}
	}
}