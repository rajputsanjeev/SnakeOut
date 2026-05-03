using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Components
{
    public class UITicTacView : View
    {
		public RectTransform gridRoot;
		public GridLayoutGroup gridLayout;
		public RectTransform ChestContainer;
		public Button ClaimButton;
		public Button PlayButton;
		public TextMeshProUGUI QuestionText;
	}
}