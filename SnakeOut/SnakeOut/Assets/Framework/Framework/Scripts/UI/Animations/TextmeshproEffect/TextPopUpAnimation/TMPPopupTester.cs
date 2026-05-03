using UnityEngine;
using Framework;

namespace Framework
{
	public class TMPPopupTester : MonoBehaviour
	{
		[Header("Popup Test")]
		public string TestText = "-250";
		public TMPPopupSettings Settings;

		[Tooltip("Optional override position")]
		public RectTransform Target;

		[HideInInspector] public bool PlayPopup;
	}
}

