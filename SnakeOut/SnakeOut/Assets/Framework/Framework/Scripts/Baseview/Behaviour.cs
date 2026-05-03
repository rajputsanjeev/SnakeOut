using System;
using System.Collections;
using System.Collections.Generic;
using Base.UI.Controller;
using Base.UI.Manager;
using UnityEngine;

namespace BaseView
{
	public abstract class Behaviour<T1> : UIBaseController where T1 : View
	{
		protected T1 Prefab;
		public GameObject ParticleEffect;

		protected override void Awake()
		{
			base.Awake();
			Prefab = this.GetComponent<T1>();
			Init();
		}

		public void GetBaseView()
		{
			Prefab = this.GetComponent<T1>();
		}

		public override void ShowPanel(bool on)
		{
			if (on)
			{
				HomeUIAnimator?.Hide(() => {  });
				ShowAnimation();
			}
			else
			{
				HomeUIAnimator?.PlayExternally();
				HideAnimation();
			}
			if (ParticleEffect) ParticleEffect.SetActive(on);
		}

		public virtual void ShowAnimation()
		{
			if (PopupAnimator != null)
			{
				PopupAnimator.Show();
			}
		}

		public virtual void HideAnimation()
		{
			if (PopupAnimator != null)
			{
				PopupAnimator.Hide();
			}
		}

		protected virtual void Init()
		{
		}
	}

	public abstract class Behaviour<T1, T2> : Behaviour<T1> where T1 : View
	{
		protected T2 Model;

		protected override void Awake()
		{
			base.Awake();
			Model = this.GetComponent<T2>();
			base.Awake();
		}
	}
}