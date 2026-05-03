using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
	public class InternetCheckerService : MonoBehaviour
	{
		public static bool IsInternetAvailable { get; private set; } = true;
		public static event Action<bool> OnInternetStateChanged;

		private void Awake()
		{
			InvokeRepeating(nameof(CheckInternet), 0, 2);
		}

		private async void CheckInternet()
		{
			bool previous = IsInternetAvailable;

			try
			{
				using UnityWebRequest req =
					UnityWebRequest.Head("https://www.google.com");
				req.timeout = 2;
				await req.SendWebRequest();

				IsInternetAvailable =
					req.result == UnityWebRequest.Result.Success;
			}
			catch
			{
				IsInternetAvailable = false;
			}

			if (previous != IsInternetAvailable)
			{
				if (!IsInternetAvailable) ToastMessage.Instance.Show("No Internet Connection");
				OnInternetStateChanged?.Invoke(IsInternetAvailable);
			}
		}
	}
}