/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace NiobiumStudios
{
    /**
     * Representation of the world clock JSON Object as shown at http://worldclockapi.com/
     **/
    [Serializable]
    public class CloudClockResult
    {
        public string datetime;
        public string currentDateTime;
        public string utcOffset;
        public string dayOfTheWeek;
        public string timeZoneName;
        public double currentFileTime;
        public string ordinalDate;
        public string serviceResponse;
    }

    /**
    * Handles the Cloud Clock global instance
    **/
    public static class CloudClockBuilder
    {
        public static string errorMessage = String.Empty;              // Failed error message
        public static DateTime cloudClockDate;                        // Global DateTime
        public static State currentState;                              // Time Server current status

        private static int connectionRetries;                          // Retries counter
        private static int serverClockIndex;                           // Index of the working Time Server

        // Cloud Clock possible states
        public enum State
        {
            NotInitialized,
            Initializing,
            Initialized,
            FailedToInitialize
        };

		public static IEnumerator InitializeCloudClock(List<CloudClock> cloudClockList, int maxRetries)
		{
			currentState = State.Initializing;

			foreach (var cloudClock in cloudClockList)
			{
				if (currentState == State.Initialized)
					yield break;

				connectionRetries = 0;
				string result = string.Empty;

				while (connectionRetries < maxRetries)
				{
					using (UnityWebRequest request = UnityWebRequest.Get(cloudClock.url))
					{
						yield return request.SendWebRequest();

						if (request.result != UnityWebRequest.Result.Success)
						{
							connectionRetries++;
							Debug.LogWarning($"Error Loading Cloud Clock {cloudClock.url}. Retrying connection {connectionRetries}");
							errorMessage = request.error;
						}
						else
						{
							result = request.downloadHandler.text;
							break;
						}
					}
				}

				if (!string.IsNullOrEmpty(result))
				{
					// Parse JSON
#if UNITY_5_3_OR_NEWER
					var cloudClockResultFromJson = JsonUtility.FromJson<CloudClockResult>(result);
#else
                var cloudClockResultJson = JSON.Parse(result);
                var cloudClockResultFromJson = new CloudClockResult();

                if (cloudClockResultJson["currentDateTime"] != null)
                    cloudClockResultFromJson.currentDateTime = cloudClockResultJson["currentDateTime"].Value;
                else if (cloudClockResultJson["datetime"] != null)
                    cloudClockResultFromJson.datetime = cloudClockResultJson["datetime"].Value;
#endif

					string dateTimeStr = cloudClockResultFromJson.currentDateTime;
					if (string.IsNullOrEmpty(dateTimeStr))
						dateTimeStr = cloudClockResultFromJson.datetime;

					try
					{
						cloudClockDate = DateTime.ParseExact(dateTimeStr, cloudClock.timeFormat, CultureInfo.InvariantCulture);
						// Add local seconds
						cloudClockDate = cloudClockDate.AddSeconds(DateTime.Now.Second);

						string time = $"{cloudClockDate:yyyy/MM/dd HH:mm:ss}";
						Debug.Log("Time Cloud Clock: " + time);
						currentState = State.Initialized;
					}
					catch (Exception ex)
					{
						Debug.LogError("Error parsing Cloud Clock date: " + ex.Message);
						currentState = State.FailedToInitialize;
					}
				}
				else
				{
					Debug.LogError($"Error Loading Cloud Clock: {cloudClock.url} Error: {errorMessage}");
					currentState = State.FailedToInitialize;
				}
			}
		}
	}
}