using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
using Unity.Notifications.Android;

#endif
#if UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
using Unity.Notifications.iOS;
#endif

#if MODULE_FIREBASE_MESSAGING
using Firebase.Messaging;
#endif

namespace Framework.Core
{
	[StaticUnload]
	[Define("MODULE_UNITY_LOCAL_NOTIFICATION", "Unity.Notifications.Android.AndroidNotificationCenter")]
	[Define("MODULE_FIREBASE_MESSAGING", "Firebase.Messaging.FirebaseMessaging", "Firebase.Messaging.dll")]
	public class NotificationManager : MonoBehaviour
	{
		private const string ANDROID_NOTIF_KEY = "ANDROID_NOTIFICATION_MAP";

		public static NotificationManager Instance { get; private set; }

		public NotificationConfig config;

#if UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
        AuthorizationRequest request;
#endif

#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
	PermissionRequest request;
	private readonly System.Collections.Generic.Dictionary<string, int> androidScheduledIds
	= new System.Collections.Generic.Dictionary<string, int>();
#endif

		private void Awake()
		{
			if (Instance != null && Instance != this) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);

			if (config == null)
			{
				Debug.LogError("[NotificationManager] No config assigned.");
				return;
			}

			SetupPlatform();
			RegisterFirebaseHandlers();
		}

		private void SetupPlatform()
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		// Create default channel if not exists
		var c = new AndroidNotificationChannel()
		{
			Id = config.defaultAndroidChannelId,
			Name = config.defaultAndroidChannelName,
			Importance = Importance.High,
			Description = config.defaultAndroidChannelDesc,
		};
		AndroidNotificationCenter.RegisterNotificationChannel(c);
		RequestNotificationPermision(null);
#endif

#if UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
        // Request permission on iOS
        StartCoroutine(RequestiOSAuthorization());
#endif
		}

#if UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
    private IEnumerator RequestiOSAuthorization()
    {
        var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound, true);
        while (!req.IsFinished) yield return null;
        if (config.debugLog) Debug.Log($"[NotificationManager] iOS Authed: {req.AuthorizationStatus}");
    }
#endif

		private void RegisterFirebaseHandlers()
		{
#if MODULE_FIREBASE_MESSAGING
		FirebaseMessaging.TokenReceived += OnTokenReceived;
		FirebaseMessaging.MessageReceived += OnFirebaseMessageReceived;
#endif
		}

#if MODULE_FIREBASE_MESSAGING
	private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
	{
		if (config.debugLog) Debug.Log($"[NotificationManager] FCM Token: {token.Token}");
		// send to your server if necessary
	}

	private void OnFirebaseMessageReceived(object sender, MessageReceivedEventArgs e)
	{
		if (config.debugLog) Debug.Log("[NotificationManager] Remote message received");
		// handle incoming remote message if you want to show local content instead
	}
#endif

		internal void RequestNotificationPermision(UnityEngine.Events.UnityAction<bool, string> completeMethod)
		{
			StartCoroutine(RequestNotificationPermission(completeMethod));
		}

		private IEnumerator RequestNotificationPermission(UnityEngine.Events.UnityAction<bool, string> completeMethod)
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		request = new PermissionRequest();
		while (request.Status == PermissionStatus.RequestPending)
		{
			yield return null;
		}
		if (completeMethod != null)
		{
			completeMethod(request.Status == PermissionStatus.Allowed, request.Status.ToString());
		}

		// here use request.Status to determine users response
#elif UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };
                request = req;
                completeMethod(req.Granted,req.ToString());
            }
#else
			yield return null;
#endif
		}

		/// <summary>
		/// Public API: Trigger notification by id (matches NotificationTemplate.id)
		/// </summary>
		public void Trigger(string id)
		{
			Cancel(id); // Cancel notification if already there

			var tpl = config.templates.FirstOrDefault(t => t != null && t.id == id);
			if (tpl == null)
			{
				if (config.debugLog) Debug.LogWarning($"[NotificationManager] Template {id} not found.");
				return;
			}

			// Mode decision
			switch (config.notificationMode)
			{
				case NotificationConfig.Mode.LocalOnly:
					if (tpl.enableLocal) ScheduleLocal(tpl);
					break;
				case NotificationConfig.Mode.RemoteOnly:
					if (tpl.enableRemote) SendRemoteRequest(tpl);
					break;
				case NotificationConfig.Mode.Both:
					if (tpl.enableLocal) ScheduleLocal(tpl);
					if (tpl.enableRemote) SendRemoteRequest(tpl);
					break;
			}
		}

		/// <summary>
		/// Public API: Trigger notification by id (matches NotificationTemplate.id)
		/// </summary>
		public void Trigger(string id, int second)
		{
			Cancel(id); // Cancel notification if already there

			var config = this.config.templates.FirstOrDefault(t => t != null && t.id == id);
			if (config == null)
			{
				if (this.config.debugLog) Debug.LogWarning($"[NotificationManager] Template {id} not found.");
				return;
			}
			config.delaySeconds = second;

			// Mode decision
			switch (this.config.notificationMode)
			{
				case NotificationConfig.Mode.LocalOnly:
					if (config.enableLocal) ScheduleLocal(config);
					break;
				case NotificationConfig.Mode.RemoteOnly:
					if (config.enableRemote) SendRemoteRequest(config);
					break;
				case NotificationConfig.Mode.Both:
					if (config.enableLocal) ScheduleLocal(config);
					if (config.enableRemote) SendRemoteRequest(config);
					break;
			}
		}

		private bool IsPlayerLoggedIn()
		{
			// TODO: tie into your auth system. Return true for now.
			return true;
		}

		private void ScheduleLocal(NotificationTemplate tpl)
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		var notif = new AndroidNotification();
		notif.Title = tpl.title;
		notif.Text = tpl.body;
		notif.SmallIcon = tpl.small_Icon;   // no file extension
		notif.LargeIcon = tpl.large_Icon;   // no file extension
		notif.FireTime = DateTime.Now.AddSeconds(Mathf.Max(0, tpl.delaySeconds));
		if (tpl.repeat)
		{
			// AndroidNotifications do not directly support repeat intervals;
			// you'd reschedule on receive or use repeating alarms from native plugin.
		}

		var channelId = string.IsNullOrEmpty(tpl.androidChannelId) ? config.defaultAndroidChannelId : tpl.androidChannelId;
		var id = AndroidNotificationCenter.SendNotification(notif, channelId);
		if (config.debugLog) Debug.Log($"[NotificationManager] Scheduled local (Android) '{tpl.displayName}' id:{id}");

		androidScheduledIds[tpl.id] = id;

		if (config.debugLog)
			Debug.Log($"[NotificationManager] Scheduled local (Android) '{tpl.displayName}' id:{id}");

