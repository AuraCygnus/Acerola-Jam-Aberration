using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Aberration.Assets.Scripts
{
	/// <summary>
	/// Selection of states a Unit can be in.
	/// </summary>
	public enum UnitState
	{
		Idle,
		Moving,
		Yeeted,
		YeetRecovering,
		ResettingBones,
		Fighting
	}

	public class Unit : MonoBehaviour
	{
		[SerializeField]
		private Collider selectionCollider;

		[SerializeField]
		private NavMeshAgent navAgent;

		[SerializeField]
		private UnitAnimationController animationController;

		/// <summary>
		/// Minimum velocity at which the Unit is still classed as yeeting
		/// </summary>
		[SerializeField]
		private float stillYeetingMinVelocity = 0.5f;

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

		public void Setup(byte teamId)
		{
			this.teamId = teamId;
		}

		public void SetMoveLocation(Vector3 moveLocation)
		{
			if (state == UnitState.Yeeted || state == UnitState.YeetRecovering)
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

		public void Yeet(Vector3 force)
		{
			SetYeeted();

			// Fall back to yeeting from the main part
			animationController.AddForceToMain(force);
		}

		private bool IsOwnCollider(Collider collider)
		{
			return animationController.IsOwnCollider(collider);
		}

		public bool IsOwner(byte teamId)
		{
			return this.teamId == teamId;
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
			animationController.SetRagdollEnabled(true);
			state = UnitState.Yeeted;
		}

		private void SetResettingBones()
		{
			animationController.SetResettingBones();
			state = UnitState.ResettingBones;
		}

		private void SetYeetRecovering()
		{
			animationController.SetRagdollEnabled(false);
			animationController.SetRecovering();
			state = UnitState.YeetRecovering;
		}

		private void SetIdleState()
		{
			selectionCollider.enabled = true;
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

				case UnitState.ResettingBones:
					ResettingBonesUpdate();
					break;

				case UnitState.YeetRecovering:
					YeetRecoveringStateUpdate();
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
				animationController.StopRagdollVelocity();
				animationController.AlignRotationToRagdoll();
				animationController.AlignPositionToRagdoll();
				animationController.PopulateRagdollTransforms();
				SetResettingBones();
			}
		}

		private bool HasFinishedYeeting()
		{
			return animationController.IsRagdollMoving(stillYeetingMinVelocity);
		}

		private void ResettingBonesUpdate()
		{
			animationController.UpdateResettingBones();

			if (Time.time > animationController.StateEndTime)
			{
				SetYeetRecovering();
			}
		}

		private void YeetRecoveringStateUpdate()
		{
			if (Time.time > animationController.StateEndTime)
			{
				SetIdleState();
			}
		}
		#endregion
	}
}
