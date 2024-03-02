using UnityEngine;

namespace Aberration.Assets.Scripts
{
	public abstract class UnitAnimationController : MonoBehaviour
	{
		[SerializeField]
		protected Animator animator;

		public abstract void SetMoving();

		public abstract void SetIdle();

		public abstract void SetYeeted();

		public abstract void SetRecovering();
	}
}
