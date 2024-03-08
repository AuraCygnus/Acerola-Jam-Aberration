using System;
using UnityEngine;

namespace Aberration
{
	public class UnitAnimationHandler : MonoBehaviour
    {
		public event Action AttackImpact;
		public event Action AttackEnded;

		private void OnAttackImpact(int parameter)
		{
			Debug.Log("Impact");

			if (AttackImpact != null)
				AttackImpact();
		}

		private void OnAttackEnded(int parameter)
		{
			Debug.Log("Ended");

			if (AttackEnded != null)
				AttackEnded();
		}
	}
}
