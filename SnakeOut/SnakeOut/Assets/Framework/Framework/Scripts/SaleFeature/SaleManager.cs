using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
	public class SaleManager
	{
		private const string FILE_NAME = "sale_data.json";
		public static SaleSaveData Data { get; private set; }

		public static async Task Initialize(PackManager packManager)
		{
			// Load or create default
			Data = await SaveSystem.LoadOrCreateAsync(FILE_NAME, new SaleSaveData());

			int currentMonth = packManager.ActivePack != null
				? packManager.ActivePack.startMonth
				: System.DateTime.UtcNow.Month;

			// Reset if new MonthWise theme started
			if (Data.lastActiveMonth != currentMonth)
			{
				Data.Reset();
				Data.lastActiveMonth = currentMonth;
				await SaveSystem.SaveAsync(Data, FILE_NAME, true);
			}
		}

		public static async Task MarkPackPurchased(string packId)
		{
			Data.lastPurchasedPackId = packId;
			await SaveSystem.SaveAsync(Data, FILE_NAME, true);
		}

		/// <summary>
		/// Returns true if user already purchased this pack this month.
		/// </summary>
		public static bool IsPackPurchased(string packId)
		{
			return Data.lastPurchasedPackId == packId;
		}
	}
}
