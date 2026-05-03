using System;
using Base.UI.Manager;
using BaseView;
using Frameork;
using Framework.Core;
using Framework.UI.Components;
using UnityEngine;
using Watermelon;

namespace Framework
{
	public class UISettingController : Behaviour<UISettingView>
	{
		public Sprite OnSprite; // Assign your Music On sprite in the Inspector
		public Sprite OffSprite; // Assign your Music Off sprite in the Inspector
		public bool IsGamePlayScene;

		private UISettingView m_View;
		private bool _isMusicOn;
		private bool _isSoundOn;
		private bool _isVibrationOn;

		protected override void Init()
		{
			base.Init();
			m_View = (UISettingView)Prefab;

			m_View.MusicToggleButton.onClick.AddListener(ToggleMusic);
			m_View.SoundToggleButton.onClick.AddListener(ToggleSound);
			m_View.VibrationToggleButton.onClick.AddListener(ToggleVibration);
			m_View.LanguageBtn.onClick.AddListener(TurnOnLanguageScreen);
			if (m_View.SettingBtn != null) m_View.SettingBtn.onClick.AddListener(OpenSettingScreen);

			if (m_View.VersionText != null) m_View.VersionText.text = "V " + Application.version;
		}

		public override void ShowPanel(bool on)
		{
			base.ShowPanel(on);
			if (on) CheckStatus();
		}

		private void OpenSettingScreen()
		{
			UIPanelManager.Instance.Show(Panel.SETTING_SCREEN, true);
		}

		public override void ShowAnimation()
		{
			base.ShowAnimation();
		}

		public override void HideAnimation()
		{
			base.HideAnimation();
		}

		private void OnEnable()
		{
			CheckStatus();
			AudioController.VolumeChanged += OnVolumeChanged;
			Haptic.StateChanged += OnStateChanged;
		}

		private void OnDisable()
		{
			AudioController.VolumeChanged -= OnVolumeChanged;
			Haptic.StateChanged -= OnStateChanged;
		}

		#region Get and Set State
		private void CheckStatus()
		{
			_isMusicOn = GetState(Framework.Core.AudioType.Music); // Load saved state
			_isSoundOn = GetState(Framework.Core.AudioType.Sound); // Load saved state
			_isVibrationOn = Haptic.IsActive;

			UpdateButtonSprite();
		}

		private bool GetState(Framework.Core.AudioType type)
		{
			return AudioController.IsAudioTypeActive(type);
		}

		private void SetState(bool state, Framework.Core.AudioType type)
		{
			var volume = state ? 1.0f : 0.0f;

			AudioController.SetVolume(type, volume);
		}
		#endregion

		#region Toggles
		public void ToggleMusic()
		{
			_isMusicOn = !_isMusicOn;
			SetState(_isMusicOn, Framework.Core.AudioType.Music);
		}

		public void ToggleSound()
		{
			_isSoundOn = !_isSoundOn;
			SetState(_isSoundOn, Framework.Core.AudioType.Sound);
		}

		public void ToggleVibration()
		{
			Haptic.IsActive = !_isVibrationOn;
			UpdateButtonSprite();
		}
		#endregion

		private void TurnOnLanguageScreen()
		{
			UIPanelManager.Show(Panel.LANGUAGE_SCREEN, true);
		}

		private void OnStateChanged(bool value)
		{
			_isVibrationOn = value;
			CheckStatus();
		}

		private void OnVolumeChanged(Framework.Core.AudioType audioType, float volume)
		{
			CheckStatus();
		}

		private void UpdateButtonSprite()
		{
			m_View.MusicToggleButton.image.sprite = _isMusicOn ? OnSprite : OffSprite; // Update button image
			m_View.SoundToggleButton.image.sprite = _isSoundOn ? OnSprite : OffSprite; // Update button image
			m_View.VibrationToggleButton.image.sprite = _isVibrationOn ? OnSprite : OffSprite; // Update button image
		}

		public override void BackMainMenu()
		{
			if (IsGamePlayScene)
			{
				MyEventArgs.GameControllerEvents.OnSettingBack.Dispatch();
			}
			base.BackMainMenu();
		}

		public override bool IsShow()
		{
			return false;
		}
	}
}