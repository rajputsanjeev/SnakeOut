using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Framework
{
	public class GetCurrentLevel : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI CurrentLevel;

		private void Start()
		{
			CurrentLevel = GetComponent<TextMeshProUGUI>();
			Invoke(nameof(SetLevel), 1);
		}

		private void SetLevel()
		{
			if (CurrentLevel == null)
			{
				CurrentLevel = GetComponent<TextMeshProUGUI>();
			}
			CurrentLevel.text = GetCurrentLevelAbstract.Instance.GetLevel().ToString();
		}
	}
}
