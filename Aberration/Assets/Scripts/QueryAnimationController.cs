using UnityEngine;

namespace Aberration.Assets.Scripts
{
	public class QueryAnimationController : UnitAnimationController
	{
		[SerializeField]
		private QuerySDEmotionalController emoControl;

		public override void SetMoving()
		{
			animator.enabled = true;
			QuerySDMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_WALK, animator, emoControl);
		}

		public override void SetIdle()
		{
			animator.enabled = true;
			QuerySDMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_IDLE, animator, emoControl);
		}

		public override void SetYeeted()
		{
			// Disable animator to let ragdoll take over
			animator.enabled = false;
			// Set the facial expression though
			emoControl.ChangeEmotion(QuerySDEmotionalController.QueryChanSDEmotionalType.NORMAL_GURUGURU);
		}

		public override void SetRecovering()
		{
			animator.enabled = true;
			QuerySDMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_POSE_SIT, animator, emoControl);
		}
	}
}
