using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NotificationConfig", menuName = "Notifications/Config", order = 1)]
public class NotificationConfig : ScriptableObject
{
	public enum Mode
	{
		LocalOnly,
		RemoteOnly,
		Both
	}

	[Header("Global mode")]
	public Mode notificationMode = Mode.Both;

	[Header("Templates")]
	public List<NotificationTemplate> templates = new List<NotificationTemplate>();

	[Header("Android Defaults")]
	public string defaultAndroidChannelId = "default_channel";
	public string defaultAndroidChannelName = "Default";
	public string defaultAndroidChannelDesc = "General notifications";

	[Header("Debug")]
	public bool debugLog = true;
}
