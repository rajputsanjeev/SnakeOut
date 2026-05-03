
using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework;
using UnityEngine;

public enum StartPoint
{
	Center,
	TopLeft,
	TopRight,
	BottomLeft,
	BottomRight
}

public class TransformWaveAnimation : MonoBehaviour
{
	public Action OnComplete { set; get; }
	public Transform Parent;

	[Header("Grid Settings")]
	public GameObject dotPrefab; // 3D prefab (cube, sphere, etc.)

	[Header("Animation Settings")]
	public float duration = 0.4f;
	public float delayMultiplier = 0.03f;
	public Ease ease = Ease.OutBack;
	public AudioClip PositiveSound;

	private List<Transform> dots = new List<Transform>();
	private int columns;
	private int rows;
	private int spacing;

	void GenerateGridDots()
	{
		if (dotPrefab == null) return;

		var gridDotParent = Instantiate(new GameObject("GridDotParent"), Parent);
		for (int y = 0; y < LevelController.LevelRepresentation.LevelData.height; y++)
		{
			for (int x = 0; x < LevelController.LevelRepresentation.LevelData.width; x++)
			{
				Vector2Int gridPos = new Vector2Int(x, y);
				Vector3 worldPos = GridToWorld(gridPos);

				GameObject dot = Instantiate(
					dotPrefab,
					worldPos,
					Quaternion.identity,
					gridDotParent.transform
				);

				dot.name = $"GridDot_{x}_{y}";
				dots.Add(dot.GetComponent<Transform>());
			}
		}
	}

	public Vector3 GridToWorld(Vector2Int gridPosition)
	{
		return new Vector3(gridPosition.x, gridPosition.y, 0);
	}

	public void PlayAnimation(StartPoint startPoint, List<Transform> dotsargs)
	{
		//GenerateGridDots();

		columns = LevelController.LevelRepresentation.LevelData.height;
		rows = LevelController.LevelRepresentation.LevelData.width;

		Vector2Int origin = GetOrigin(startPoint);

		var config = new WaveAnimationUtility3D.RadialWaveConfig();
		config.PeakScaleMultiplier = 0.5f;
		config.RestScale = Vector3.one;
		Framework.Core.AudioController.PlaySound(PositiveSound);

		WaveAnimationUtility3D.PlayRipplePulseAnimation(
			   dotsargs,
			   origin,
			   gridWidth: rows,
			   config
			 );
	}

	private Vector2Int GetOrigin(StartPoint point)
	{
		int maxX = rows - 1;
		int maxY = columns - 1;

		return point switch
		{
			StartPoint.Center => new Vector2Int(maxX / 2, maxY / 2),
			StartPoint.TopLeft => new Vector2Int(0, maxY),
			StartPoint.TopRight => new Vector2Int(maxX, maxY),
			StartPoint.BottomLeft => new Vector2Int(0, 0),
			StartPoint.BottomRight => new Vector2Int(maxX, 0),
			_ => new Vector2Int(maxX / 2, maxY / 2)
		};
	}
}