#elif UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0,0,tpl.delaySeconds),
            Repeats = tpl.repeat
        };
        var notif = new iOSNotification()
        {
            Identifier = tpl.id,
            Title = tpl.title,
            Body = tpl.body,
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            CategoryIdentifier = "default",
            Trigger = timeTrigger,
        };
        iOSNotificationCenter.ScheduleNotification(notif);
        if (config.debugLog) Debug.Log($"[NotificationManager] Scheduled local (iOS) '{tpl.displayName}'");
#else
			if (config.debugLog) Debug.Log($"[NotificationManager] Local notifications not supported on this platform.");
#endif
		}

		private void SendRemoteRequest(NotificationTemplate tpl)
		{
			// NOTE: Unity cannot directly send to FCM without server credentials.
			// You should send the request to your server which then calls FCM with proper keys.
			// For convenience we show debug log and place where you'd call your backend.

			if (config.debugLog) Debug.Log($"[NotificationManager] Request to send remote notification: {tpl.displayName}");
			// Example: StartCoroutine(SendRemoteToYourServer(tpl));
		}

		// OPTIONAL: helper to expose schedule time by seconds for testing
		public void TriggerWithDelay(string id, int seconds)
		{
			var tpl = config.templates.Find(t => t.id == id);
			if (tpl == null) { if (config.debugLog) Debug.LogWarning("template not found"); return; }
			tpl.delaySeconds = seconds;
			Trigger(id);
		}

		private void EnsureAndroidMapLoaded()
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		if (androidScheduledIds.Count > 0)
			return;

		if (!PlayerPrefs.HasKey(ANDROID_NOTIF_KEY))
			return;

		var json = PlayerPrefs.GetString(ANDROID_NOTIF_KEY);
		var wrapper = JsonUtility.FromJson<AndroidMapWrapper>(json);

		if (wrapper?.items == null)
			return;

		androidScheduledIds.Clear();
		foreach (var item in wrapper.items)
		{
			androidScheduledIds[item.templateId] = item.androidId;
		}

		if (config.debugLog)
			Debug.Log("[NotificationManager] Android map loaded on-demand");
#endif
		}

		public void Cancel(string templateId)
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		// 🔑 LOAD FIRST — ALWAYS
		EnsureAndroidMapLoaded();

		if (androidScheduledIds.TryGetValue(templateId, out int androidId))
		{
			AndroidNotificationCenter.CancelNotification(androidId);
			androidScheduledIds.Remove(templateId);
			SaveAndroidMap();

			if (config.debugLog)
				Debug.Log($"[NotificationManager] Canceled Android notification: {templateId}");
		}
		else if (config.debugLog)
		{
			Debug.Log($"[NotificationManager] No Android notification found for: {templateId}");
		}
#endif

#if UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
    iOSNotificationCenter.RemoveScheduledNotification(templateId);
    iOSNotificationCenter.RemoveDeliveredNotification(templateId);

    if (config.debugLog)
        Debug.Log($"[NotificationManager] Canceled iOS notification: {templateId}");
#endif
		}

		private void SaveAndroidMap()
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		var wrapper = new AndroidMapWrapper();
		foreach (var kv in androidScheduledIds)
		{
			wrapper.items.Add(new AndroidMapItem
			{
				templateId = kv.Key,
				androidId = kv.Value
			});
		}

		string json = JsonUtility.ToJson(wrapper);
		PlayerPrefs.SetString(ANDROID_NOTIF_KEY, json);
		PlayerPrefs.Save();
#endif
		}

		public void CancelAll()
		{
#if UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		AndroidNotificationCenter.CancelAllScheduledNotifications();
		AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif

#if UNITY_IOS && MODULE_UNITY_LOCAL_NOTIFICATION
    iOSNotificationCenter.RemoveAllScheduledNotifications();
    iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
		}

		[Serializable]
		private class AndroidMapItem
		{
			public string templateId;
			public int androidId;
		}

		[Serializable]
		private class AndroidMapWrapper
		{
#if UNITY_IOS || UNITY_ANDROID && MODULE_UNITY_LOCAL_NOTIFICATION
		public List<AndroidMapItem> items = new List<AndroidMapItem>();
#endif
		}
	}
}
