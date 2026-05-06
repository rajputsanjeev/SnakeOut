using UnityEngine;

namespace ArrowOut
{
	/// <summary>
	/// Handles coordinate conversion between grid space and world space
	/// Single Responsibility: Coordinate transformation
	/// </summary>
	public class CoordinateConverter : ICoordinateConverter
	{
		private readonly GameRenderMode renderMode;
		private readonly Camera mainCamera;
		private readonly float spacing;

		public CoordinateConverter(GameRenderMode mode, Camera camera, float spacing = 1f)
		{
			renderMode = mode;
			mainCamera = camera;
			this.spacing = spacing;
		}

		public Vector3 GridToWorld(Vector2Int gridPosition)
		{
			switch (renderMode)
			{
				case GameRenderMode.Mesh3D:
				case GameRenderMode.LineRenderer3D:
					return new Vector3(gridPosition.x * spacing, 0, gridPosition.y * spacing);
				default:
					return new Vector3(gridPosition.x * spacing, gridPosition.y * spacing, 0);
			}
		}

		public Vector2Int WorldToGrid(Vector3 worldPosition)
		{
			switch (renderMode)
			{
				case GameRenderMode.Mesh3D:
				case GameRenderMode.LineRenderer3D:
					return new Vector2Int(
						Mathf.RoundToInt(worldPosition.x / spacing),
						Mathf.RoundToInt(worldPosition.z / spacing)
					);
				default:
					return new Vector2Int(
						Mathf.RoundToInt(worldPosition.x / spacing),
						Mathf.RoundToInt(worldPosition.y / spacing)
					);
			}
		}

		public Vector2Int ScreenToGrid(Vector2 screenPosition)
		{
			switch (renderMode)
			{
				case GameRenderMode.Mesh3D:
				case GameRenderMode.LineRenderer3D:
					return ScreenToGrid3D(screenPosition);
				default:
					return ScreenToGrid2D(screenPosition);
			}
		}

		private Vector2Int ScreenToGrid2D(Vector2 screenPosition)
		{
			Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
			return WorldToGrid(worldPos);
		}

		private Vector2Int ScreenToGrid3D(Vector2 screenPosition)
		{
			Ray ray = mainCamera.ScreenPointToRay(screenPosition);
			Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

			if (groundPlane.Raycast(ray, out float enter))
			{
				Vector3 hitPoint = ray.GetPoint(enter);
				return WorldToGrid(hitPoint);
			}

			return new Vector2Int(-1, -1);
		}
	}
}
