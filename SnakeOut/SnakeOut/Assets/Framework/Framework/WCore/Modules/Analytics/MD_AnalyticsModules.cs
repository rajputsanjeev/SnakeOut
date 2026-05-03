using Framework;

namespace Framework.Core
{
	public static partial class AnalyticsModules
	{
		private static BaseAnalyticsModule[] GetAnalyticsModules()
		{
			return new BaseAnalyticsModule[]
			{
				new FirebaseAnalyticsModule(),
				new GameAnalyticsModule(),
				//new FacebookAnalyticsModule(),
			};
		}
	}
}