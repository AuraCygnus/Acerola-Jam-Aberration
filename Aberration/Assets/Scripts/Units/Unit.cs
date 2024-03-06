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
		MovingToFight,
		Fighting
	}

	public class Unit : MonoBehaviour
	{
		[SerializeField]
		private UnitData unitData;

		[SerializeField]
		private Collider selectionCollider;

		[SerializeField]
		private NavMeshAgent navAgent;

		[SerializeField]
		private UnitAnimationController animationController;

		/// <summary>
		/// Team the unit belongs to.
		/// </summary>
		[SerializeField]
		private Team team;
		/// <summary>
		/// Id of the team the unit belongs to.
		/// </summary>
		public byte TeamID
		{
			get { return team.TeamID; }
		}

		/// <summary>
		/// Last set move location.
		/// </summary>
		private Vector3 moveLocation;

		/// <summary>
		/// Last set target Unit.
		/// </summary>
		private Unit targetUnit;

		/// <summary>
		/// Current Unit state.
		/// </summary>
		private UnitState state;

		protected void Awake()
		{
			navAgent.speed = unitData.MoveSpeed;
			navAgent.angularSpeed = unitData.TurnSpeed;
			navAgent.acceleration = unitData.Acceleration;
			navAgent.radius = unitData.Radius;
			navAgent.height = unitData.Height;
		}

		public void Setup(Team team)
		{
			this.team = team;
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
			animationController.AddForceToAll(force);
		}

		public void SetTarget(Unit targetUnit)
		{
			this.targetUnit = targetUnit;

			Vector3 diff = targetUnit.transform.position - animationController.MainTransform.position;
			float distance = diff.magnitude;

			// Target in range
			if ((distance - unitData.Range - targetUnit.unitData.Radius) < 0f)
			{
				// Attack target
				state = UnitState.Fighting;
			}
			else
			{
				// Move towards target before attacking
				state = UnitState.MovingToFight;
			}
		}

		public bool IsOwner(byte teamId)
		{
			return this.team.TeamID == teamId;
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
			return animationController.IsRagdollMoving(unitData.StillYeetingMinVelocity);
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
