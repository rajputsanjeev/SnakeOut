using UnityEngine;
using TMPro;
using DG.Tweening;
using Framework;
using System;

public class MysteryBoxTextAnim : GenericSingletonClass<MysteryBoxTextAnim>
{
	[Header("Animation Settings")]
	[SerializeField] private float popUpHeight = 150f;   // How high it pops
	[SerializeField] private float duration = 0.6f;      // Duration of pop animation
	[SerializeField] private Canvas parentCanvas;     // Where text will spawn (Canvas transform)

	/// <summary>
	/// Creates and animates reward text from a given world/UI position
	/// </summary>
	public void ShowRewardText(Vector3 spawnPosition, Transform parent = null, string message = "0", Action onComplete = null,int fontSize = 100, Color? textColor = null, FontStyles fontStyle = FontStyles.Bold, Color? outlineColor = null, float outlineWidth = 0.2f)
	{
		// ✅ Create TextMeshProUGUI object at runtime
		var textGO = new GameObject("PopUpText");
		if (parent == null)
		{
			textGO.transform.SetParent(parentCanvas.transform, false);
		}
		else
		{

			textGO.transform.SetParent(parent, false);
		}

		var rewardText = textGO.AddComponent<TextMeshProUGUI>();

		// ✅ Assign properties
		rewardText.text = message;
		rewardText.fontSize = fontSize;
		rewardText.color = textColor ?? Color.green; // default Yellow
		rewardText.fontStyle = fontStyle;

		// ✅ Outline/Border (using TMP effects)
		rewardText.outlineColor = outlineColor ?? Color.black;
		rewardText.outlineWidth = outlineWidth;

		// ✅ Position setup
		var rect = rewardText.rectTransform;
		rect.anchoredPosition = spawnPosition;
		rect.localScale = Vector3.zero; // start hidden
		rewardText.alpha = 0;

		// ✅ Animate using DOTween
		var seq = DOTween.Sequence();
		seq.Append(rect.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack)); // pop
		seq.Join(rewardText.DOFade(1f, 0.3f)); // fade in
		seq.Join(rect.DOAnchorPosY(spawnPosition.y + popUpHeight, duration).SetEase(Ease.OutQuad)); // move up
		seq.Append(rect.DOScale(1f, 0.2f)); // settle
		seq.AppendInterval(1.2f); // stay visible
		seq.Append(rewardText.DOFade(0f, 0.3f)); // fade out
		seq.OnComplete(() =>
		{
			onComplete?.Invoke();
			Destroy(textGO); // clean up after animation
		});
	}
}
