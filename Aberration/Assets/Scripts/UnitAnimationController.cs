using System;
using UnityEngine;

namespace Aberration.Assets.Scripts
{
	/// <summary>
	/// Elements for a Ragdoll part.
	/// </summary>
	[Serializable]
	public struct RagdollElement
	{
		public Transform transform;
		public Collider collider;
		public Rigidbody rigidBody;
		public CharacterJoint joint;
	}

	public struct BoneTransform
	{
		public Vector3 position;
		public Quaternion rotation;
	}

	public abstract class UnitAnimationController : MonoBehaviour
	{
		/// <summary>
		/// Main root transform of Unit.
		/// </summary>
		[SerializeField]
		protected Transform mainTransform;

		[SerializeField]
		protected Animator animator;

		[SerializeField]
		protected RagdollElement[] ragdollElements;

		[SerializeField]
		protected RagdollElement mainRagdollElement;

		[SerializeField]
		protected string recoverAnimClipName;

		[SerializeField]
		protected float timeToResetBones = 2f;

		[SerializeField]
		protected string recoverAnimStateName;

		[SerializeField]
		protected float recoverTimeSecs = 2f;

		protected BoneTransform[] recoverBoneTransforms;
		protected BoneTransform[] ragdollBoneTransforms;

		protected float stateEndTime;
		public float StateEndTime
		{
			get { return stateEndTime; }
		}

		protected void Awake()
		{
			recoverBoneTransforms = new BoneTransform[ragdollElements.Length];
			ragdollBoneTransforms = new BoneTransform[ragdollElements.Length];

			PopulateAnimationStartBoneTransforms(recoverAnimClipName, recoverBoneTransforms);
		}

		public abstract void SetMoving();

		public abstract void SetIdle();

		public abstract void SetYeeted();

		public abstract void SetResettingBones();

		public abstract void SetRecovering();

		public virtual void UpdateResettingBones()
		{

		}

		public void AddForceToMain(Vector3 force)
		{
			// Fall back to yeeting from the main part
			mainRagdollElement.rigidBody.AddForce(force);
		}

		public bool IsOwnCollider(Collider collider)
		{
			foreach (RagdollElement element in ragdollElements)
			{
				if (element.collider == collider)
					return true;
			}

			return false;
		}

		public void SetRagdollEnabled(bool isEnabled)
		{
			foreach (RagdollElement element in ragdollElements)
			{
				// Enable/Disable collider
				element.collider.enabled = isEnabled;

				// Enable/Disable Rigid Body
				element.rigidBody.detectCollisions = isEnabled;
				element.rigidBody.useGravity = isEnabled;

				// Join is optional as not every part of the ragdoll has one
				if (element.joint != null)
				{
					// Enable/Disable Character Joint
					element.joint.enableCollision = isEnabled;
				}
			}
		}

		public void StopRagdollVelocity()
		{
			foreach (RagdollElement element in ragdollElements)
			{
				element.rigidBody.velocity = Vector3.zero;
			}
		}

		public void AlignRotationToRagdoll()
		{
			Vector3 originalRagdollPosition = mainRagdollElement.transform.position;
			Quaternion originalRagdollRotation = mainRagdollElement.transform.rotation;

			Vector3 desiredDirection = mainRagdollElement.transform.up * -1f;
			desiredDirection.y = 0f;
			desiredDirection.Normalize();

			Quaternion fromToRotation = Quaternion.FromToRotation(mainTransform.forward, desiredDirection);
			mainTransform.rotation *= fromToRotation;

			// Need to reset the Ragdoll position since the parent has moved
			mainRagdollElement.transform.position = originalRagdollPosition;
			// Need to reset the Ragdoll position since the parent has moved
			mainRagdollElement.transform.rotation = originalRagdollRotation;
		}

		/// <summary>
		/// Based on https://www.youtube.com/watch?v=B_NnQQKiw6I&ab_channel=KetraGames
		/// </summary>
		public void AlignPositionToRagdoll()
		{
			Vector3 originalRagdollPosition = mainRagdollElement.transform.position;

			// Move main Transform to Ragdoll
			mainTransform.position = originalRagdollPosition;

			if (Physics.Raycast(mainTransform.position, Vector3.down, out RaycastHit hit))
			{
				mainTransform.position = new Vector3(originalRagdollPosition.x, hit.point.y, originalRagdollPosition.z);
			}

			// Need to reset the Ragdoll position since the parent has moved
			mainRagdollElement.transform.position = originalRagdollPosition;
		}

		public bool IsRagdollMoving(float minVelocity)
		{
			foreach (RagdollElement element in ragdollElements)
			{
				float speed = element.rigidBody.velocity.magnitude;
				if (speed > minVelocity)
				{
					return false;
				}
			}

			return true;
		}

		public void PopulateRagdollTransforms()
		{
			PopulateBoneTransforms(ragdollBoneTransforms);
		}

		private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
		{
			for (int i = 0; i < ragdollElements.Length; i++)
			{
				boneTransforms[i].position = ragdollElements[i].transform.localPosition;
				boneTransforms[i].rotation = ragdollElements[i].transform.localRotation;
			}
		}

		private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
		{
			Vector3 positionBefore = mainTransform.position;
			Quaternion rotationBefore = mainTransform.rotation;

			foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
			{
				if (clip.name == clipName)
				{
					clip.SampleAnimation(animator.gameObject, 0);
					PopulateBoneTransforms(boneTransforms);
					break;
				}
			}

			mainTransform.position = positionBefore;
			mainTransform.rotation = rotationBefore;
		}
	}
}
