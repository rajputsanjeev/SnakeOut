using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour
{
	public int rows = 4;
	public int columns = 4;
	public float spacingX = 10f;
	public float spacingY = 10f;
	public GameObject cellPrefab;

	public RectTransform rectTransform;

	void Start()
	{
		//rectTransform = GetComponent<RectTransform>();
		GenerateGrid();
	}

	void GenerateGrid()
	{
		// Clear existing cells if any
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		float cellWidth = (rectTransform.rect.width - (columns - 1) * spacingX) / columns;
		float cellHeight = (rectTransform.rect.height - (rows - 1) * spacingY) / rows;
		float startX = -rectTransform.rect.width / 2 + cellWidth / 2;
		float startY = rectTransform.rect.height / 2 - cellHeight / 2;

		//float cellSize = Mathf.Min(cellWidth, cellHeight);

		//for (int i = 0; i < rows; i++)
		//{
		//	for (int j = 0; j < columns; j++)
		//	{
		//		GameObject cell = Instantiate(cellPrefab, transform);
		//		RectTransform cellRect = cell.GetComponent<RectTransform>();

		//		cellRect.sizeDelta = new Vector2(cellSize, cellSize);

		//		float posX = startX + j * (cellWidth + spacingX);
		//		float posY = startY - i * (cellHeight + spacingY);

		//		cellRect.anchoredPosition = new Vector2(posX, posY);
		//	}
		//}

		//var cellWidth = (rectTransform.rect.width - (_currentLevel.CurrentLayerData.width - 1) * spacingX) / _currentLevel.CurrentLayerData.width;
		//var cellHeight = (rectTransform.rect.height - (_currentLevel.CurrentLayerData.height - 1) * spacingY) / _currentLevel.CurrentLayerData.height;

		//var startX = -TileAreaRect.rect.width / 2 + cellWidth / 2;
		//var startY = TileAreaRect.rect.height / 2 - cellHeight / 2;


		var tileDistanceX = (int)cellWidth + spacingX;
		var tileDistanceY = (int)cellHeight + spacingY;

		// Spawn tiles
		for (var posY = 0; posY <= rows; posY++)
		{
			for (var posX = 0; posX <= columns; posX++)
			{
				var tileInstance = Instantiate(cellPrefab, rectTransform);
				tileInstance.name = $"Tile ({posY - 1}, {posX - 1})";

				var tileRect = tileInstance.GetComponent<RectTransform>();
				tileRect.sizeDelta = new Vector2(cellWidth, cellHeight);

				var posTitleX = startX + (posX - 1) * (cellWidth + spacingX);
				var posTitleY = startY - (posY - 1) * (cellHeight + spacingY);
				tileRect.anchoredPosition = new Vector2(posTitleX, posTitleY);
			}
		}
	}
}