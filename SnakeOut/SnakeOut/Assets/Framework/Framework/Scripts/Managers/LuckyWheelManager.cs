using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Framework
{
	public class LuckyWheelManager : MonoBehaviour
	{
		public bool IsMoving;

		[Header("UI References")]
		public RectTransform arrow;
		public TextMeshProUGUI multiplierText;
		public Button claimButton;

		[Header("Settings")]
		public int[] multipliers;                 // Example: {1,2,3,4,1}
		public RectTransform[] slots;             // Slot UI elements
		public float moveSpeed = 300f;

		[Header("Slot Scale Settings")]
		public float normalScale = 1f;
		public float highlightedScale = 1.25f;
		public float scaleSpeed = 10f;

		[Header("Reward")]
		public int baseReward = 100;
		public System.Action<int> OnRewardCalculated;

		public int InitialReward { get; private set; }
		private bool movingRight = true;
		private int currentIndex = 0;
		private float leftLimit;
		private float rightLimit;

		void Start()
		{
			// 🔥 AUTO-CALCULATE LIMITS BASED ON FIRST AND LAST SLOT
			leftLimit = slots[0].anchoredPosition.x;
			rightLimit = slots[slots.Length - 1].anchoredPosition.x;

			claimButton.onClick.AddListener(StopAndReward);
			UpdateMultiplierText();
		}

		void Update()
		{
			if (!IsMoving) return;

			MoveArrowPingPong();
			HighlightCurrentSlot();
			UpdateMultiplierIndex();
			UpdateMultiplierText();
		}

		// ------------------------------------------------------------
		// 🔥 NEW MOVEMENT — USING AUTO CALCULATED LIMITS
		// ------------------------------------------------------------
		void MoveArrowPingPong()
		{
			Vector2 pos = arrow.anchoredPosition;

			if (movingRight)
			{
				pos.x += moveSpeed * Time.deltaTime;

				if (pos.x >= rightLimit)
				{
					pos.x = rightLimit;
					movingRight = false;
				}
			}
			else
			{
				pos.x -= moveSpeed * Time.deltaTime;

				if (pos.x <= leftLimit)
				{
					pos.x = leftLimit;
					movingRight = true;
				}
			}

			arrow.anchoredPosition = pos;
		}

		// ------------------------------------------------------------
		// 🔥 Highlight slot under the arrow
		// ------------------------------------------------------------
		void HighlightCurrentSlot()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				float targetScale = (i == currentIndex) ? highlightedScale : normalScale;

				slots[i].localScale = Vector3.Lerp(
					slots[i].localScale,
					Vector3.one * targetScale,
					Time.deltaTime * scaleSpeed
				);
			}
		}

		// ------------------------------------------------------------

		void UpdateMultiplierIndex()
		{
			float arrowX = arrow.anchoredPosition.x;

			for (int i = 0; i < slots.Length; i++)
			{
				float slotX = slots[i].anchoredPosition.x;

				if (Mathf.Abs(arrowX - slotX) < 30f)
				{
					currentIndex = i;
					break;
				}
			}
		}

		void UpdateMultiplierText()
		{
			multiplierText.text = "Get " + multipliers[currentIndex] + "X";
		}

		void StopAndReward()
		{
			IsMoving = false;

			int finalMultiplier = multipliers[currentIndex];
			int finalReward = baseReward * finalMultiplier;
			InitialReward = finalMultiplier;
			Debug.Log("Reward Won = " + finalReward);

			OnRewardCalculated?.Invoke(finalReward);
		}

		public void RestartWheel()
		{
			IsMoving = true;
		}
	}
}