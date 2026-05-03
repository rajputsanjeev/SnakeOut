using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NotificationTemplate", menuName = "Notifications/Template", order = 0)]
public class NotificationTemplate : ScriptableObject
{
	[Header("Identity")]
	public string id; // unique id, used as method name when generating code
	public string displayName;
	public string small_Icon;
	public string large_Icon;

	[Header("Local Notification")]
	public bool enableLocal = true;
	public string androidChannelId = "default_channel";
	public int delaySeconds = 5;
	public bool repeat = false;
	public int repeatIntervalSeconds = 86400; // default daily

	[Header("Content")]
	[TextArea(2, 5)] public string title;
	[TextArea(2, 5)] public string body;

	[Header("Remote")]
	public bool enableRemote = true;
	// remote payload keys can be validated by your backend

	[Header("Conditions")]
	public bool onlyWhenLoggedIn = false;
}
