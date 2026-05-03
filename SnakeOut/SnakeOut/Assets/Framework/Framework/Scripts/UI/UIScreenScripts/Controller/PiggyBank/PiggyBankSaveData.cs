using UnityEngine;

namespace Framework
{
	public class PiggyBankSaveData
	{
		public int coinsStored = 0;
		public int purchasesCount = 0;
		public string cycleStartUtc = "";  
		public string lastPurchaseUtc = "";
		public string startDateTime = "";
		public string lastOfferEndUtc;
	}
}