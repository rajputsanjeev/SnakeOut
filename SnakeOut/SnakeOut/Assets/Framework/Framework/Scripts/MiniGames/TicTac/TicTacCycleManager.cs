using System;
using System.Collections.Generic;

namespace Framework
{
	public class TicTacCycleManager : DailyCycleManager<TicTacSaveData>
	{
		public TicTacSaveData SaveData => save;

		#region Save Binding
		public override void Load()
		{
			save = TicTacPersistence.Load();
			base.Load();
		}

		public override void Save()
		{
			TicTacPersistence.Save(save);
		}

		public void Save(TicTacSaveData ticTacSaveData)
		{
			TicTacPersistence.Save(ticTacSaveData);
			save = ticTacSaveData;
		}
		#endregion

		#region Feature Logic
		protected override void DoDailyReset()
		{
			base.DoDailyReset();
		}

		void ResetTicTac()
		{
			save.CurrentChest = 0;
			save.IsComplete = false;
			save.CurrentChestCollected = new bool[6];
			save.Cells = new CellRevealData[0];
			save.TicTacFruitRevels = new List<TicTacFruitRevelData>();
		}
		#endregion

		#region Public API (Controller uses this)
		public void MarkCompleted()
		{
			save.CurrentChestCollected[save.CurrentChest] = true;
			save.IsComplete = true;
			save.cooldownEndUtc = Now.AddHours(24).ToString("o");
			Save();
			Notify();
		}

		protected override void OnDailyDataReset()
		{
			ResetTicTac();
		}
		#endregion
	}
}
