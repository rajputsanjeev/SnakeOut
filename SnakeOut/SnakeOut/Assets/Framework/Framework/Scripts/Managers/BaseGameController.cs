using Base.Interfaces;
using UnityEngine;

namespace Base.Controller
{
	public class BaseGameController : MonoBehaviour
	{
		protected IGameEventListener gameEventListener;

		protected virtual void Awake()
		{
			///GameManager.OnInit += OnGameInit;
		}

		protected virtual void OnDestroy()
		{
			//GameManager.OnInit -= OnGameInit;
		}

		protected void OnGameInit(IGameEventListener gameEventListener)
		{
			this.gameEventListener = gameEventListener;
			SetGameManagerListner();
		}

		public virtual void SetGameManagerListner()
		{

		}
	}
}