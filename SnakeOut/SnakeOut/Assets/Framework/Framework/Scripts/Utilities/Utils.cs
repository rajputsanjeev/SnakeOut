


using System;
using System.Globalization;
using System.Net.Mail;
using UnityEngine;

namespace Framework
{
	public static class Utils
	{
		public static bool IsEmpty(string value)
		{
			return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
		}

		public static string ToTitleCase(string value)
		{
			// Convert to proper case.
			CultureInfo culture_info = new CultureInfo(1);
			TextInfo text_info = culture_info.TextInfo;
			value = text_info.ToTitleCase(value);
			return value;
		}

		public static void DownloadImage(string url, System.Action<Sprite> callback)
		{
			GameObject obj = new GameObject();
			DownloadImageHandler downloadImageHandler = obj.AddComponent<DownloadImageHandler>();
			downloadImageHandler.StartDownload(url, callback);
		}
		public static bool IsValid(string emailaddress)
		{
			try
			{
				MailAddress m = new MailAddress(emailaddress);

				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

		public static bool IsDateLaterWithUtc(DateTime compare)
		{
			DateTime now = DateTime.UtcNow;
			return Utils.CheckEventLater(now, compare);
		}

		public static bool IsDateLater(DateTime date1, DateTime compare)
		{
			return Utils.CheckEventLater(date1, compare);
		}

		public static bool CheckEventLater(DateTime now, DateTime end)
		{
			int value = DateTime.Compare(now, end);
			return value > 0; // If now is later than end
		}

		public static bool IsDateEarlier(DateTime date1, DateTime compare)
		{
			return Utils.CheckEventEarlier(date1, compare);
		}

		public static bool CheckEventEarlier(DateTime now, DateTime end)
		{
			int value = DateTime.Compare(now, end);
			return (value < 0); // If now is earlier than end
		}
		public static DateTime CovertToDateTime(string dateTime)
		{
			if (dateTime == null)
				return new DateTime();

			DateTime dt = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
								   DateTimeStyles.None);
			return dt;
		}

		public static DateTime CovertToDateTimeWithOutUtc(string dateTime)
		{
			DateTime dt = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
								   DateTimeStyles.None);
			return dt;
		}

		public static TimeSpan TimeLeft(DateTime dateTimeNow, DateTime dateTimeCompare)
		{
			var prevDate = dateTimeCompare;
			var today = dateTimeNow;

			//get difference of two dates
			var diffOfDates = today.Subtract(prevDate);
			return diffOfDates;
		}

		public static int GetTotalSeconds(DateTime final, DateTime current)
		{
			int result = DateTime.Compare(final, current);

			if (result > 0)
			{
				DateTime dt = current;//from 1970/1/1 00:00:00 to now
				DateTime dtNow = final;
				TimeSpan resultTimespan = dtNow.Subtract(dt);
				int seconds = Convert.ToInt32(resultTimespan.TotalSeconds);
				return seconds;
			}
			else
			{
				DateTime dt = current;//from 1970/1/1 00:00:00 to now
				DateTime dtNow = final;
				TimeSpan resultTimespan = dt.Subtract(dtNow);
				int seconds = Convert.ToInt32(resultTimespan.TotalSeconds);
				return seconds;
			}
		}
	}
}
