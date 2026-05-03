using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;
using Base.UI.Manager;
using TMPro;
using System.Collections.Generic;

namespace Framework
{
	public class FortuneWheelManager : MonoBehaviour
	{
		public Action OnRefreshWheel;

		// Key name for storing in PlayerPrefs
		private const string LAST_FREE_TURN_TIME_NAME = "LastFreeTurnTimeTicks";
		private const string FIRST_TIME = "FirsTime";
		private const string FREE_SPIN = "FreeSpin";
		private const string FIRS_TIME = "FirsTime";
		private const string AD_WATCH_COUNT = "AdWatchCount";

		public bool UseUtc = true;
		public bool UseOnline;
		protected DateTime Now =>
		UseUtc
			? (UseOnline ? UtcTimeService.Instance.GetCurrentDate() : DateTime.UtcNow)
			: DateTime.Now;

		public string GetLastFortuneTime()
		{
			if (!PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME))
			{
				PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, Now.AddHours(TimerMaxHours).AddMinutes(TimerMaxMinutes).AddSeconds(TimerMaxSeconds).ToString("o"));

			}
			return PlayerPrefs.GetString(LAST_FREE_TURN_TIME_NAME);
		}

		public int GetSpinWheelWatchAdsCount
		{
			get => PlayerPrefs.GetInt(AD_WATCH_COUNT);
			set
			{
				if (GetSpinWheelWatchAdsCount != value)
				{
					PlayerPrefs.SetInt(AD_WATCH_COUNT, value);
					PlayerPrefs.Save();
				}
			}
		}

		public int GetRemainReward()
		{
			Debug.Log("GetSpinWheelWatchAdsCount " + GetSpinWheelWatchAdsCount);
			Debug.Log("_freeSpin " + _freeSpin);
			var ads = (SpinWheelWatchAddCount - GetSpinWheelWatchAdsCount) + _freeSpin;
			return ads;
		}

		// Set TRUE if you want to let players to turn the wheel for FREE from time to time
		[Header("Can players turn the wheel for FREE from time to time?")]
		public bool IsFreeTurnEnabled = true;
		public int FreeSpinWheelCount;
		public int SpinWheelWatchAddCount;

		[Header("Assgine Button and text from main screen")]
		public bool IsTurnOn;                          // Button will remain interactable if all free and ads count at zero.
		public Button SpinButtonOnMainScreen;
		public TextMeshProUGUI TimeTextOnMainScreen; // Text element that contains remaining time to next free turn
		public TextMeshProUGUI TimeTextOnWheelScreen;
		public string Message;

		[Header("Calibration Particle effect")]
		public GameObject FireWork;

		[Header("Game Objects for some elements")]
		public GameObject Circle;                   // Rotatable GameObject on scene with reward objects
		public Button SpinButtonOnWheelScreen;
		public Button WatchAdButton;
		public Button CrossButton;

		// Here you can set time between two free turns
		[Header("Time Between Two Free Turns")]
		public int TimerMaxHours;
		[RangeAttribute(0, 59)]
		public int TimerMaxMinutes;
		[RangeAttribute(0, 59)]
		public int TimerMaxSeconds = 10;

		public RequiredLevelScriptableObject RequiredLevelScriptableObject;
		public MainMenuButtons MainMenuButton;

		public List<DailyRewardSet> RewardSets = new List<DailyRewardSet>();
		private RuntimeSpinWheel _runtimeSpinWheel;

		// Flag that player can turn the wheel for free right now
		private bool _isFreeTurnAvailable;
		private int _freeSpin = 0;

		private void Awake()
		{
			_runtimeSpinWheel = GetComponent<RuntimeSpinWheel>();

			MainMenuButton.SetRequiredLevel(RequiredLevelScriptableObject.ShowAfterLevel, RequiredLevelScriptableObject.IsShowWhenLevelNotReach, true, OnClickMainScreenSpinButton);

			var firstTime = PlayerPrefs.GetInt(FIRST_TIME, 0);
			if (firstTime == 0)
			{
				PlayerPrefs.SetInt(FREE_SPIN, FreeSpinWheelCount);
				PlayerPrefs.SetInt(FIRS_TIME, 1);
			}
			_freeSpin = PlayerPrefs.GetInt(FREE_SPIN, FreeSpinWheelCount);

			SpinButtonOnWheelScreen.onClick.AddListener(() => TurnWheelButtonClick());

			_runtimeSpinWheel.SetData(RewardSets[0].GetRewardSprite(), RewardSets[0].GetRewardText());
			_runtimeSpinWheel.GenerateWheel();
			_runtimeSpinWheel.OnSpinComplete += OnSpinComplete;

			if (MainMenuButton.IsLevelReach()) CheckFreeTurns();
		}

		private void OnSpinComplete(int index, string arg2)
		{
			RewardBaseHandle.Instance.SetReward(RewardSets[0].steps[index].items);
			if (FireWork != null)
			{
				FireWork?.SetActive(true);
			}
			StartCoroutine(HideCoinsDelta());
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		private void OnEnable()
		{
			if (FireWork != null)
			{
				FireWork?.SetActive(false);
			}
		}

		private void CheckFreeTurns()
		{
			if (!IsFreeTurnEnabled)
				return;

			// Default UI state (everything off)
			WatchAdButton.gameObject.SetActive(false);
			SpinButtonOnWheelScreen.gameObject.SetActive(false);

			if (_freeSpin > 0)
			{
				// Case 1: Player has free spins
				SpinButtonOnWheelScreen.gameObject.SetActive(true);
			}
			else if (GetSpinWheelWatchAdsCount < SpinWheelWatchAddCount)
			{
				// Case 2: No free spin, ads still allowed
				WatchAdButton.gameObject.SetActive(true);
			}
			else
			{
				// Case 3: Ads exhausted => reset timer
				if (!IsTurnOn) SpinButtonOnMainScreen.interactable = false;
				if (!IsTimerRunning())
				{
					GetLastFortuneTime();
					SetNextFreeTime();
					StartWheelTimer();
				}
				else if (PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME) && _freeSpin == 0)
				{
					SetNextFreeTime();
					StartWheelTimer();
				}
			}

			Invoke(nameof(OnWheelAwake), 1);
		}

		private void StartWheelTimer()
		{
			TimeSpan remaining = GetRemainingTime();

			if (remaining == TimeSpan.Zero)
			{
				OnTimerCompleted();
				return;
			}

			TimerManager.CreateTimer(
				remaining,
				OnTimerCompleted,
				OnTimerTick,
				"Fortune_Wheel"
			);
		}

		private void OnTimerCompleted()
		{
			PlayerPrefs.DeleteKey(LAST_FREE_TURN_TIME_NAME);
			OnRefreshWheel?.Invoke();

			_isFreeTurnAvailable = true;
			GetSpinWheelWatchAdsCount = 0;

			_freeSpin += 1;
			PlayerPrefs.SetInt(FREE_SPIN, _freeSpin);

			if (TimeTextOnMainScreen != null)
				TimeTextOnMainScreen.text = "Spin";

			if (TimeTextOnWheelScreen != null)
				TimeTextOnWheelScreen.text = "";

			SpinButtonOnMainScreen.interactable = true;
			SpinButtonOnWheelScreen.interactable = true;

			WatchAdButton.gameObject.SetActive(false);
			SpinButtonOnWheelScreen.gameObject.SetActive(true);
		}

		private void OnTimerTick(TimeSpan x)
		{
			var str = GetRemainReward() > 1 ? "Claim before \n" : "Come Back after \n ";

			string t = string.Format("{0:00}:{1:00}:{2:00}", x.Hours, x.Minutes, x.Seconds);

			if (TimeTextOnMainScreen != null)
				TimeTextOnMainScreen.text = str + t;

			if (TimeTextOnWheelScreen != null)
				TimeTextOnWheelScreen.text = str + t;
		}

		private bool IsTimerRunning()
		{
			return PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME);
		}

		private void OnClickMainScreenSpinButton()
		{
			CrossButton.interactable = true;
			UIPanelManager.Instance.Show(Panel.SPIN_WHEEL);
		}

		private void TurnWheelButtonClick()
		{
			if (_isFreeTurnAvailable || _freeSpin > 0)
			{
				SpinButtonOnWheelScreen.interactable = false;
				CrossButton.interactable = false;
				_runtimeSpinWheel.StartSpin();
			}
		}

		private void SetNextFreeTime()
		{
			_isFreeTurnAvailable = false;
		}

		/// <summary>
		/// Sample callback for giving reward (in editor each sector have Reward Callback field pointed to this method)
		/// </summary>
		/// <param name="awardCoins">Coins for user</param>
		// Hide coins delta text after animation
		private IEnumerator HideCoinsDelta()
		{
			yield return new WaitForSeconds(0.5f);

			if (FireWork != null)
			{
				FireWork?.SetActive(false);
			}

			// Default UI state
			WatchAdButton.gameObject.SetActive(false);
			SpinButtonOnWheelScreen.gameObject.SetActive(false);

			if (GetSpinWheelWatchAdsCount < SpinWheelWatchAddCount)
			{
				// Ads available → show Watch Ad button
				WatchAdButton.gameObject.SetActive(true);
			}
			else if (GetSpinWheelWatchAdsCount >= SpinWheelWatchAddCount && !IsTimerRunning())
			{
				// Ads exhausted → save timestamp and reset timer
				if (!IsTurnOn) SpinButtonOnMainScreen.interactable = false;
				GetLastFortuneTime();
				SetNextFreeTime();
				StartWheelTimer();
			}

			// Reduce free spin if available
			if (_freeSpin > 0)
			{
				_freeSpin--;
				PlayerPrefs.SetInt(FREE_SPIN, _freeSpin);
			}
			OnRefreshWheel?.Invoke();
		}

		private TimeSpan GetRemainingTime()
		{
			if (!PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME))
				return TimeSpan.Zero;

			var nextTime = DateTime.Parse(
				PlayerPrefs.GetString(LAST_FREE_TURN_TIME_NAME),
				null,
				System.Globalization.DateTimeStyles.RoundtripKind);

			var remain = nextTime - Now;
			return remain <= TimeSpan.Zero ? TimeSpan.Zero : remain;
		}


		public void OnAdCompleted()
		{
			_freeSpin += 1;
			GetSpinWheelWatchAdsCount++;
			PlayerPrefs.SetInt(FREE_SPIN, _freeSpin);
			WatchAdButton.gameObject.SetActive(false);
			SpinButtonOnWheelScreen.gameObject.SetActive(true);
			SpinButtonOnWheelScreen.interactable = true;
		}

		private void OnWheelAwake()
		{
			OnRefreshWheel?.Invoke();
		}

		public void OnDestroy()
		{
			_runtimeSpinWheel.OnSpinComplete -= OnSpinComplete;
		}
	}

	[Serializable]
	public class FortuneWheelSector : System.Object
	{
		[Tooltip("Text object where value will be placed (not required)")]
		public GameObject ValueTextObject;

		[Tooltip("Value of reward")]
		public string RewardValue = "100";

		[Tooltip("Chance that this sector will be randomly selected")]
		[RangeAttribute(0, 100)]
		public float Probability = 100;

		[Tooltip("Method that will be invoked if this sector will be randomly selected")]
		public UnityEvent RewardCallback;
	}

}