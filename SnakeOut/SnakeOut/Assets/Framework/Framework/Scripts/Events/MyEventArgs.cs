using System.Collections.Generic;
using UnityEngine;

namespace Frameork
{
	public static class MyEventArgs
	{
		public static class GameControllerEvents
		{
			public static int LevelCompleteCount;
			public static MyEvent OnLevelWin = new MyEvent();
			public static MyEvent<int, int> OnLevelComplete = new MyEvent<int, int>();
			public static MyEvent OnLevelFailed = new MyEvent();
			public static MyEvent OnSettingBack = new MyEvent();
			public static MyEvent<bool> OnCorrectObjectClicked = new MyEvent<bool>();
			public static MyEvent<bool> OnWrongObjectClicked = new MyEvent<bool>();
			public static MyEvent<int> SendTotalMove = new MyEvent<int>();
			public static MyEvent<bool> OnScreenOpen = new MyEvent<bool>();
		}
	}
}