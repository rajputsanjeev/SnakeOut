using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;         // <-- New Input System
using UnityEngine.InputSystem.EnhancedTouch; // <-- Touch support
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Frameork;

namespace ArrowOut
{
	public class CameraController : MonoBehaviour
	{
		// ================= SETTINGS =================
		[Header("Zoom")]
		public float zoomSpeed = 2f;
		public float zoomSmoothTime = 0.1f;
		public float minZoomMultiplier = 0.3f;

		[Header("Intro Auto Zoom")]
		public bool enableIntroZoom = true;
		public float introZoomDelay = 1.5f;
		[Range(0f, 1f), Tooltip("0 is fully zoomed out (0%), 1 is fully zoomed in (100%)")]
		public float introZoomTargetFill = 0.5f;

		[Header("Pan")]
		public float panSpeed = 0.5f;
		public float panSmoothTime = 0.1f;

		[Header("Boundary Margin")]
		public float boundaryMargin = 0.5f;

		[Header("UI Zoom Buttons")]
		public Button zoomInButton;
		public Button zoomOutButton;
		[Tooltip("How much each button press zooms in/out")]
		public float uiZoomStep = 1f;
		[Tooltip("How fast zoom changes while button is held")]
		public float uiZoomHoldSpeed = 3f;

		[Header("UI Zoom Progress")]
		public Image zoomProgressFillImage;
		public TextMeshProUGUI zoomProgressText;

		// ================= RUNTIME =================
		Camera cam;
		Bounds gridBounds;
		float initialOrthoSize;
		float targetOrthoSize;
		float zoomVelocity;

		Vector3 targetPosition;
		Vector3 panVelocity;

		bool isPanning;
		Vector3 lastPanWorldPos;
		float lastPinchDistance;

		bool isZoomInHeld;
		bool isZoomOutHeld;

		bool is3D;

		// ================= INPUT GUARDS (for RaycastController) =================
		private static float zoomCooldownTimer;
		private const float ZOOM_COOLDOWN = 0.15f; // seconds after any zoom input ends

		private static float panCooldownTimer;
		private const float PAN_COOLDOWN = 0.1f;  // seconds after pan ends

		/// <summary>
		/// True while any zoom input is active (pinch, scroll, UI buttons)
		/// OR within the cooldown window after it ends.
		/// </summary>
		public static bool IsZooming => zoomCooldownTimer > 0f;

		/// <summary>
		/// True while a pan gesture is active (touch drag, right/middle mouse)
		/// OR within the cooldown window after it ends.
		/// </summary>
		public static bool IsPanning => panCooldownTimer > 0f;

		// ================= LIFECYCLE =================
		void OnEnable()
		{
			EnhancedTouchSupport.Enable();
			MyEventArgs.GameControllerEvents.OnLevelWin.AddListener(ResetCamera);
		}

		void OnDisable()
		{
			EnhancedTouchSupport.Disable();
		}

		public void SetUp()
		{
			cam = GetComponent<Camera>();
			if (cam == null)
				cam = Camera.main;
			SetupUIButtons();
			Invoke(nameof(Setup), 0.05f);
		}

		void SetupUIButtons()
		{
			if (zoomInButton != null)
			{
				zoomInButton.onClick.AddListener(() => ZoomStep(-uiZoomStep));

				var inTrigger = zoomInButton.gameObject.AddComponent<UIButtonHoldTrigger>();
				inTrigger.OnHeld = () => isZoomInHeld = true;
				inTrigger.OnReleased = () => isZoomInHeld = false;
			}

			if (zoomOutButton != null)
			{
				zoomOutButton.onClick.AddListener(() => ZoomStep(uiZoomStep));

				var outTrigger = zoomOutButton.gameObject.AddComponent<UIButtonHoldTrigger>();
				outTrigger.OnHeld = () => isZoomOutHeld = true;
				outTrigger.OnReleased = () => isZoomOutHeld = false;
			}
		}

		private Coroutine introZoomCoroutine;

		void Setup()
		{
			if (GridManager.Instance == null) return;

			gridBounds = GridManager.Instance.GetGridWorldBounds();
			is3D = GridManager.Instance.renderMode == GameRenderMode.Mesh3D;

			// ------------------------------------------------------------------
			// Compute the orthographic size required to fit the ENTIRE grid,
			// taking both axes AND the camera's aspect ratio into account.
			//
			//   sizeFromHeight  = half the grid's vertical extent (+ margin)
			//   sizeFromWidth   = half the grid's horizontal extent (+ margin)
			//                     divided by aspect so it maps to ortho-height
			//
			// We take whichever axis needs MORE room, so no edge is ever clipped
			// regardless of grid shape or screen orientation.
			// ------------------------------------------------------------------
			float gridHalfH = is3D
				? (gridBounds.size.z * 0.5f)
				: (gridBounds.size.y * 0.5f);
			float gridHalfW = gridBounds.size.x * 0.5f;

			float sizeFromHeight = gridHalfH + boundaryMargin;
			float sizeFromWidth = (gridHalfW + boundaryMargin) / cam.aspect;

			initialOrthoSize = Mathf.Max(sizeFromHeight, sizeFromWidth);

			// Compute the grid center we want to pan/zoom to
			Vector3 gridCenter = is3D
				? new Vector3(gridBounds.center.x, cam.transform.position.y, gridBounds.center.z)
				: new Vector3(gridBounds.center.x, gridBounds.center.y, cam.transform.position.z);

			// Only snap on the very first setup (no valid state yet).
			// On resets, only update targets so SmoothDamp plays a smooth transition.
			bool isFirstSetup = cam.orthographicSize <= 0;
			if (isFirstSetup)
			{
				cam.orthographicSize = initialOrthoSize;
				cam.transform.position = gridCenter;
			}

			// Set targets — Update() SmoothDamp will animate toward these
			targetOrthoSize = initialOrthoSize;
			targetPosition = gridCenter;

			// Reset velocities so each smooth transition starts clean
			zoomVelocity = 0f;
			panVelocity = Vector3.zero;
		}

