using Framework.Core;
using UnityEngine;

public class IsAdDisable : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		gameObject.SetActive(AdsManager.IsForcedAdEnabled());
	}
}
