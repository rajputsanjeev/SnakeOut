
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class TicTacReveal : MonoBehaviour
	{
		public GameObject FruitSpritePrefab;
		public RectTransform PrefabContainer;
		public TextMeshProUGUI RevealCount;

		public void SetFruitSprites(int count, Sprite sprite)
		{
			PrefabContainer.DestroyChild();
			for (int i = 0; i < count; i++)
			{
				var obj = Instantiate(FruitSpritePrefab, PrefabContainer);
				obj.GetComponent<Image>().sprite = sprite;
			}
		}

		public void SetText(int required, int current)
		{
			RevealCount.SetText($"{current}/{required}");
		}
	}
}