using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public abstract class ProgressBar<T> : MonoBehaviour
	{
		public static ProgressBar<T> Instance;
		protected T maxValue;
		[SerializeField] protected Image m_Slider;
		[SerializeField] protected TextMeshProUGUI m_PercentageText;

		private void Awake()
		{
			Instance = this;
		}

		public void Init(T total)
		{
			Instance.maxValue = total;
		}

		public void Value(T value)
		{
			Instance.ShowProgress(value);
		}

		protected abstract void ShowProgress(T value);
	}
}