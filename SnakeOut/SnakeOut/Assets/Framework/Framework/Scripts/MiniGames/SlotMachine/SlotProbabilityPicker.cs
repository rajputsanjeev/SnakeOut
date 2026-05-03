using System.Collections.Generic;
using UnityEngine;

public static class SlotProbabilityPicker
{
	public static int PickIndex<T>(List<T> list, System.Func<T, float> weight)
	{
		float total = 0;
		foreach (var i in list) total += weight(i);

		float rand = Random.Range(0, total);
		float cur = 0;

		for (int i = 0; i < list.Count; i++)
		{
			cur += weight(list[i]);
			if (rand <= cur)
				return i;
		}

		return list.Count - 1;
	}
}
