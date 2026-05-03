using UnityEngine;
using Framework;



namespace Framework
{
[CreateAssetMenu(fileName = "LoadingScreenConfig", menuName = "Framework/Loading Screen Config")]
public class LoadingScreenConfig : ScriptableObject
{
	[Header("Timing")]
	public float minimumLoadingTime = 2f;
	public float fadeInTime = 0.5f;
	public float fadeOutTime = 0.5f;

	[Header("Tips")]
	[TextArea] public string[] tips;
	public float tipChangeInterval = 3f;

	[Header("Background Slides")]
	public Sprite[] backgroundSlides;
	public float slideChangeInterval = 4f;

	[Header("Progress Mapping")]
	public float progressStart = 0.2f;
	public float progressEnd = 0.9f;
}
}