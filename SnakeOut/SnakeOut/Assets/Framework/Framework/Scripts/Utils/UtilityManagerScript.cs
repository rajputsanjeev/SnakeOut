using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{

}
public class UtilityManagerScript : MonoBehaviour
{
	public static UtilityManagerScript Instance;
	public static bool isRemoveAds = false;
	public static bool isClaim = false;
	public static bool IS_SEVEN_LEVEL_COMPLETE = false;
	public Sprite[] sprites;
	public Action<int> UpdateCoin { get; set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	public void SetCoin(int coin)
	{
		PlayerPrefs.SetInt("player_coin", coin);
		UpdateCoin?.Invoke(coin);
	}

	public int GetCoin()
	{
		return PlayerPrefs.GetInt("player_coin", 300);
	}

	public void SetSpeedPower(int speed)
	{
		PlayerPrefs.SetInt("player_Speed", speed);
	}

	public int GetSpeedPower()
	{
		return PlayerPrefs.GetInt("player_Speed", 3);
	}
	public void SetTaskPower(int task)
	{
		PlayerPrefs.SetInt("player_Task", task);
	}

	public int GetTaskPower()
	{
		return PlayerPrefs.GetInt("player_Task", 3);
	}

	public void SetEmptyPower(int empty)
	{
		PlayerPrefs.SetInt("player_empty", empty);
	}

	public int GetEmptyPower()
	{
		return PlayerPrefs.GetInt("player_empty", 3);
	}

	public int GetPiggyCoins()
	{
		return PlayerPrefs.GetInt("piggy_coins", 0);
	}

	public void SetPiggyCoins(int coins)
	{
		PlayerPrefs.SetInt("piggy_coins", coins);
	}

	public int GetTheme()
	{
		return PlayerPrefs.GetInt("gameTheme", 0);
	}

	public void SetTheme(int theme)
	{
		PlayerPrefs.SetInt("gameTheme", theme);
	}

	public int GetJoyStick()
	{
		return PlayerPrefs.GetInt("gameJoyStick", 0);
	}

	public void SetJoyStick(int theme)
	{
		PlayerPrefs.SetInt("gameJoyStick", theme);
	}

	public int GetReward()
	{
		return PlayerPrefs.GetInt("reward", 0);
	}

	public void SetReward(int reward)
	{
		PlayerPrefs.SetInt("reward", reward);
	}

	public int IsNotFirstTime
	{
		get => PlayerPrefs.GetInt("Is-First-Time", 0);
		set
		{
			if (IsNotFirstTime != value)
			{
				PlayerPrefs.SetInt("Is-First-Time", value);
				PlayerPrefs.Save();
			}
		}
	}

	public bool IsNotRewardedFirstTime
	{
		get
		{
			return PlayerPrefs.GetInt("Is-First-Rewarded-Time", 0) == 0 ? false : true; ;
		}
		set
		{
			if (IsNotRewardedFirstTime != value)
			{
				PlayerPrefs.SetInt("Is-First-Rewarded-Time", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}
	}

	public string LastTimeReward
	{
		get => PlayerPrefs.GetString("LastClaimTime");
		set
		{
			if (LastTimeReward != value)
			{
				PlayerPrefs.SetString("LastClaimTime", value);
				PlayerPrefs.Save();
			}
		}
	}

	public string NoCoinAdLimit
	{
		get => PlayerPrefs.GetString("NoCoinAdLimit");
		set
		{
			if (LastTimeReward != value)
			{
				PlayerPrefs.SetString("NoCoinAdLimit", value);
				PlayerPrefs.Save();
			}
		}
	}
}
