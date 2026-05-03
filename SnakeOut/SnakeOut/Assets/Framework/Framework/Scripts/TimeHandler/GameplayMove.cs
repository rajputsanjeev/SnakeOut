using System;
using Framework.Core;
using UnityEngine;

public class GameplayMove
{
	public int MaxMove { get; private set; }
	public int CurrentMove { get; private set; }
	public bool IsGameStarted { get; private set; }
	public bool IsActive { get; private set; }
	public event SimpleCallback OnMoveFinished;
	public event SimpleCallback OnTimerStart;
	public event SimpleFloatCallback OnMoveChange;
	public event SimpleIntCallback OnMoveAdded;

	public void Start()
	{
		IsActive = true;
		CurrentMove = MaxMove;
		OnTimerStart?.Invoke();
	}

	public void SetMaxMove(int moves)
	{
		MaxMove = moves;
		CurrentMove = MaxMove;
		OnMoveChange?.Invoke(CurrentMove);
	}

	public void AdjustMove(int move)
	{
		CurrentMove += move;
		OnMoveAdded?.Invoke(move);
	}

	public void Substract(int move)
	{
		CurrentMove -= move;
		OnMoveChange?.Invoke(move);

		if (CurrentMove <= 0)
		{
			OnMoveFinished?.Invoke();
		}
	}
}
