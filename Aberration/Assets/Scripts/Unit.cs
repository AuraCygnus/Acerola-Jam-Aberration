using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

	/// <summary>
	/// Selection of states a Unit can be in.
	/// </summary>
	public enum UnitState
	{
		Idle,
		Moving,
		Yeeted,
		Recovering,
		Fighting
	}

	public class Unit : MonoBehaviour
	{
		[SerializeField]
		private Collider selectionCollider;

		[SerializeField]
		private RagdollElement[] ragdollElements;

		[SerializeField]
		private RagdollElement mainRagdollElement;

		[SerializeField]
		private NavMeshAgent navAgent;

		[SerializeField]
		private UnitAnimationController animationController;

		/// <summary>
		/// Minimum velocity at which the Unit is still classed as yeeting
		/// </summary>
		[SerializeField]
		private float stillYeetingMinVelocity = 0.5f;

		[SerializeField]
		private float recoverTimeSecs = 3f;

		/// <summary>
		/// Id of team the unit belongs to.
		/// </summary>
		private byte teamId;

		/// <summary>
		/// Last set move location.
		/// </summary>
		private Vector3 moveLocation;

		/// <summary>
		/// Current Unit state.
		/// </summary>
		private UnitState state;

		private float recoverEndTime;

		public void Setup(byte teamId)
		{
			this.teamId = teamId;
		}

		public void SetMoveLocation(Vector3 moveLocation)
		{
			if (state == UnitState.Yeeted || state == UnitState.Recovering)
			{
				Debug.Log("Unable to move at current");
				return;
			}

			// Store for our own usage
			this.moveLocation = moveLocation;

			// Set nav agent move location
			if (navAgent.SetDestination(moveLocation))
			{
				SetMovingState();
			}
			else
			{
				Debug.LogError("Failed to request destination");
			}
		}

		public void Yeet(Vector3 force, List<Collider> selectedBodyParts)
		{
			SetYeeted();

			if (selectedBodyParts != null)
			{
				bool hasYeetedCollider = false;
				foreach (var collider in selectedBodyParts)
				{
					if (IsOwnCollider(collider))
					{
						if (collider.attachedRigidbody != null)
						{
							collider.attachedRigidbody.AddForce(force);
							hasYeetedCollider = true;
						}
					}
				}

				if (!hasYeetedCollider)
				{
					// Fall back to yeeting from the main part
					mainRagdollElement.rigidBody.AddForce(force);
				}
			}
		}

		private bool IsOwnCollider(Collider collider)
		{
			foreach (RagdollElement element in ragdollElements)
			{
				if (element.collider == collider)
					return true;
			}

			return false;
		}

		public bool IsOwner(byte teamId)
		{
			return this.teamId == teamId;
		}

		private void SetRagdollEnabled(bool isEnabled)
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

		#region Set States
		private void SetMovingState()
		{
			navAgent.isStopped = false;
			animationController.SetMoving();
			state = UnitState.Moving;
		}

		private void SetYeeted()
		{
			animationController.SetYeeted();
			navAgent.isStopped = true;
			selectionCollider.enabled = false;
			SetRagdollEnabled(true);
			state = UnitState.Yeeted;
		}

		private void SetRecovering()
		{
			selectionCollider.enabled = true;
			SetRagdollEnabled(false);
			animationController.SetRecovering();
			state = UnitState.Recovering;

			recoverEndTime = Time.time + recoverTimeSecs;
		}

		private void SetIdleState()
		{
			animationController.SetIdle();
			state = UnitState.Idle;
		}
		#endregion

		#region Updates
		private void Update()
		{
			switch (state)
			{
				case UnitState.Moving:
					MovingStateUpdate();
					break;

				case UnitState.Yeeted:
					YeetedStateUpdate();
					break;

				case UnitState.Recovering:
					RecoveringStateUpdate();
					break;
			}
		}

		private void MovingStateUpdate()
		{
			// Check if we've reached the destination
			if (!navAgent.pathPending)
			{
				if (navAgent.remainingDistance <= navAgent.stoppingDistance)
				{
					if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
					{
						SetIdleState();
					}
				}
			}
		}

		private void YeetedStateUpdate()
		{
			// Check for most movement stopped
			if (HasFinishedYeeting())
			{
				StopRagdollVelocity();
				SetRecovering();
			}
		}

		private bool HasFinishedYeeting()
		{
			foreach (RagdollElement element in ragdollElements)
			{
				float speed = element.rigidBody.velocity.magnitude;
				if (speed > stillYeetingMinVelocity)
				{
					return false;
				}
			}

			return true;
		}

		private void StopRagdollVelocity()
		{
			foreach (RagdollElement element in ragdollElements)
			{
				element.rigidBody.velocity = Vector3.zero;
			}
		}

		private void RecoveringStateUpdate()
		{
			if (Time.time > recoverEndTime)
			{
				SetIdleState();
			}
		}
		#endregion
	}
}
