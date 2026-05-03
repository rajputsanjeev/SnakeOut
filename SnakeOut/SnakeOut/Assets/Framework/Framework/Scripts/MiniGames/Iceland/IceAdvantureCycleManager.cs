using System;
using Framework;
using UnityEngine;

public class IceAdvantureCycleManager : DailyCycleManager<IcelandAdvantacher>
{
	public IcelandAdvantacher SaveData => save;

	protected override void Awake()
	{
		base.Awake();
	}

	#region Save Binding
	public override void Load()
	{
		save = IceLandPersistence.Load();
		base.Load();
	}

	public override void Save()
	{
		IceLandPersistence.Save(save);
	}

	public void Save(IcelandAdvantacher ticTacSaveData)
	{
		IceLandPersistence.Save(ticTacSaveData);
		save = ticTacSaveData;
		Notify();
	}
	#endregion

	#region Feature Logic
	protected override void DoDailyReset()
	{
		base.DoDailyReset();
	}

	private void ResetData()
	{
		save.IsStarted = false;
		save.CurrentStep = 0;
		save.IsComplete = false;
		save.IsRewardCollected = false;
		save.LastStep = 0;
	}

	public void ResetIceLand()
	{
		ResetData();
		Save();
		Notify();
	}
	#endregion

	#region Public API (Controller uses this)
	public void StartGame()
	{
		save.IsStarted = true;
		Save();
		Notify();
	}

	public void MarkCompleted()
	{
		save.IsComplete = true;
		save.cooldownEndUtc = Now.AddHours(24).ToString("o");
		Save();
		Notify();
	}

	protected override void OnDailyDataReset()
	{
		ResetData();
	}
	#endregion
}
