using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// Static utility to apply a distance‑based wave animation to any collection of RectTransforms.
/// </summary>
public static class WaveAnimationUtility
{
    /// <summary>
    /// Plays a wave animation where elements scale/fade in from a given origin point.
    /// </summary>
    /// <param name="elements">List of RectTransforms to animate.</param>
    /// <param name="origin">World/anchor position from which the wave expands.</param>
    /// <param name="duration">Duration of each individual tween.</param>
    /// <param name="delayMultiplier">Delay per unit distance from origin.</param>
    /// <param name="startScale">Initial scale.</param>
    /// <param name="endScale">Final scale.</param>
    /// <param name="startAlpha">Initial alpha.</param>
    /// <param name="endAlpha">Final alpha.</param>
    /// <param name="ease">Easing for the scale animation.</param>
    public static void PlayWaveAnimation(
        IList<RectTransform> elements,
        Vector2 origin,
        float duration = 0.4f,
        float delayMultiplier = 0.03f,
        Vector3? startScale = null,
        Vector3? endScale = null,
        float startAlpha = 0f,
        float endAlpha = 1f,
        Ease ease = Ease.OutBack)
    {
        if (elements == null || elements.Count == 0) return;

        Vector3 actualStartScale = startScale ?? Vector3.zero;
        Vector3 actualEndScale   = endScale   ?? Vector3.one;

        // Cache CanvasGroups to avoid repeated GetComponent calls
        var canvasGroups = new Dictionary<RectTransform, CanvasGroup>();

        for (int i = 0; i < elements.Count; i++)
        {
            RectTransform element = elements[i];
            if (element == null) continue;

            // Ensure a CanvasGroup exists (add if missing)
            if (!canvasGroups.TryGetValue(element, out CanvasGroup cg))
            {
                cg = element.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = element.gameObject.AddComponent<CanvasGroup>();
                canvasGroups[element] = cg;
            }

            float distance = Vector2.Distance(element.anchoredPosition, origin);
            float delay = distance * delayMultiplier;

            // Reset before starting new animation
            element.localScale = actualStartScale;
            cg.alpha = startAlpha;

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.Append(element.DOScale(actualEndScale, duration).SetEase(ease));
            seq.Join(cg.DOFade(endAlpha, duration));
            seq.SetTarget(element); // Allows killing tweens per element if needed
        }
    }
}