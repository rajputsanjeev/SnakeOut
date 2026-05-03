/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace NiobiumStudios
{
    /**
    * The class representation of the Reward
    **/
    [Serializable]
    public class Reward
    {
        public string unit;
        public int reward;
        public Sprite sprite;
    }

	[Serializable]
	public class DayRewards
	{
		public List<RewardItem> rewards = new List<RewardItem>();
	}
}