namespace Framework
{
	public static class LoadingEvents
	{
		public delegate void LoadSceneDelegate(string sceneName);
		public static event LoadSceneDelegate OnLoadSceneRequested;

		public static void RequestSceneLoad(string sceneName)
		{
			OnLoadSceneRequested?.Invoke(sceneName);
		}
	}
}