using System;

namespace Framework.Core
{
	[StaticUnload]
	public static partial class AnalyticsModules
	{
		private static BaseAnalyticsModule[] analyticsModules;

		public static void Init()
		{
			analyticsModules = GetAnalyticsModules();

			foreach (var module in analyticsModules)
			{
				module.Init();
			}
		}

		public static void OnModuleInitialized<T>()
		{
			Type type = typeof(T);

			if (!analyticsModules.IsNullOrEmpty())
			{
				foreach (BaseAnalyticsModule analyticsModule in analyticsModules)
				{
					if (analyticsModule.GetType() == type)
						analyticsModule.Init();
				}
			}
		}

		private static void UnloadStatic()
		{
			analyticsModules = null;
		}
	}
}
