using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextAnim : MonoBehaviour
{
	public bool autoStart;

	[Header("References")]
	public TextMeshProUGUI tmpText;

	[Header("Text Source")]
	[TextArea] public string customText;
	public bool useCustomText;

	[Header("Timing")]
	public float startDelay = 0.25f;
	public float charCascadeDelay = 0.04f;
	public float popDuration = 0.25f;
	public float holdTime = 2f;

	[Header("Ease")]
	public Ease easeIn = Ease.OutBack;
	public Ease easeOut = Ease.InBack;

	private TMP_TextInfo textInfo;
	private Vector3[][] cachedVertices;
	private string fullText;

	private void Start()
	{
		if (autoStart)
			StartTypeText();
	}

	public void SetText(string text, bool startAnimatio = false)
	{
		fullText = text;
		Debug.Log("Animated text " + tmpText.text);

		if (startAnimatio)
		{
			gameObject.SetActive(true);
			StartTypeText();
		}
	}

	public void StartTypeText()
	{
		StopAllCoroutines();
		DOTween.Kill(this); // safety cleanup
		StartCoroutine(AnimateText());
	}

	private IEnumerator AnimateText()
	{
		tmpText.text = "";
		tmpText.ForceMeshUpdate();

		yield return new WaitForSeconds(startDelay);

		tmpText.text = fullText;
		tmpText.ForceMeshUpdate();

		textInfo = tmpText.textInfo;

		CacheOriginalVertices();
		SetAllCharactersScale(0f);

		// POP IN
		for (int i = 0; i < textInfo.characterCount; i++)
		{
			AnimateCharacter(i, 0f, 1f, popDuration, easeIn);
			yield return new WaitForSeconds(charCascadeDelay);
		}

		yield return new WaitForSeconds(holdTime);

		// POP OUT
		for (int i = 0; i < textInfo.characterCount; i++)
		{
			AnimateCharacter(i, 1f, 0f, popDuration, easeOut);
			yield return new WaitForSeconds(charCascadeDelay);
		}
	}

	private void CacheOriginalVertices()
	{
		cachedVertices = new Vector3[textInfo.meshInfo.Length][];

		for (int i = 0; i < textInfo.meshInfo.Length; i++)
		{
			cachedVertices[i] =
				textInfo.meshInfo[i].vertices.Clone() as Vector3[];
		}
	}

	private void SetAllCharactersScale(float scale)
	{
		for (int i = 0; i < textInfo.characterCount; i++)
		{
			ApplyScaleToCharacter(i, scale);
		}

		tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
	}

	private void AnimateCharacter(
		int charIndex,
		float from,
		float to,
		float duration,
		Ease ease)
	{
		if (!textInfo.characterInfo[charIndex].isVisible)
			return;

		TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
		int meshIndex = charInfo.materialReferenceIndex;
		int vertexIndex = charInfo.vertexIndex;

		Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
		Vector3[] origVerts = cachedVertices[meshIndex];

		Vector3 center =
			(origVerts[vertexIndex] + origVerts[vertexIndex + 2]) * 0.5f;

		float scale = from;

		DOTween.To(
				() => scale,
				x => scale = x,
				to,
				duration)
			.SetEase(ease)
			.SetTarget(this)
			.OnUpdate(() =>
			{
				for (int i = 0; i < 4; i++)
				{
					vertices[vertexIndex + i] =
						center + (origVerts[vertexIndex + i] - center) * scale;
				}

				tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
			});
	}

	private void ApplyScaleToCharacter(int charIndex, float scale)
	{
		if (!textInfo.characterInfo[charIndex].isVisible)
			return;

		TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
		int meshIndex = charInfo.materialReferenceIndex;
		int vertexIndex = charInfo.vertexIndex;

		Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
		Vector3[] origVerts = cachedVertices[meshIndex];

		Vector3 center =
			(origVerts[vertexIndex] + origVerts[vertexIndex + 2]) * 0.5f;

		for (int i = 0; i < 4; i++)
		{
			vertices[vertexIndex + i] =
				center + (origVerts[vertexIndex + i] - center) * scale;
		}
	}
}
