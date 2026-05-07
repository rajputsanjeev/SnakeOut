using UnityEngine;

namespace ArrowOut
{
	/// <summary>
	/// 3D Mesh-based arrow head
	/// </summary>
	public class MeshArrowHead : ArrowHeadBase
	{
		private MeshRenderer meshRenderer;
		private Material material;
		private IArrowInputHandler parentArrow;
		private Transform _parent;

		public override void Initialize(Vector2Int position, Transform parent, Vector3 rotationOffset = default)
		{
			base.Initialize(position, parent, rotationOffset);
			parentArrow = parent.GetComponent<IArrowInputHandler>();
			_parent = parent;
		}

		public void Setup(GameObject headPrefab)
		{
			if (headPrefab != null)
			{
				var instance = Instantiate(headPrefab, transform);
				instance.transform.localPosition = Vector3.zero;
				instance.transform.localScale = Vector3.one * 1.5f;

				var head = instance.GetComponent<GeckoHead>();
				var arrowComp = _parent.GetComponent<Arrow>();

				if (arrowComp != null && arrowComp.Data != null && arrowComp.Data.texture != null)
				{
					for (int i = 0; i < head.GeckoMeshRender.Count; i++)
					{
						head.GeckoMeshRender[i].material = arrowComp.Data.texture;
					}
				}
			}
			else
			{
				// Create default cone
				CreateDefaultCone();
			}

			// Add collider
			SphereCollider collider = gameObject.AddComponent<SphereCollider>();
			collider.radius = 0.4f;

			if (meshRenderer != null)
			{
				material = meshRenderer.material;
			}
		}

		private void CreateDefaultCone()
		{
			GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			cone.transform.SetParent(transform);
			cone.transform.localPosition = Vector3.zero;
			cone.transform.localScale = Vector3.one * 0.4f;
			meshRenderer = cone.GetComponent<MeshRenderer>();
			Destroy(cone.GetComponent<Collider>());
		}

		public override void SetColor(Color color)
		{
			currentColor = color;
			if (material != null)
				material.color = color;
		}

		public void SetRotation(Vector3 direction)
		{
			if (direction != Vector3.zero)
				transform.rotation = Quaternion.LookRotation(direction);
		}
	}
}
