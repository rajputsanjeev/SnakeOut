using UnityEngine;
using UnityEngine.UI;

namespace UI.Extension
{
	public class ToggleExtended : MonoBehaviour
	{
		[SerializeField] private Toggle toggle;
		[SerializeField] private GameObject checkmarkObject;
		[SerializeField] private GameObject NonCheckmarkObject;

		private void Awake()
		{
			if (toggle == null)
				toggle = GetComponent<Toggle>();

			// Add listener to toggle state change
			toggle.onValueChanged.AddListener(OnToggleChanged);

			// Update UI once on start
			OnToggleChanged(toggle.isOn);
		}

		private void OnToggleChanged(bool isOn)
		{
			if (checkmarkObject != null)
				checkmarkObject.SetActive(isOn);

			if(NonCheckmarkObject != null)
				NonCheckmarkObject.SetActive(!isOn);
		}
	}
}