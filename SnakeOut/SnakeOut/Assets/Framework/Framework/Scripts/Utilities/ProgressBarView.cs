using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class ProgressBarView : MonoBehaviour
	{
		public string ExtraText;
		public bool IsDecrease;
		public bool IsTextByText;// This Used when user progess bar is like 1/2, 2/4
		protected float maxValue;
		[SerializeField] protected Image m_Slider;
		[SerializeField] protected TextMeshProUGUI m_PercentageText;

		public void Init(float total)
		{
			maxValue = total;
		}

		public void Value(float value)
		{
			ShowProgress(value);
		}

		protected void ShowProgress(float value)
		{
			if (IsDecrease) value = maxValue - value;

			// Smooth fill using DOTween
			m_Slider.DOFillAmount((float)value / maxValue, 0.5f).SetEase(Ease.OutQuad);

			Debug.Log("Fill Amount " + Mathf.Clamp01((float)value / maxValue));
			//m_Slider.fillAmount = Mathf.Clamp01((float)value / maxValue);

			if (m_PercentageText != null && !IsTextByText)
			{
				m_PercentageText.text = ExtraText + "" + (Mathf.Round(m_Slider.fillAmount * 100) + " %");
			}
		}

		public void SetText(string progess)
		{
			m_PercentageText.text = progess;
		}
	}
}
