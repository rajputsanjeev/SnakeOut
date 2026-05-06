using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using Framework.Core;

namespace Watermelon
{
	public class TimerVisualiser : MonoBehaviour
	{
		[SerializeField] TMP_Text timerText;
		private GameplayTimer timer;

		[SerializeField] SlicedFilledImage fillImage;
		[SerializeField] RectTransform _timeContainer;

		public void Init()
		{
		}

		public void SetTimeContainer(bool isActive)
		{
			_timeContainer.gameObject.SetActive(isActive);
		}

		public void Show(GameplayTimer timer)
		{
			this.timer = timer;

			gameObject.SetActive(true);

			timer.OnTimeSpanChanged += OnTimeChanged;

			OnTimeChanged(timer.CurrentTimeSpan);
		}

		private void OnDestroy()
		{
			if (timer != null)
				timer.OnTimeSpanChanged -= OnTimeChanged;
		}

		public void Hide()
		{
			gameObject.SetActive(false);

			if (timer != null)
				timer.OnTimeSpanChanged -= OnTimeChanged;
		}

		public void SetFreezeFillAmount(float t)
		{
			fillImage.fillAmount = t;
		}

		public void OnTimeChanged(TimeSpan timeSpan)
		{
			timerText.text = string.Format("{0:mm\\:ss}", timeSpan);
		}
	}
}
