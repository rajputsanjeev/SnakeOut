using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Framework
{
	[System.Serializable]
	public class TMPPopupSettings
	{
		// ---------- OPTIONAL PREFAB ----------
		// If null → text will be created at runtime
		public TextMeshProUGUI TextPrefab;

		// ---------- PARENT ----------
		public RectTransform Parent;

		// ---------- POSITION ----------
		public Vector3 WorldPosition;

		// ---------- SCALE ----------
		public Vector3 StartScale = Vector3.zero;
		public Vector3 EndScale = Vector3.one;
		public float ScaleInDuration = 0.2f;

		// ---------- JUMP MOVE ----------
		public float MoveUpDistance = 80f;
		public float JumpPower = 120f;
		public int JumpCount = 1;
		public float MoveDuration = 0.6f;

		// ---------- OUTLINE ----------
		public bool EnableOutline = true;
		public Color OutlineColor = Color.yellow;
		public float OutlineWidth = 0.3f;

		// ---------- FADE ----------
		public bool FadeIn = false;
		public float FadeInDuration = 0.1f;

		public bool FadeOut = true;
		public float FadeOutDelay = 0.1f;
		public float FadeOutDuration = 0.3f;

		// ---------- END SCALE ----------
		public bool ScaleToZeroAtEnd = true;
		public float EndScaleDuration = 0.15f;

		// ---------- COLOR ----------
		public Color TextColor = Color.white;
		public int FontSize = 36;
	}

	public static class TMPPopupPlayer
	{
		/// <summary>
		/// Create → animate → destroy TMP popup text
		/// </summary>
		public static Tween Play(string textValue, TMPPopupSettings s)
		{
			// ---------- Create Text ----------
			TextMeshProUGUI text;

			if (s.TextPrefab != null)
			{
				text = Object.Instantiate(s.TextPrefab, s.Parent);
			}
			else
			{
				GameObject go = new GameObject("TMP_Popup");
				go.transform.SetParent(s.Parent, false);
				text = go.AddComponent<TextMeshProUGUI>();
			}

			RectTransform rt = text.rectTransform;

			// ---------- Setup ----------
			text.text = "+" + textValue;
			text.color = s.TextColor;
			text.fontSize = s.FontSize;
			text.verticalAlignment = VerticalAlignmentOptions.Middle;
			text.horizontalAlignment = HorizontalAlignmentOptions.Center;

			rt.localPosition = s.WorldPosition;
			rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.one * 0.5f;
			rt.localScale = s.StartScale;

			// Outline
			if (s.EnableOutline)
			{
				text.outlineColor = s.OutlineColor;
				text.outlineWidth = s.OutlineWidth;
			}
			else
			{
				text.outlineWidth = 0;
			}

			// Fade in setup
			if (s.FadeIn)
			{
				Color c = text.color;
				c.a = 0;
				text.color = c;
			}

			Vector2 startPos = rt.anchoredPosition;
			Vector2 endPos = startPos + Vector2.up * s.MoveUpDistance;

			// ---------- Animation ----------
			Sequence seq = DOTween.Sequence().SetUpdate(true);

			// Fade in
			if (s.FadeIn)
			{
				seq.Append(text.DOFade(1f, s.FadeInDuration));
			}

			// Scale in
			seq.Join(
				rt.DOScale(s.EndScale, s.ScaleInDuration)
				  .SetEase(Ease.OutBack)
			);

			// Jump upward
			seq.Join(
				rt.DOJumpAnchorPos(
					endPos,
					s.JumpPower,
					s.JumpCount,
					s.MoveDuration
				).SetEase(Ease.OutQuad)
			);

			// Fade out
			if (s.FadeOut)
			{
				seq.AppendInterval(s.FadeOutDelay);
				seq.Append(text.DOFade(0f, s.FadeOutDuration));
			}

			// Scale to zero at end
			if (s.ScaleToZeroAtEnd)
			{
				seq.Join(
					rt.DOScale(Vector3.zero, s.EndScaleDuration)
					  .SetEase(Ease.InBack)
				);
			}

			// ---------- Cleanup ----------
			seq.OnComplete(() =>
			{
				Object.Destroy(text.gameObject);
			});

			return seq;
		}
	}
}
