using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Components
{
    public class UIRemoveAdsView : View
    {
		public Button[] RemoveAdsBtns;
		public RectTransform[] LockImages;
		public GameObject LockImage;
		public TextMeshProUGUI AdsTimerText;
		public Toggle HomeToggle;
	}
}