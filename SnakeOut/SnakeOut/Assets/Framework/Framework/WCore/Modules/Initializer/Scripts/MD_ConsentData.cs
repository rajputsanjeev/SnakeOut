namespace Framework.Core
{
	[StaticUnload]
	public static class ConsentData
	{
		public static bool IsConsentGiven { get; private set; } = false;
		public static AuthorizationTrackingStatus ATTStatus { get; private set; } = AuthorizationTrackingStatus.NOT_DETERMINED;

		public static void SetATTStatus(AuthorizationTrackingStatus status)
		{
			ATTStatus = status;
		}

		public static void SetConsentGiven(bool consentGiven)
		{
			IsConsentGiven = consentGiven;
		}

		private static void UnloadStatic()
		{
			IsConsentGiven = false;
			ATTStatus = AuthorizationTrackingStatus.NOT_DETERMINED;
		}
	}
}
