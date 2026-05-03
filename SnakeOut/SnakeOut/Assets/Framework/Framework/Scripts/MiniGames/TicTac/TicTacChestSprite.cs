using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class TicTacChestSprite : MonoBehaviour
	{
		public Image ChestImage;
		public GameObject RingImage;

		public void SetSprite(Sprite chestSprite)
		{
			ChestImage.sprite = chestSprite;
		}

		public void EnableRing(bool isEnable)
		{
			RingImage.gameObject.SetActive(isEnable);
		}
	}
}

