using UnityEngine;

namespace Framework
{
	public class LogToggle : MonoBehaviour
	{
		[Header("Logging Control")]
		[Tooltip("Disable all Unity Debug logs (Log, Warning, Error)")]
		public bool disableLogs = true;

		private void Awake()
		{
			ApplyLogSetting();
		}

		private void ApplyLogSetting()
		{
			Debug.unityLogger.logEnabled = !disableLogs;
		}
	}
}
