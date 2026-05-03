using UnityEngine;
using System.Collections.Generic;
using Framework;
using System;



namespace Framework
{
	[CreateAssetMenu(menuName = "TicTac/Mode")]
	public class TicTacModeSO : ScriptableObject
	{
		public int modeId;

		public float GridX;
		public float GridY;

		[Header("Grid")]
		public int width = 3;
		public int height = 3;

		[Header("Reveal Rules")]
		public List<Requiredreveal> requiredreveals;

		[Header("Timer (hours, 0 = no timer)")]
		public float timeLimitHours = 0;

		[Header("AI / Blockers")]
		public bool enableAI;
		public int blockerCount;
	}

	[Serializable]
	public class Requiredreveal
	{
		public int WinCount;
		public int requiredReveals = 1;
		public Sprite possibleFruit;
	}
}