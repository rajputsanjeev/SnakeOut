using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.UI.Manager;
using Framework;
using MaskTransitions;
using UnityEngine;
using Watermelon;

namespace Framework
{
	public class RewardFlowController : GenericSingletonClass<RewardFlowController>
	{
		public static Action OnFlowAnimationComplete;

		[Header("UI Defaults")]
		public RectTransform StartingPoint;
		public RectTransform EndPoint;
		public GameObject RwPrefab;

		[Header("Playback")]
		[Tooltip("Play all reward animations at the same time instead of sequentially.")]
		public bool PlaySimultaneous = false;

		[Space]
		public List<RewardTransform> RewardTransforms = new List<RewardTransform>();

		// ─────────────────────────────────────────────────────────
		// Lifecycle
		// ─────────────────────────────────────────────────────────

		private void Start()
		{
			if (!RewardFlowQueue.HasRewards) return;
			SceneLoader.AfterLoad += PlayAnim;
			TransitionManager.AfterLoad += PlayAnim;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			RewardFlowQueue.Clear();
			SceneLoader.AfterLoad -= PlayAnim;
			TransitionManager.AfterLoad -= PlayAnim;
		}

		// ─────────────────────────────────────────────────────────
		// Public API
		// ─────────────────────────────────────────────────────────

		public void PlayAnim(string sceneName) => StartCoroutine(CoPlayQueue());

		public void PlayPurchaseAnimation(List<FlowReward> flowRewards, Action onComplete = null)
			=> StartCoroutine(CoPlayPurchase(flowRewards, onComplete));

		// ─────────────────────────────────────────────────────────
		// Coroutines
		// ─────────────────────────────────────────────────────────

		private IEnumerator CoPlayQueue()
		{
			UIPanelManager.IsRewardFlowAnimation = true;

			var rewards = RewardFlowQueue.GetRewards().ToList();
			yield return StartCoroutine(CoPlayRewards(rewards));

			OnFlowAnimationComplete?.Invoke();
			UIPanelManager.IsRewardFlowAnimation = false;
			RewardFlowQueue.Clear();
		}

		private IEnumerator CoPlayPurchase(List<FlowReward> flowRewards, Action onComplete)
		{
			UIPanelManager.IsRewardFlowAnimation = true;

			yield return StartCoroutine(CoPlayRewards(flowRewards));

			// Small settle delay after animations finish
			yield return new WaitForSeconds(0.6f);

			UIPanelManager.IsRewardFlowAnimation = false;
			onComplete?.Invoke();
		}

		/// <summary>Routes to sequential or simultaneous play based on the inspector toggle.</summary>
		private IEnumerator CoPlayRewards(List<FlowReward> rewards)
		{
			if (rewards == null || rewards.Count == 0) yield break;

			if (PlaySimultaneous)
				yield return StartCoroutine(CoPlayAllTogether(rewards));
			else
				yield return StartCoroutine(CoPlayOneByOne(rewards));
		}

		/// <summary>Plays each reward animation one after another, waiting for completion.</summary>
		private IEnumerator CoPlayOneByOne(List<FlowReward> rewards)
		{
			foreach (var reward in rewards)
			{
				var rwTransform = GetPreparedTransform(reward);
				if (rwTransform == null) continue;

				bool completed = false;
				CurrencyFlow.OnAnimationComplete += OnComplete;
				CurrencyFlow.SpawnCurrency(rwTransform);
				yield return new WaitUntil(() => completed);
				CurrencyFlow.OnAnimationComplete -= OnComplete;

				void OnComplete() { completed = true; }
			}
		}

		/// <summary>Fires all reward animations simultaneously, waits until the last one finishes.</summary>
		private IEnumerator CoPlayAllTogether(List<FlowReward> rewards)
		{
			int completedCount = 0;
			int totalCount = 0;

			// Subscribe once — counts completions from any running animation
			CurrencyFlow.OnAnimationComplete += OnOneComplete;

			foreach (var reward in rewards)
			{
				var rwTransform = GetPreparedTransform(reward);
				if (rwTransform == null) continue;

				totalCount++;
				CurrencyFlow.SpawnCurrency(rwTransform);
			}

			if (totalCount == 0)
			{
				CurrencyFlow.OnAnimationComplete -= OnOneComplete;
				yield break;
			}

			yield return new WaitUntil(() => completedCount >= totalCount);
			CurrencyFlow.OnAnimationComplete -= OnOneComplete;

			void OnOneComplete() { completedCount++; }
		}

		// ─────────────────────────────────────────────────────────
		// Helpers
		// ─────────────────────────────────────────────────────────

		/// <summary>
		/// Finds the matching RewardTransform and fills in any null
		/// UI references from the controller's global defaults.
		/// </summary>
		private RewardTransform GetPreparedTransform(FlowReward reward)
		{
			var rwTransform = RewardTransforms.Find(x => x.Type == reward.RwType);
			if (rwTransform == null)
			{
				Debug.LogWarning($"[RewardFlowController] No RewardTransform found for type: {reward.RwType}");
				return null;
			}

			if (rwTransform.StartingPoint == null) rwTransform.StartingPoint = StartingPoint;
			if (rwTransform.EndPoint == null) rwTransform.EndPoint = EndPoint;
			if (rwTransform.Prefab == null) rwTransform.Prefab = RwPrefab;

			return rwTransform;
		}
	}
}