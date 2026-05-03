using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "ButtonEffectPreset", menuName = "UI/Button Effect Preset")]
	public class UIButtonEffectPreset : ScriptableObject
	{
		[Header("General")]
		public bool enableScaleEffect = true;
		public bool enableColorEffect = false;
		public float animationDuration = 0.15f;

		[Header("Hover")]
		public float hoverScale = 1.05f;
		public Color hoverColor = Color.white;

		[Header("Press")]
		public float pressScale = 0.9f;
		public Color pressColor = Color.white;

		[Header("Click")]
		public float clickScale = 1.15f;
		public Color clickColor = Color.white;

		[Header("Audio")]
		public AudioClip clickSFX;
		public float clickVolume = 1f;
	}
}