using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
	public class UtcTimeService : GenericSingletonClass<UtcTimeService>
	{
		// How often to sync from server (in minutes)
		private const int SyncIntervalMinutes = 1;
		private const string Url = "https://timeapi.io/api/Time/current/zone?timeZone=UTC";

		private DateTime CurrentUtc;
		public bool IsOffline;


		private void Start()
		{
			StartCoroutine(UtcUpdaterRoutine());
		}

		/// <summary>
		/// Runs forever: syncs API every X minutes + increments time every second.
		/// </summary>
		private IEnumerator UtcUpdaterRoutine()
		{
			// Initial fetch
			yield return FetchAndSetUtc();

			while (true)
			{
				// Increment local UTC clock every second
				yield return new WaitForSeconds(1f);
				CurrentUtc = CurrentUtc.AddSeconds(1);

				// Sync API at interval
				if (DateTime.UtcNow.Minute % SyncIntervalMinutes == 0 &&
					DateTime.UtcNow.Second == 0)
				{
					yield return FetchAndSetUtc();
				}
			}
		}

		/// <summary>
		/// Fetch from API OR system fallback and update CurrentUtc.
		/// </summary>
		private IEnumerator FetchAndSetUtc()
		{
			bool completed = false;

			GetCurrentUtc((utc) =>
			{
				CurrentUtc = utc;
				completed = true;
			});

			while (!completed)
				yield return null;
		}

		public void GetCurrentUtc(Action<DateTime> callback)
		{
			StartCoroutine(GetUtcCoroutine(callback));
		}

		private IEnumerator GetUtcCoroutine(Action<DateTime> callback)
		{
			if (IsOffline)
			{
				callback?.Invoke(DateTime.UtcNow);
				yield break;
			}

			using (UnityWebRequest req = UnityWebRequest.Get(Url))
			{
				req.timeout = 5;
				yield return req.SendWebRequest();

				bool isError =
					req.result == UnityWebRequest.Result.ConnectionError ||
					req.result == UnityWebRequest.Result.ProtocolError;

				if (!isError)
				{
					try
					{
						ResponseData data = JsonUtility.FromJson<ResponseData>(req.downloadHandler.text);
						Debug.Log("UTC Time: " + data.dateTime);
						DateTime utc = DateTime.Parse(data.dateTime).ToUniversalTime();
						callback?.Invoke(utc);
						yield break;
					}
					catch { Debug.LogError("UTC JSON Parse Error"); }
				}

				// fallback
				callback?.Invoke(DateTime.UtcNow);
			}
		}

		public DateTime GetCurrentDate()
		{
			return CurrentUtc;
		}

		[Serializable]
		private class ResponseData
		{
			public string dateTime;
		}
	}
}
