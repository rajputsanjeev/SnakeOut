using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LayoutAutoRefresher : MonoBehaviour
{
	[Header("Target Layout Root (with LayoutGroup)")]
	public LayoutGroup layoutRoot;
	public ContentSizeFitter sizeFitter;

	[Header("Refresh Settings")]
	public float delay = 0.5f;
	public bool refreshOnEnable = true;

	private void OnEnable()
	{
		layoutRoot = GetComponent<LayoutGroup>();
		sizeFitter = GetComponent<ContentSizeFitter>();

		//if (refreshOnEnable)
		//	StartCoroutine(RefreshRoutine());

		InvokeRepeating(nameof(RefreshNow), 1, 2);
	}

	public void RefreshNow()
	{
		StartCoroutine(RefreshRoutine());
	}

	private IEnumerator RefreshRoutine()
	{
		// Wait one frame so Unity builds UI first
		yield return null;

		// First refresh
		layoutRoot.enabled = false;
		sizeFitter.enabled = false;

		// Wait for given seconds
		yield return new WaitForSeconds(delay);

		// Second refresh (after some seconds)
		layoutRoot.enabled = true;
		sizeFitter.enabled = true;
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<RectTransform>());
	}
}