		public void StartIntroZoom()
		{
			if (enableIntroZoom)
			{
				if (introZoomCoroutine != null) StopCoroutine(introZoomCoroutine);
				introZoomCoroutine = StartCoroutine(IntroZoomSequence());
			}
		}

		IEnumerator IntroZoomSequence()
		{
			yield return new WaitForSeconds(introZoomDelay);

			float minOrtho = initialOrthoSize * minZoomMultiplier;
			float maxOrtho = initialOrthoSize;

			// desiredSize lerps between max (0% progress) and min (100% progress)
			float desiredSize = Mathf.Lerp(maxOrtho, minOrtho, introZoomTargetFill);
			targetOrthoSize = desiredSize;
		}

		// ================= UPDATE =================
		void Update()
		{
			if (cam == null || GridManager.Instance == null || initialOrthoSize <= 0) return;

			HandleZoom();
			HandlePan();
			ClampCamera();
			UpdateZoomProgressUI();
		}

		// ================= ZOOM =================
		void HandleZoom()
		{
			// Mouse scroll wheel
			var mouse = Mouse.current;
			if (mouse != null)
			{
				float scrollDelta = mouse.scroll.ReadValue().y;
				if (Mathf.Abs(scrollDelta) > 0.01f)
				{
					ApplyZoomDelta(-scrollDelta * zoomSpeed * 0.01f); // scroll values are ~120 units per notch
					zoomCooldownTimer = ZOOM_COOLDOWN; // suppress raycasts during scroll zoom
				}
			}

			// Touch pinch (requires EnhancedTouch)
			var activeTouches = Touch.activeTouches;
			if (activeTouches.Count >= 2)
			{
				zoomCooldownTimer = ZOOM_COOLDOWN;
				if (activeTouches.Count == 2)
					HandlePinchZoom(activeTouches[0], activeTouches[1]);
			}
			else
			{
				if (zoomCooldownTimer > 0f)
					zoomCooldownTimer -= Time.deltaTime;
			}

			// UI button hold — also counts as zooming for raycast suppression
			if (isZoomInHeld)
			{
				zoomCooldownTimer = ZOOM_COOLDOWN;
				ApplyZoomDelta(-uiZoomHoldSpeed * Time.deltaTime);
			}
			else if (isZoomOutHeld)
			{
				zoomCooldownTimer = ZOOM_COOLDOWN;
				ApplyZoomDelta(uiZoomHoldSpeed * Time.deltaTime);
			}

			// Smooth zoom
			if (cam.orthographic)
				cam.orthographicSize = Mathf.SmoothDamp(
					cam.orthographicSize, targetOrthoSize, ref zoomVelocity, zoomSmoothTime);
		}

		void HandlePinchZoom(Touch t0, Touch t1)
		{
			float currentDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);

			if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
			{
				lastPinchDistance = currentDist;
			}
			else
			{
				ApplyZoomDelta((lastPinchDistance - currentDist) * zoomSpeed * 0.01f);
				lastPinchDistance = currentDist;
			}
		}

		void ApplyZoomDelta(float delta)
		{
			targetOrthoSize = ClampZoom(targetOrthoSize + delta);
		}

		void ZoomStep(float step)
		{
			targetOrthoSize = ClampZoom(targetOrthoSize + step);
		}

		float ClampZoom(float size)
		{
			return Mathf.Clamp(size, initialOrthoSize * minZoomMultiplier, initialOrthoSize);
		}

