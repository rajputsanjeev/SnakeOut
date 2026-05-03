using UnityEngine;

namespace Framework
{
	public class ShareManager : MonoBehaviour
	{
		public string gameUrlAndroid = "https://play.google.com/store/apps/details?id=com.NextGame.BlockFlowColourJam&hl=en-US&ah=0GQVywcl3RdyZ785Ckq-A0u7IsY";
		public string gameUrliOS = "https://apps.apple.com/app/id1234567890";
		public void ShareGame()
		{
			string shareText = "Check out this awesome game I am playing!";

#if UNITY_ANDROID
			shareText += "\n" + gameUrlAndroid;
#elif UNITY_IOS
        shareText += "\n" + gameUrliOS;
#else
        shareText += "\n" + gameUrlAndroid;
#endif

#if NATIVE_SHARE
			new NativeShare()
				.SetText(shareText)
				.Share();
#endif
		}
	}
}
