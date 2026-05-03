using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ManuGames.UI.Components
{
    public class UIDailyRewardView : View
    {
		public GameObject panelDebug;
		public Button buttonAdvanceDay;
		public Button buttonAdvanceHour;
		public Button buttonReset;
		public Button buttonReloadScene;

		[Header("Panel Reward")]
		public Button ButtonClaim;                  // Claim Button
		public Button ButtonClose;                  // Close Button
		public TextMeshProUGUI TextTimeDue;
	}
}