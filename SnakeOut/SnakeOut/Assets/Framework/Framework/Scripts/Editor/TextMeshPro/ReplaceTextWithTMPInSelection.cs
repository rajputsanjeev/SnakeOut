using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace framework
{
	public class ReplaceTextWithTMPInSelection : EditorWindow
	{
		[MenuItem("Tools/Text Utilities/Replace Selected Texts With TextMeshProUGUI")]
		public static void ReplaceSelectedTexts()
		{
			GameObject[] selectedObjects = Selection.gameObjects;

			if (selectedObjects.Length == 0)
			{
				Debug.LogWarning("⚠️ Please select one or more GameObjects in the Hierarchy.");
				return;
			}

			int replacedCount = 0;

			foreach (GameObject go in selectedObjects)
			{
				// Find all Text components in this selection
				Text[] texts = go.GetComponentsInChildren<Text>(true);
				foreach (Text oldText in texts)
				{
					Undo.RegisterFullObjectHierarchyUndo(oldText.gameObject, "Replace Text with TMP");

					// Backup old Text settings
					string textValue = oldText.text;
					Color color = oldText.color;
					int fontSize = Mathf.RoundToInt(oldText.fontSize);
					TextAnchor alignment = oldText.alignment;
					bool raycastTarget = oldText.raycastTarget;
					FontStyle fontStyle = oldText.fontStyle;

					// Get parent and sibling index
					Transform parent = oldText.transform.parent;
					int siblingIndex = oldText.transform.GetSiblingIndex();

					// Create new TMP object
					GameObject tmpGO = new GameObject(oldText.name, typeof(RectTransform), typeof(TextMeshProUGUI));
					tmpGO.transform.SetParent(parent, false);
					tmpGO.transform.SetSiblingIndex(siblingIndex);

					// Copy RectTransform data
					RectTransform oldRect = oldText.rectTransform;
					RectTransform newRect = tmpGO.GetComponent<RectTransform>();
					newRect.anchorMin = oldRect.anchorMin;
					newRect.anchorMax = oldRect.anchorMax;
					newRect.pivot = oldRect.pivot;
					newRect.sizeDelta = oldRect.sizeDelta;
					newRect.anchoredPosition = oldRect.anchoredPosition;
					newRect.localRotation = oldRect.localRotation;
					newRect.localScale = oldRect.localScale;

					// Configure TMP
					TextMeshProUGUI tmp = tmpGO.GetComponent<TextMeshProUGUI>();
					tmp.text = textValue;
					tmp.color = color;
					tmp.fontSize = fontSize;
					tmp.alignment = ConvertAlignment(alignment);
					tmp.raycastTarget = raycastTarget;

					// Match font style
					if (fontStyle == FontStyle.Bold) tmp.fontStyle = FontStyles.Bold;
					else if (fontStyle == FontStyle.Italic) tmp.fontStyle = FontStyles.Italic;

					// Remove old Text
					Undo.DestroyObjectImmediate(oldText.gameObject);

					replacedCount++;
				}
			}

			Debug.Log($"✅ Replaced {replacedCount} Text components with TextMeshProUGUI in selected objects.");
		}

		// Converts UnityEngine.UI.Text alignment to TMP alignment
		private static TextAlignmentOptions ConvertAlignment(TextAnchor anchor)
		{
			switch (anchor)
			{
				case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
				case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
				case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
				case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
				case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
				case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
				case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
				case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
				case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
				default: return TextAlignmentOptions.Center;
			}
		}
	}

}