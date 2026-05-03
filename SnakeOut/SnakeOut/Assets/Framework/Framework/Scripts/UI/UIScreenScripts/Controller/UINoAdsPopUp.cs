using Framework;
using Framework.Core;


namespace Framework
{
	public class UINoAdsPopUp : UIIAPOffer
	{
		public override void PlayShowAnimation()
		{
			base.PlayShowAnimation();
			var noAdCollision = GetComponentInChildren<NoAdsCollisionSequence>();
			if (noAdCollision != null) noAdCollision.OnStart();

			var uiEffectToolkit = GetComponentsInChildren<UIEffectsToolkit>();

			if (uiEffectToolkit.Length > 0)
			{
				foreach (var effect in uiEffectToolkit)
				{
					effect.PlayCurrentEffect();
				}
			}
		}
	}
}
