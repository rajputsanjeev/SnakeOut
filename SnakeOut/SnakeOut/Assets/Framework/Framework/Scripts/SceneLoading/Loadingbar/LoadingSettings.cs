using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "LoadingSettings", menuName = "Loading/Loading Settings")]
	public class LoadingSettings : ScriptableObject
	{
		[Header("Fade Settings")]
		public float fadeInDuration = 0.5f;
		public float fadeOutDuration = 0.5f;

		[Header("Tips Settings")]
		public string[] tips;
		public float tipChangeInterval = 3f;

		[Header("Slideshow Settings")]
		public Sprite[] backgroundSlides;
		public float slideChangeInterval = 4f;
		public float slideFadeDuration = 1f;

		[Header("Spinner Settings")]
		public float spinnerSpeed = 150f;

		[Header("Tap To Continue")]
		public bool waitForUserInput = false;
	}
}