using System.Collections;
using UnityEngine;
using Framework.Core;

#if MODULE_GOOGLE_IN_APP_UPDATE
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

using TMPro;

namespace Framework
{
	[StaticUnload]
	[Define("MODULE_GOOGLE_IN_APP_UPDATE", "Google.Play.AppUpdate.AppUpdateManager", "com.google.play.appupdate.metadata.dll")]
	public class GooglePlayInAppUpdate : MonoBehaviour
	{
		[Header("Update Settings")]
#if MODULE_GOOGLE_IN_APP_UPDATE
	[SerializeField] private UpdateMode updateMode = UpdateMode.Flexible;
	
#endif
		[SerializeField] private bool checkOnStart = true;
		[SerializeField] private bool autoInstallAfterDownload = true;

		[Header("UI (Optional)")]
		[SerializeField] private GameObject downloadProgressPanel;
		[SerializeField] private UnityEngine.UI.Slider progressBar;
		[SerializeField] private TextMeshProUGUI progressText;
		[SerializeField] private GameObject installPromptPanel;
		[SerializeField] private UnityEngine.UI.Button installNowButton;
		[SerializeField] private UnityEngine.UI.Button installLaterButton;

#if MODULE_GOOGLE_IN_APP_UPDATE
		private AppUpdateManager appUpdateManager;
#endif
		private bool updateReadyToInstall = false;

		public enum UpdateMode
		{
			Flexible,   // User can continue using app while downloading
			Immediate   // User must update before continuing
		}

#if MODULE_GOOGLE_IN_APP_UPDATE
		void Start()
		{

	appUpdateManager = new AppUpdateManager();

			if (downloadProgressPanel != null)
				downloadProgressPanel.SetActive(false);

			if (installPromptPanel != null)
				installPromptPanel.SetActive(false);

			if (installNowButton != null)
				installNowButton.onClick.AddListener(OnInstallNowClicked);

			if (installLaterButton != null)
				installLaterButton.onClick.AddListener(OnInstallLaterClicked);

			// Check for pending updates first (from previous session)

			StartCoroutine(CheckPendingUpdate());

			if (checkOnStart)
			{
				StartCoroutine(CheckForUpdate());
			}
		}


		// Check if there's a pending update from previous session
		private IEnumerator CheckPendingUpdate()
		{
	PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
				appUpdateManager.GetAppUpdateInfo();
			
			yield return appUpdateInfoOperation;

			if (appUpdateInfoOperation.IsSuccessful)
			{
				var appUpdateInfo = appUpdateInfoOperation.GetResult();

				// Check if update was downloaded but not installed
				if (appUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
				{
					Debug.Log("Found pending update from previous session.");

					// Check if it's downloaded and ready to install
					if (appUpdateInfo.BytesDownloaded >= appUpdateInfo.TotalBytesToDownload &&
						appUpdateInfo.TotalBytesToDownload > 0)
					{
						Debug.Log("Update is ready to install!");
						updateReadyToInstall = true;

						if (autoInstallAfterDownload)
						{
							// Auto-install the pending update
							CompleteFlexibleUpdate();
						}
						else
						{
							// Show install prompt
							if (installPromptPanel != null)
								installPromptPanel.SetActive(true);
						}
					}
					else
					{
						// Update is still downloading, resume monitoring
						if (downloadProgressPanel != null)
							downloadProgressPanel.SetActive(true);

						StartCoroutine(MonitorDownloadProgress());
					}
				}
			}
		}

		public IEnumerator CheckForUpdate()
		{
			Debug.Log("Checking for updates...");

			// Small delay to avoid conflicting with pending update check
			yield return new WaitForSeconds(0.5f);

			// Check for update availability
			PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
				appUpdateManager.GetAppUpdateInfo();

			// Wait until request completed
			yield return appUpdateInfoOperation;

			if (appUpdateInfoOperation.IsSuccessful)
			{
				var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

				// Check if update is available
				if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
				{
					Debug.Log("Update available!");

					// Check allowed update types
					AppUpdateOptions updateOptions;

					if (updateMode == UpdateMode.Immediate)
					{
						updateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
					}
					else
					{
						updateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();
					}

					// Start the update flow
					StartCoroutine(StartUpdate(appUpdateInfoResult, updateOptions));
				}
				else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
				{
					Debug.Log("No update available.");
				}
				else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
				{
					Debug.Log("Update already in progress (handled by CheckPendingUpdate).");
				}
			}
			else
			{
				Debug.LogError("Error checking for update: " + appUpdateInfoOperation.Error);
			}
		}

		private IEnumerator StartUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions updateOptions)
		{
			// Create update request
			var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, updateOptions);

