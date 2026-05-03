using UnityEngine;
using System.Collections.Generic;
using Framework;

namespace Framework
{
    [CreateAssetMenu(menuName = "TicTac/Chest")]
    public class ChestConfigSO : ScriptableObject
    {
        public Sprite ChestSprite;
        public Sprite CloseChest;
    	public string chestId;
    	public List<TicTacModeSO> modes;
		public List<RewardItem> Reward = new List<RewardItem>();
	}
}