using Aberration.Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Aberration.Assets.Scripts
{
	/// <summary>
	/// Selection of states a Unit can be in.
	/// </summary>
	public enum UnitState : byte
	{
		Idle,
		Moving,
		Yeeted,
		YeetRecovering,
		ResettingBones,
		MovingToFight,
		Fighting,
		Defeated
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

		private int remainingHp;
		public int RemainingHP
		{
			get { return remainingHp; }
		}

		protected void Awake()
		{
			navAgent.speed = unitData.MoveSpeed;
			navAgent.angularSpeed = unitData.TurnSpeed;
			navAgent.acceleration = unitData.Acceleration;
			navAgent.radius = unitData.Radius;
			navAgent.height = unitData.Height;

			remainingHp = unitData.MaxHP;

			if (team != null)
				SetupTeam();

			SetIdleState();
		}

		public void Setup(Team team)
		{
			this.team = team;

			SetupTeam();
		}

		private void SetupTeam()
		{
			team.AddUnit(this);
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

			if (targetUnit != null)
			{
				UpdateTargetting();
			}
		}

		private void UpdateTargetting()
		{
			Vector3 ownPosition = animationController.MainTransform.position;
			Vector3 targetPosition = targetUnit.transform.position;
			Vector3 between = targetPosition - ownPosition;
			// Target in range
			if (IsTargetInRange(between))
			{
				SetFightingState();
			}
			else
			{
				// Normalize to get the unit length vector between
				Vector3 directionBetween = Vector3.Normalize(between);
				// Calculate offset to move slightly inside the range and target radius
				Vector3 offset = directionBetween * (unitData.Range + targetUnit.unitData.Radius - 0.25f);
				Vector3 movePosition = targetPosition - offset;

				navAgent.SetDestination(movePosition);

				SetMovingToFightState();
			}
		}

		private bool IsTargetInRange()
		{
			Vector3 between = targetUnit.transform.position - animationController.MainTransform.position;
			return IsTargetInRange(between);
		}

		private bool IsTargetInRange(Vector3 between)
		{
			float distance = between.magnitude;

			// Target in range
			return (distance - unitData.Range - targetUnit.unitData.Radius) < 0f;
		}

		public bool IsOwner(byte teamId)
		{
			return this.team.TeamID == teamId;
		}

		#region Set States
		private void SetMovingState()
		{
			if (state == UnitState.Fighting)
				EndCombat();

			navAgent.isStopped = false;
			animationController.SetMoving();
			state = UnitState.Moving;
		}

		private void SetYeeted()
		{
			if (state == UnitState.Fighting)
				EndCombat();

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
			if (state == UnitState.Fighting)
				EndCombat();

			selectionCollider.enabled = true;
			animationController.SetIdle();
			state = UnitState.Idle;
		}

		private void SetMovingToFightState()
		{
			if (state == UnitState.Fighting)
				EndCombat();

			navAgent.isStopped = false;
			animationController.SetMovingToFight();
			state = UnitState.MovingToFight;
		}

		private void SetFightingState()
		{
			navAgent.isStopped = false;
			animationController.SetFighting();
			animationController.AnimationHandler.AttackImpact += OnAttackImpact;
			animationController.AnimationHandler.AttackEnded += OnAttackEnded;
			state = UnitState.Fighting;
		}

		private void SetDefeated()
		{
			navAgent.isStopped = true;
			animationController.SetDefeated();
			state = UnitState.Defeated;
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

				case UnitState.MovingToFight:
					MovingToFightUpdate();
					break;

				case UnitState.Fighting:
					FightingUpdate();
					break;

				case UnitState.Defeated:
					DefeatedUpdate();
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
			if (HasFinishedRagdoll())
			{
				animationController.StopRagdollVelocity();
				animationController.AlignRotationToRagdoll();
				animationController.AlignPositionToRagdoll();
				animationController.PopulateRagdollTransforms();
				SetResettingBones();
			}
		}

		private bool HasFinishedRagdoll()
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

		private void MovingToFightUpdate()
		{
			UpdateTargetting();
		}

		private void FightingUpdate()
		{
			if (IsTargetInRange())
			{
				// Execute attack animation

				// If Ranged Fire Projectile 

				animationController.UpdateFighting();
			}
			else
			{
				// Switch back to moving to attack target
				SetMovingToFightState();
			}
		}

		private void EndCombat()
		{
			targetUnit = null;

			animationController.AnimationHandler.AttackImpact -= OnAttackImpact;
			animationController.AnimationHandler.AttackEnded -= OnAttackEnded;
		}

		private void OnAttackImpact()
		{
			if (targetUnit != null)
			{
				// At correct point in animation Damage target
				targetUnit.remainingHp -= CombatUtils.CalculateDamage(unitData.Attack, targetUnit.unitData.Armour);

				// Repeat until target is defeated, unit loses or unit is issued new orders
				if (targetUnit.remainingHp <= 0)
				{
					targetUnit.SetDefeated();
					EndCombat();
					SetIdleState();
				}
			}
			else
			{
				EndCombat();
				SetIdleState();
			}
		}

		private void OnAttackEnded()
		{

		}

		private void DefeatedUpdate()
		{
			if (HasFinishedRagdoll())
			{
				team.RemoveUnit(this);
				Destroy(gameObject);
			}
		}
		#endregion
	}
}
