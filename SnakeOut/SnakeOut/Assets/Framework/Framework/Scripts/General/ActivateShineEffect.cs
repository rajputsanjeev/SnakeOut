using System.Security.Cryptography;
using Coffee.UIEffects;
using UnityEngine;

namespace Framework
{
	public class ActivateShineEffect : MonoBehaviour
	{
		public Animator ButtonAnimator;
		public UIShiny UIShiny;

		private void Awake()
		{
			ButtonAnimator = GetComponent<Animator>();
			UIShiny = GetComponent<UIShiny>();
		}

		public void ActivateShine(bool isActivate)
		{
			if (ButtonAnimator != null && UIShiny != null)
			{
				ButtonAnimator.enabled = isActivate;
				UIShiny.enabled = isActivate;
			}
		}
	}
}