using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	public class RuntimeSpinWheel : MonoBehaviour
	{
		const float POINTER_OFFSET = 90f;

		[Header("Rotation Mode")]
		public bool useDotweenRotation = true;
		private Tween spinTween;

		// ================= VISUAL =================

		[Header("Wheel Visual")]
		public int textureSize = 512;
		public Transform wheelTransform;
		public Image wheelImage;
		public RectTransform wheelRect;
		public List<Color> segmentColors;
		public List<Sprite> rewardSprites;
		public List<string> rewardTexts;
		public Material wheelMaterial; // Material to apply the texture to

		public float iconRadius = 180f;
		public float textOffset = 40f;

		[Header("TMP Text")]
		public TMP_FontAsset tmpFont;
		public int tmpFontSize = 32;
		public Color tmpTextColor = Color.white;

		// Cached visual truth
		private List<float> segmentCenterAngles = new();

		// ================= PROBABILITY =================

		[Header("Probability (Logic Only)")]
		public List<float> probabilities; // weights (e.g. 50,10,5...)

		// ================= SPIN =================

		[Header("Spin Settings")]
		public float totalSpinTime = 4f;
		public float slowDownTime = 2f;
		public float initialSpeed = 1400f;
		public float minSpeed = 80f;
		public int extraRotations = 6;

		public Action<int, string> OnSpinComplete;

		[Header("Blink Dots")]
		public bool IsColorBlink;
		public List<Image> blinkDots; // assign all dots i
		public float blinkInterval = 0.08f;

		private int currentBlinkIndex = -1;
		public Color dotOnColor = Color.white;
		public Color dotOffColor = new Color(1, 1, 1, 0.25f);
		public Sprite dotOnSprite;
		public Sprite dotOffSprite;

		[Header("Dot Layout (Editor Only)")]
		public float dotRadius = 220f;
		public float dotStartAngle = 0f; // 0 = right, 90 = top
		public bool rotateDotsOutward = false;

		private Coroutine _blinkRoutine;

		private bool isSpinning;
		private float remainingAngle;
		private float currentSpeed;
		private float elapsed;

		private int targetIndex;
		private float targetAngle;

		[Header("Tick Sound")]
		public AudioSource tickAudioSource;
		public AudioClip tickClip;

		private float lastTickAngle = -1f;
		private float anglePerSegment;
		public enum HapticMode
		{
			Off,
			Light,
			Medium,
			Heavy
		}

		[Header("Haptics")]
		public HapticMode tickHaptic = HapticMode.Light;
		public HapticMode finalHaptic = HapticMode.Heavy;

		public void SetData(List<Sprite> rewardSprites, List<string> rewardTexts)
		{
			this.rewardSprites = rewardSprites;
			this.rewardTexts = rewardTexts;
		}

		// ================= PUBLIC API =================
		public void GenerateWheel()
		{
			int count = rewardSprites.Count;
			if (count == 0) return;

			tickAudioSource = GetComponent<AudioSource>();
			segmentCenterAngles.Clear();
			InitAngleData();

			Texture2D tex = new Texture2D(textureSize, textureSize);
			tex.filterMode = FilterMode.Bilinear;
			Vector2 center = new(textureSize / 2f, textureSize / 2f);

			float sweep = 360f / count;
			float startAngle = 0f;

			for (int i = 0; i < count; i++)
			{
				DrawSegment(tex, center, startAngle, startAngle + sweep, segmentColors[i % segmentColors.Count]);

				float midAngle = 360f - (startAngle + sweep * 0.5f);
				segmentCenterAngles.Add(midAngle);

				DrawIcon(tex, rewardSprites[i], midAngle, iconRadius);

				if (i < rewardTexts.Count)
					DrawTMPText(tex, rewardTexts[i], midAngle, iconRadius - textOffset);

				startAngle += sweep;
			}

			tex.Apply();
			wheelImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
		}

		public void StartSpin()
		{
			if (isSpinning || segmentCenterAngles.Count == 0) return;

			elapsed = 0f;
			currentSpeed = initialSpeed;
			isSpinning = true;

			// 🔥 Pick result FIRST using probability
			targetIndex = PickWeightedIndex();

			// Visual truth from generator
			float visualMid = segmentCenterAngles[targetIndex];
			targetAngle = (360f - visualMid + POINTER_OFFSET) % 360f;

			if (useDotweenRotation)
			{
				StartSpin_DOTween();
			}
			else
			{
				StartSpin_SpeedBased(); // your existing Update-based logic
			}


			// COLOR VERSION
			if (IsColorBlink)
			{
				_blinkRoutine = StartCoroutine(BlinkDots_Color());
			}
			else
			{
				_blinkRoutine = StartCoroutine(BlinkDots_Sprite());
			}
		}

		private void StartSpin_SpeedBased()
		{
			float currentZ = transform.eulerAngles.z;
			float delta = Mathf.DeltaAngle(currentZ, targetAngle);
			if (delta < 0) delta += 360f;

			remainingAngle = delta + 360f * extraRotations;
		}

		private void InitAngleData()
		{
			anglePerSegment = 360f / segmentCenterAngles.Count;
			lastTickAngle = GetPointerAngle();
		}

		private float GetPointerAngle()
		{
			// Pointer on RIGHT (3 o'clock)
			const float POINTER_OFFSET = 90f;

			float z = transform.eulerAngles.z;
			return (360f - z + POINTER_OFFSET) % 360f;
		}

		private void HandleTickSound()
		{
			float currentAngle = GetPointerAngle();

			if (lastTickAngle < 0f)
			{
				lastTickAngle = currentAngle;
				return;
			}

			float delta = Mathf.DeltaAngle(lastTickAngle, currentAngle);

			// We only care about forward rotation (clockwise)
			if (delta <= 0f)
				return;

			// How many segments crossed?
			int steps = Mathf.FloorToInt(Mathf.Abs(delta) / anglePerSegment);

			if (steps > 0)
			{
				PlayTick();
				lastTickAngle = currentAngle;
			}
		}

		private void PlayTick()
		{
			if (tickAudioSource == null || tickClip == null)
				return;

			float speedT = Mathf.Clamp01(currentSpeed / initialSpeed);
			tickAudioSource.pitch = Mathf.Lerp(0.8f, 1.2f, speedT);
			tickAudioSource.PlayOneShot(tickClip);
		}

		private void StartSpin_DOTween()
		{
			// Pick result FIRST (already done before calling this)
			float finalZ = targetAngle - 360f * extraRotations;

			// Kill previous tween if any
			spinTween?.Kill();

			isSpinning = true;

			spinTween = wheelTransform
				.DORotate(
					new Vector3(0, 0, finalZ),
					totalSpinTime,
					RotateMode.FastBeyond360
				)
				.SetEase(Ease.OutCubic).
				OnUpdate(HandleTickSound)
				.OnComplete(() =>
				{
					isSpinning = false;
					FinishSpin();
				});
		}

		private void Update()
		{
			if (!isSpinning || useDotweenRotation) return;

			elapsed += Time.fixedUnscaledDeltaTime;
			float remainingTime = totalSpinTime - elapsed;

			if (remainingTime <= slowDownTime)
			{
				currentSpeed = Mathf.Max(
					remainingAngle / Mathf.Max(remainingTime, 0.01f),
					minSpeed
				);
			}

			float step = Mathf.Min(currentSpeed * Time.deltaTime, remainingAngle);
			wheelTransform.Rotate(0, 0, -step);
			remainingAngle -= step;
			HandleTickSound();

			if (remainingAngle <= 0.01f)
				FinishSpin();
		}

		private void FinishSpin()
		{
			isSpinning = false;
			Vector3 e = wheelTransform.eulerAngles;
			e.z = targetAngle;
			wheelTransform.eulerAngles = e;

			// Stop blinking
			if (_blinkRoutine != null)
			{
				StopCoroutine(_blinkRoutine);
				_blinkRoutine = null;
			}

			Debug.Log(rewardTexts[targetIndex]);
			OnSpinComplete?.Invoke(targetIndex, rewardTexts[targetIndex]);
		}

		// ================= LOGIC =================

		private int PickWeightedIndex()
		{
			float total = 0f;
			foreach (var p in probabilities) total += p;

			float r = UnityEngine.Random.Range(0f, total);
			float sum = 0f;

			for (int i = 0; i < probabilities.Count; i++)
			{
				sum += probabilities[i];
				if (r <= sum) return i;
			}

			return probabilities.Count - 1;
		}

		// ================= DRAWING =================

		private void DrawSegment(Texture2D tex, Vector2 center, float start, float end, Color color)
		{
			int r = textureSize / 2;
			for (int x = 0; x < textureSize; x++)
			{
				for (int y = 0; y < textureSize; y++)
				{
					Vector2 point = new Vector2(x, y);
					Vector2 direction = point - center;
					float distance = direction.magnitude;

					if (distance <= r)
					{
						Vector2 d = new(x - center.x, y - center.y);
						if (d.magnitude > r) continue;

						float a = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
						if (a < 0) a += 360;
						a = 360 - a;

						if (a >= start && a < end)
							tex.SetPixel(x, y, color);

					}
					else
					{
						tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
					}
				}
			}
		}

		private void DrawIcon(Texture2D tex, Sprite sprite, float angle, float radius)
		{
			Texture2D icon = ExtractSprite(sprite);
			icon = Scale(icon, 64, 64);

			Vector2 c = new(tex.width / 2f, tex.height / 2f);
			float rad = angle * Mathf.Deg2Rad;
			Vector2 pos = c + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

			int sx = Mathf.RoundToInt(pos.x - icon.width / 2);
			int sy = Mathf.RoundToInt(pos.y - icon.height / 2);

			for (int x = 0; x < icon.width; x++)
				for (int y = 0; y < icon.height; y++)
				{
					Color col = icon.GetPixel(x, y);
					if (col.a <= 0) continue;

					int px = sx + x;
					int py = sy + y;
					if (px >= 0 && py >= 0 && px < tex.width && py < tex.height)
						tex.SetPixel(px, py, col);
				}
		}

		private void DrawTMPText(Texture2D tex, string text, float angle, float radius)
		{
			const int w = 256, h = 64;

			GameObject camGO = new("TMP_CAM");
			Camera cam = camGO.AddComponent<Camera>();
			cam.orthographic = true;
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = Color.clear;
			cam.cullingMask = 1 << 31;

			RenderTexture rt = new(w, h, 0);
			cam.targetTexture = rt;

			GameObject go = new("TMP");
			go.layer = 31;
			TextMeshPro tmp = go.AddComponent<TextMeshPro>();
			tmp.font = tmpFont;
			tmp.text = text;
			tmp.fontSize = tmpFontSize;
			tmp.color = tmpTextColor;
			tmp.alignment = TextAlignmentOptions.Center;

			camGO.transform.position = new Vector3(0, 0, -10);
			cam.Render();

			RenderTexture.active = rt;
			Texture2D t = new(w, h, TextureFormat.ARGB32, false);
			t.ReadPixels(new Rect(0, 0, w, h), 0, 0);
			t.Apply();
			RenderTexture.active = null;

			Vector2 c = new(tex.width / 2f, tex.height / 2f);
			float rad = angle * Mathf.Deg2Rad;
			Vector2 pos = c + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

			int sx = Mathf.RoundToInt(pos.x - w / 2);
			int sy = Mathf.RoundToInt(pos.y - h / 2);

			for (int x = 0; x < w; x++)
				for (int y = 0; y < h; y++)
				{
					Color col = t.GetPixel(x, y);
					if (col.a <= 0) continue;

					int px = sx + x;
					int py = sy + y;
					if (px >= 0 && py >= 0 && px < tex.width && py < tex.height)
						tex.SetPixel(px, py, col);
				}

			DestroyImmediate(go);
			DestroyImmediate(camGO);
			DestroyImmediate(rt);
			DestroyImmediate(t);
		}

		// ================= HELPERS =================

		private Texture2D ExtractSprite(Sprite s)
		{
			Rect r = s.textureRect;
			Texture2D t = new((int)r.width, (int)r.height);
			t.SetPixels(s.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height));
			t.Apply();
			return t;
		}

		private Texture2D Scale(Texture2D src, int w, int h)
		{
			Texture2D r = new(w, h);
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
					r.SetPixel(x, y, src.GetPixelBilinear((float)x / w, (float)y / h));
			r.Apply();
			return r;
		}

		private IEnumerator BlinkDots_Color()
		{
			// Initialize all dots OFF
			for (int i = 0; i < blinkDots.Count; i++)
				blinkDots[i].color = dotOffColor;

			currentBlinkIndex = 0;

			while (isSpinning)
			{
				// Turn OFF previous
				if (currentBlinkIndex >= 0)
					blinkDots[currentBlinkIndex].color = dotOffColor;

				// Move to next
				currentBlinkIndex = (currentBlinkIndex + 1) % blinkDots.Count;

				// Turn ON current
				blinkDots[currentBlinkIndex].color = dotOnColor;

				yield return new WaitForSeconds(blinkInterval);
			}

			// Reset when spin ends
			for (int i = 0; i < blinkDots.Count; i++)
				blinkDots[i].color = dotOnColor;
		}

		private IEnumerator BlinkDots_Sprite()
		{
			for (int i = 0; i < blinkDots.Count; i++)
				blinkDots[i].sprite = dotOffSprite;

			currentBlinkIndex = 0;

			while (isSpinning)
			{
				if (currentBlinkIndex >= 0)
					blinkDots[currentBlinkIndex].sprite = dotOffSprite;

				currentBlinkIndex = (currentBlinkIndex + 1) % blinkDots.Count;

				blinkDots[currentBlinkIndex].sprite = dotOnSprite;

				yield return new WaitForSeconds(blinkInterval);
			}

			for (int i = 0; i < blinkDots.Count; i++)
				blinkDots[i].sprite = dotOnSprite;
		}

		public void AlignDotsInCircle()
		{
			if (blinkDots == null || blinkDots.Count == 0)
			{
				Debug.LogWarning("No dots assigned");
				return;
			}

			int count = blinkDots.Count;
			float step = 360f / count;

			for (int i = 0; i < count; i++)
			{
				float angle = dotStartAngle - step * i;
				float rad = angle * Mathf.Deg2Rad;

				Vector2 pos = new Vector2(
					Mathf.Cos(rad),
					Mathf.Sin(rad)
				) * dotRadius;

				RectTransform rt = blinkDots[i].rectTransform;
				rt.SetParent(wheelRect, false);
				rt.anchoredPosition = pos;

				if (rotateDotsOutward)
				{
					rt.localRotation = Quaternion.Euler(0, 0, angle);
				}
				else
				{
					rt.localRotation = Quaternion.identity;
				}
			}
		}
	}
}
