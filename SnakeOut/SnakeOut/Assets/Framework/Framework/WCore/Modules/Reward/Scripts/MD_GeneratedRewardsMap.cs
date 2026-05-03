// Auto-generated file. Do not edit.
using System;
using System.Collections.Generic;

namespace Framework.Core
{
    public static class RewardsMap
    {
        public static Dictionary<Type, Type> ViewMap { get; } = GetMap();

        public static Dictionary<Type, Type> GetMap()
        {
            Dictionary<Type, Type> map = new Dictionary<Type, Type>();
            map[typeof(LivesReward)] = typeof(LivesRewardView);
            map[typeof(LivesMaxLivesReward)] = typeof(LivesRewardView);
            map[typeof(Watermelon.PUReward)] = typeof(Watermelon.PURewardView);
            map[typeof(NoAdsReward)] = typeof(NoAdsRewardView);
            map[typeof(CurrencyReward)] = typeof(CurrencyRewardView);
            map[typeof(SkinReward)] = typeof(SkinRewardView);
            map[typeof(LivesInfiniteModeReward)] = typeof(LivesInfiniteModeRewardView);

            return map;
        }
    }
}