		// ================= PAN =================
		void HandlePan()
		{
			var mouse = Mouse.current;
			var activeTouches = Touch.activeTouches;
			bool touchPan = activeTouches.Count == 1;

			// --- Begin pan ---
			if (mouse != null &&
				(mouse.rightButton.wasPressedThisFrame || mouse.middleButton.wasPressedThisFrame))
			{
				isPanning = true;
				lastPanWorldPos = GetMouseWorldPos(mouse);
			}
			else if (touchPan && activeTouches[0].phase == TouchPhase.Began)
			{
				isPanning = true;
				lastPanWorldPos = GetTouchWorldPos(activeTouches[0]);
			}

			// --- Continue / end pan ---
			if (isPanning)
			{
				panCooldownTimer = PAN_COOLDOWN; // keep IsPanning true while dragging

				Vector3 currentWorldPos;

				if (touchPan)
				{
					currentWorldPos = GetTouchWorldPos(activeTouches[0]);
					if (activeTouches[0].phase == TouchPhase.Ended ||
						activeTouches[0].phase == TouchPhase.Canceled)
						isPanning = false;
				}
				else if (mouse != null &&
						 (mouse.rightButton.isPressed || mouse.middleButton.isPressed))
				{
					currentWorldPos = GetMouseWorldPos(mouse);
				}
				else
				{
					isPanning = false;
					goto SmoothPan;
				}

				targetPosition += lastPanWorldPos - currentWorldPos;
				lastPanWorldPos = currentWorldPos;
			}
			else
			{
				// Countdown cooldown after pan ends
				if (panCooldownTimer > 0f)
					panCooldownTimer -= Time.deltaTime;
			}

		SmoothPan:
			cam.transform.position = Vector3.SmoothDamp(
				cam.transform.position, targetPosition, ref panVelocity, panSmoothTime);
		}

		// ================= CLAMP =================
		void ClampCamera()
		{
			if (!cam.orthographic) return;

			float vertExtent = cam.orthographicSize;
			float horizExtent = vertExtent * cam.aspect;
			Vector3 pos = targetPosition;

			float minX = gridBounds.min.x - boundaryMargin + horizExtent;
			float maxX = gridBounds.max.x + boundaryMargin - horizExtent;

			pos.x = minX > maxX
				? (gridBounds.min.x + gridBounds.max.x) * 0.5f
				: Mathf.Clamp(pos.x, minX, maxX);

			if (is3D)
			{
				float minZ = gridBounds.min.z - boundaryMargin + vertExtent;
				float maxZ = gridBounds.max.z + boundaryMargin - vertExtent;

				pos.z = minZ > maxZ
					? (gridBounds.min.z + gridBounds.max.z) * 0.5f
					: Mathf.Clamp(pos.z, minZ, maxZ);
			}
			else
			{
				float minY = gridBounds.min.y - boundaryMargin + vertExtent;
				float maxY = gridBounds.max.y + boundaryMargin - vertExtent;

				pos.y = minY > maxY
					? (gridBounds.min.y + gridBounds.max.y) * 0.5f
					: Mathf.Clamp(pos.y, minY, maxY);
			}

			targetPosition = pos;
		}

		// ================= PROGRESS UI =================
		void UpdateZoomProgressUI()
		{
			if (zoomProgressFillImage == null && zoomProgressText == null) return;

			// calculate the active range for Zoom (initial is largest size, minZoomMultiplier is smallest size)
			float minOrtho = initialOrthoSize * minZoomMultiplier;
			float maxOrtho = initialOrthoSize;
			float range = maxOrtho - minOrtho;

			if (range <= 0) return;

			// Reverse so that most zoomed in (current size == minOrtho) is 100%, and most zoomed out is 0%
			float zoomPercentage = (maxOrtho - cam.orthographicSize) / range;
			zoomPercentage = Mathf.Clamp01(zoomPercentage); // Ensure it's exactly between 0 and 1

			if (zoomProgressFillImage != null)
			{
				zoomProgressFillImage.fillAmount = zoomPercentage;
			}

			if (zoomProgressText != null)
			{
				zoomProgressText.text = $"{(zoomPercentage * 100f):F0}%";
			}
		}

		// ================= HELPERS =================
		Vector3 GetMouseWorldPos(Mouse mouse)
		{
			Vector3 pos = mouse.position.ReadValue();
			pos.z = Mathf.Abs(cam.transform.position.z);
			return cam.ScreenToWorldPoint(pos);
		}

		Vector3 GetTouchWorldPos(Touch touch)
		{
			Vector3 pos = touch.screenPosition;
			pos.z = Mathf.Abs(cam.transform.position.z);
			return cam.ScreenToWorldPoint(pos);
		}

		// ================= PUBLIC API =================
		// ================= PUBLIC API =================
		public void ResetCamera()
		{
			Setup();
			targetOrthoSize = initialOrthoSize;

			Vector3 center = is3D
				? new Vector3(
					(gridBounds.min.x + gridBounds.max.x) * 0.5f,
					cam.transform.position.y,
					(gridBounds.min.z + gridBounds.max.z) * 0.5f)
				: new Vector3(
					(gridBounds.min.x + gridBounds.max.x) * 0.5f,
					(gridBounds.min.y + gridBounds.max.y) * 0.5f,
					cam.transform.position.z);

			targetPosition = center;
			cam.orthographicSize = 5;

			gameObject.SetActive(false);
		}

		public void OnDestroy()
		{
			MyEventArgs.GameControllerEvents.OnLevelWin.RemoveListener(ResetCamera);
		}
	}
}