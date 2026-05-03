using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[System.Serializable]
	public class PowerUpData : MiniGameCoolDown
	{
		public string powerUpName;
		public int maxUses;
		public int currentUses;
	}

	[System.Serializable]
	public class PowerUpSaveModel
	{
		public List<PowerUpData> allPowerUps = new List<PowerUpData>();
	}

}
