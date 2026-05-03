using System.Collections.Generic;
using Framework.UI.Controllers;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardSet", menuName = "Data/SlotMachine/RewardSet")]
public class SlotRewardSet : ScriptableObject
{
	public List<SlotMachineReward> Rewards;
}
