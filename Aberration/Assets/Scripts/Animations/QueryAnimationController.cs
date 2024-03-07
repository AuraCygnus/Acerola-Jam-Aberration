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
			// Set the facial expression
			emoControl.ChangeEmotion(QuerySDEmotionalController.QueryChanSDEmotionalType.NORMAL_SAD);
		}

		public override void SetResettingBones()
		{
			// Set the facial expression
			emoControl.ChangeEmotion(QuerySDEmotionalController.QueryChanSDEmotionalType.NORMAL_GURUGURU);

			stateEndTime = Time.time + unitData.TimeToResetBones;
		}

		public override void UpdateResettingBones()
		{
			float elapsedTime = unitData.TimeToResetBones - (stateEndTime - Time.time);
			float fullTime = Mathf.Max(unitData.TimeToResetBones, 0.001f);
			float elapsedPercentage = Mathf.Clamp01(elapsedTime / fullTime);

			// Don't interpolate if 1 or more, creates NaN errors
			if (elapsedPercentage < 1f)
			{
				for (int i = 0; i < ragdollElements.Length; i++)
				{
					ragdollElements[i].transform.localPosition = Vector3.Lerp(ragdollBoneTransforms[i].position, recoverBoneTransforms[i].position, elapsedPercentage);
					ragdollElements[i].transform.localRotation = Quaternion.Lerp(ragdollBoneTransforms[i].rotation, recoverBoneTransforms[i].rotation, elapsedPercentage);
				}
			}
			else
			{
				for (int i = 0; i < ragdollElements.Length; i++)
				{
					ragdollElements[i].transform.localPosition = recoverBoneTransforms[i].position;
					ragdollElements[i].transform.localRotation = recoverBoneTransforms[i].rotation;
				}
			}
		}

		public override void SetRecovering()
		{
			animator.enabled = true;
			animator.Play(unitData.RecoverAnimStateName);

			stateEndTime = Time.time + unitData.RecoverTimeSecs;
		}

		public override void SetFighting()
		{
			if (!string.IsNullOrEmpty(unitData.CombatAnimStateName))
				animator.Play(unitData.CombatAnimStateName);
		}
	}
}
