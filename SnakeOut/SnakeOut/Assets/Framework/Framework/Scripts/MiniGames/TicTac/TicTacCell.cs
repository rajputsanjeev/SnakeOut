using UnityEngine;
using UnityEngine.UI;
using Framework;
using Framework.UI.Controllers;

namespace Framework
{
	public class TicTacCell : MonoBehaviour
	{
		public Image icon;
		public Image CellImage;
		public Button button;
		public Sprite questionSprite;
		public Sprite emptySprite;
		private int index;
		private TicTacGridGenerator controller;

		public void Init(int i, TicTacGridGenerator c)
		{
			index = i;
			controller = c;
			button.onClick.AddListener(OnClick);
		}

		void OnClick()
		{
			controller.OnCellClicked(index);
		}

		public void ResetCell()
		{
			CellImage.sprite = questionSprite;
			button.interactable = true;
		}

		public void Reveal(Sprite fruit)
		{
			icon.enabled = true;
			icon.sprite = fruit;
			CellImage.sprite = emptySprite;
			button.interactable = false;
		}

		public void RevealEmpty()
		{
			CellImage.sprite = emptySprite;
			button.interactable = false;
		}
	}
}