			yield return startUpdateRequest;

			if (startUpdateRequest.IsDone)
			{
				Debug.Log("Update started successfully!");

				if (updateMode == UpdateMode.Flexible)
				{
					// Show download progress panel
					if (downloadProgressPanel != null)
						downloadProgressPanel.SetActive(true);

					// Monitor download progress for flexible updates
					StartCoroutine(MonitorDownloadProgress());
				}
			}
			else
			{
				Debug.LogError("Error starting update: " + startUpdateRequest.Error);
			}
		}

		private IEnumerator MonitorDownloadProgress()
		{
			while (appUpdateManager != null)
			{
				// Get current update info
				PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> infoOperation =
					appUpdateManager.GetAppUpdateInfo();

				yield return infoOperation;

				if (infoOperation.IsSuccessful)
				{
					var info = infoOperation.GetResult();

					// Check download progress
					if (info.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
					{
						ulong bytesDownloaded = info.BytesDownloaded;
						ulong totalBytes = info.TotalBytesToDownload;

						if (totalBytes > 0)
						{
							float progress = (float)bytesDownloaded / totalBytes * 100f;
							Debug.Log($"Download progress: {progress:F1}%");

							// You can update UI here with progress
							OnDownloadProgress(progress);
						}

						// Check if download is complete
						if (bytesDownloaded >= totalBytes && totalBytes > 0)
						{
							Debug.Log("Update downloaded! Ready to install.");
							OnDownloadComplete();
							yield break;
						}
					}
					else if (info.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
					{
						Debug.Log("Update completed or no longer available.");
						yield break;
					}
				}

				yield return new WaitForSeconds(0.5f);
			}
		}

		// Call this to complete flexible update installation
		public void CompleteFlexibleUpdate()
		{
			var completeUpdateRequest = appUpdateManager.CompleteUpdate();
			completeUpdateRequest.Completed += (result) =>
			{
				if (result.Error == AppUpdateErrorCode.NoError)
				{
					Debug.Log("Update installation started. App will restart.");
				}
				else
				{
					Debug.LogError("Failed to complete update: " + result.Error);
				}
			};
		}

		// Optional: Override these methods to update your UI
		protected virtual void OnDownloadProgress(float progress)
		{
			// Update progress bar
			if (progressBar != null)
			{
				progressBar.value = progress / 100f; // Slider value is 0-1
			}

			// Update progress text
			if (progressText != null)
			{
				progressText.text = $"Downloading update: {progress:F1}%";
			}
		}

		protected virtual void OnDownloadComplete()
		{
			updateReadyToInstall = true;
			Debug.Log("Update ready to install!");

			// Hide download progress panel
			if (downloadProgressPanel != null)
				downloadProgressPanel.SetActive(false);

			if (autoInstallAfterDownload)
			{
				// Automatically trigger installation
				Debug.Log("Auto-installing update...");
				CompleteFlexibleUpdate();
			}
			else
			{
				// Show install prompt UI with Install Now / Later buttons
				if (installPromptPanel != null)
					installPromptPanel.SetActive(true);
			}
		}

		// Handle "Install Now" button click
		private void OnInstallNowClicked()
		{
			if (installPromptPanel != null)
				installPromptPanel.SetActive(false);

			CompleteFlexibleUpdate();
		}

		// Handle "Install Later" button click
		private void OnInstallLaterClicked()
		{
			if (installPromptPanel != null)
				installPromptPanel.SetActive(false);

			Debug.Log("User chose to install later. Update will be available on next app restart.");
			// Update is already downloaded, will be installed on next app launch
		}
#endif
	}
}