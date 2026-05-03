using System;
using Base.UI.Manager;
using UnityEngine.EventSystems;
using Framework;
using Framework.Core;

namespace Watermelon
{
	public class SettingsMenuButton : SettingsButtonBase
	{
		private bool GameStart;

		public override void Init()
		{
			gameObject.SetActive(SceneUtils.DoesSceneExist(GameConsts.SCENE_MENU));
			LevelController.LevelLoaded += OnLevelLoaded;
		}

		private void OnLevelLoaded()
		{
			LevelController.LevelLoaded -= OnLevelLoaded;

			if (LevelController.GameplayTimer != null)
			{
				LevelController.GameplayTimer.OnTimerStart += OnTimerStart;
			}
			if (LevelController.GameplayMove != null)
			{
				LevelController.GameplayMove.OnTimerStart += OnTimerStart;
			}
		}

		private void OnTimerStart()
		{
			GameStart = true;
			if (LevelController.GameplayMove != null)
			{
				LevelController.GameplayMove.OnTimerStart -= OnTimerStart;
			}
			if (LevelController.GameplayTimer != null)
			{
				LevelController.GameplayTimer.OnTimerStart -= OnTimerStart;
			}
		}

		public override void OnClick()
		{
			// Play button sound
			AudioController.PlaySound(AudioController.AudioClips.buttonSound);

			UIPanelManager.Instance.Show(Panel.SETTING_SCREEN, false);

			if (GameStart)
			{
				LivesSystem.LockLife();
				UILevelQuitPopUp.Show((confirmed) =>
				{
					if (confirmed)
					{
						LoadMenuCutLIfe();
					}
					else
					{
						UIPanelManager.Instance.Show(Panel.SETTING_SCREEN, true);
					}
				});
			}
			else
			{
				LoadMenuDontCutLife();
			}
		}

		private void LoadMenuDontCutLife()
		{
			// Unload the current level and all the dependencies
			GameController.LoadMenu();
		}

		private void LoadMenuCutLIfe()
		{
			// Show fullscreen black overlay
			LivesSystem.UnlockLife(true);

			// Save the current state of the game
			SaveController.Save(true);

			// Unload the current level and all the dependencies
			GameController.LoadMenu();
		}

		public override void Select()
		{
			IsSelected = true;

			Button.Select();

			EventSystem.current.SetSelectedGameObject(null); //clear any previous selection (best practice)
			EventSystem.current.SetSelectedGameObject(Button.gameObject, new BaseEventData(EventSystem.current));
		}

		public override void Deselect()
		{
			IsSelected = false;

			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
