using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Framework
{
	public class LoadingProgressView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI loadingText;
		[SerializeField] private Image slider;

		private void OnEnable()
		{
			loadingText.text = "";
			slider.fillAmount = 0f;
		}

		public void SetLoading(float progress, string loadingMsg = null)
		{
			int progressInt = Mathf.RoundToInt(progress * 100f);
			loadingText.text = loadingMsg + "Loading..." + (progress * 100).ToString("F0") + "%";
			slider.DOFillAmount(progress, 0.5f);
		}
	}
}