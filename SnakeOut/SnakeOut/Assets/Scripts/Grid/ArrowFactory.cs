using System.Collections.Generic;
using UnityEngine;
using Watermelon;

namespace ArrowOut
{
	/// <summary>
	/// Factory Pattern: Creates arrow components based on render mode
	/// Single Responsibility: Object creation logic
	/// </summary>
	public class ArrowFactory
	{
		private readonly GameRenderMode renderMode;
		private readonly GridManager gridManager;

		public ArrowFactory(GameRenderMode mode, GridManager manager)
		{
			renderMode = mode;
			gridManager = manager;
		}

		/// <summary>
		/// Creates an arrow head based on current render mode
		/// </summary>
		public IArrowHead CreateArrowHead(Vector2Int position, Transform parent, Color color, IClickableObjectRenderer clickableObjectRenderer, Vector3 rotationOffset = default)
		{
			GameObject headObj = new GameObject("ArrowHead");
			IArrowHead head = null;

			switch (renderMode)
			{
				case GameRenderMode.LineRenderer2D:
				case GameRenderMode.SpriteMesh2D:
					SpriteArrowHead spriteHead = headObj.AddComponent<SpriteArrowHead>();
					var colliderClick = headObj.AddComponent<ColliderClickForwarder>();
					colliderClick.Initialize(clickableObjectRenderer);
					spriteHead.Setup(gridManager.arrowHeadSprite, color);
					head = spriteHead;
					break;

				case GameRenderMode.Mesh3D:
				case GameRenderMode.LineRenderer3D:
					MeshArrowHead meshHead = headObj.AddComponent<MeshArrowHead>();
					var colliderClick3D = headObj.AddComponent<ColliderClickForwarder>();
					colliderClick3D.Initialize(clickableObjectRenderer);
					meshHead.Initialize(position, parent, rotationOffset);
					meshHead.Setup(GridManager.Instance.snakeHeadPrefab);
					head = meshHead;
					break;
			}

			return head;
		}

		/// <summary>
		/// Creates an arrow body renderer based on current render mode
		/// </summary>
		public IArrowRenderer CreateArrowRenderer(Transform parent, List<Vector2Int> path, Color color, IArrowInputHandler arrow)
		{
			GameObject rendererObj = new GameObject("ArrowBody");
			IArrowRenderer renderer = null;

			switch (renderMode)
			{
				case GameRenderMode.LineRenderer2D:
					renderer = rendererObj.AddComponent<LineRenderer2DArrow>();
					break;

				case GameRenderMode.SpriteMesh2D:
					renderer = rendererObj.AddComponent<SpriteMesh2DArrow>();
					break;

				case GameRenderMode.Mesh3D:
					renderer = rendererObj.AddComponent<TubeMesh3DArrow>();
					break;

				case GameRenderMode.LineRenderer3D:
					renderer = rendererObj.AddComponent<LineRenderer3DArrow>();
					break;
			}

			if (renderer != null)
			{
				renderer.Initialize(parent, path, color);
			}

			return renderer;
		}

		/// <summary>
		/// Creates movement validator
		/// </summary>
		public IMovementValidator CreateMovementValidator()
		{
			return new MovementValidator(gridManager);
		}

		/// <summary>
		/// Creates coordinate converter
		/// </summary>
		public ICoordinateConverter CreateCoordinateConverter(Camera camera, float spacing = 1f)
		{
			return new CoordinateConverter(renderMode, camera, spacing);
		}
	}
}
