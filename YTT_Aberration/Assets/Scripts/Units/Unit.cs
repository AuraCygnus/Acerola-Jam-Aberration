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
		public UnitData UnitData
		{
			get { return unitData; }
		}

		[SerializeField]
		private Collider selectionCollider;

		[SerializeField]
		private NavMeshAgent navAgent;

		[SerializeField]
		private UnitAnimationController animationController;

		[SerializeField]
		private UnitUI unitUI;

		/// <summary>
		/// Transform to use for targetting the Unit (Generally the root of the ragdoll)
		/// </summary>
		[SerializeField]
		private Transform ragdollTargetTransform;
		public Transform TargetTransform
		{
			get
			{
				// In these states should target the ragdoll instead of the main part
				if (state == UnitState.Yeeted || state == UnitState.ResettingBones || state == UnitState.YeetRecovering || state == UnitState.Defeated)
				{
					return ragdollTargetTransform;
				}
				else
				{
					return animationController.MainTransform;
				}
			}
		}

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
		public UnitState State
		{
			get { return state; }
		}

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

			unitUI.SetHp(remainingHp, unitData.MaxHP);

			if (team != null)
				SetupTeam();

			SetIdleState();
		}

		public void Setup(Team team, Camera camera)
		{
			this.team = team;

			SetupTeam();
			unitUI.FollowWorld.Setup(camera);
		}

		private void SetupTeam()
		{
			team.AddUnit(this);
		}

		public void SetSelected(bool isSelected, Team selectingTeam)
		{
			unitUI.SetSelected(isSelected, team == selectingTeam);
		}

		public void SetMoveLocation(Vector3 moveLocation)
		{
			if (state == UnitState.Yeeted || state == UnitState.ResettingBones || state == UnitState.YeetRecovering)
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

			// Add UnitDefeated listener
			targetUnit.team.Controller.EventDispatcher.UnitDefeated += OnUnitDefeated;

			if (targetUnit != null)
			{
				UpdateTargetting();
			}
		}

		private void OnUnitDefeated(Unit unit)
		{
			if (targetUnit != null && targetUnit == unit)
			{
				// Remove UnitDefeated listener
				targetUnit.team.Controller.EventDispatcher.UnitDefeated -= OnUnitDefeated;

				EndCombat();
				targetUnit = null;
				SetIdleState();
			}
			else if (state == UnitState.Fighting || state == UnitState.MovingToFight)
			{
				EndCombat();
				targetUnit = null;
				SetIdleState();
			}
		}

		private void UpdateTargetting()
		{
			Vector3 ownPosition = TargetTransform.position;
			Vector3 targetPosition = targetUnit.TargetTransform.position;
			Vector3 between = targetPosition - ownPosition;
			// Target in range
			if (CombatUtils.IsUnitInAttackRange(between, this, targetUnit))
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
			Vector3 between = targetUnit.TargetTransform.position - TargetTransform.position;
			return CombatUtils.IsUnitInAttackRange(between, this, targetUnit);
		}

		public bool IsOwner(byte teamId)
		{
			return this.team.TeamID == teamId;
		}

		#region Set States
		private void SetMovingState()
		{
			if (!CanChangeState())
				return;

			if (state == UnitState.Fighting)
				EndCombat();

			navAgent.enabled = true;
			animationController.SetMoving();
			state = UnitState.Moving;
		}

		private void SetYeeted()
		{
			if (!CanChangeState())
				return;

			if (state == UnitState.Fighting)
				EndCombat();

			animationController.SetYeeted();
			navAgent.enabled = false;
			selectionCollider.enabled = false;
			animationController.SetRagdollEnabled(true);
			state = UnitState.Yeeted;
		}

		private void SetResettingBones()
		{
			if (!CanChangeState())
				return;

			SetNavMeshPosition();
			animationController.SetResettingBones();
			state = UnitState.ResettingBones;
		}

		private void SetYeetRecovering()
		{
			if (!CanChangeState())
				return;

			animationController.SetRagdollEnabled(false);
			animationController.SetRecovering();
			state = UnitState.YeetRecovering;
		}

		private void SetIdleState()
		{
			if (!CanChangeState())
				return;

			if (state == UnitState.Fighting)
				EndCombat();

			navAgent.enabled = true;
			selectionCollider.enabled = true;
			animationController.SetIdle();
			state = UnitState.Idle;
		}

		private void SetMovingToFightState()
		{
			if (!CanChangeState())
				return;

			if (state == UnitState.Fighting)
				EndCombat();

			navAgent.enabled = true;
			animationController.SetMovingToFight();
			state = UnitState.MovingToFight;
		}

		private void SetFightingState()
		{
			if (!CanChangeState())
				return;

			Debug.Log($"Set Fighting");

			navAgent.enabled = true;
			animationController.SetFighting();

			animationController.AnimationHandler.AttackImpact -= OnAttackImpact;
			animationController.AnimationHandler.AttackImpact += OnAttackImpact;

			animationController.AnimationHandler.AttackEnded -= OnAttackEnded;
			animationController.AnimationHandler.AttackEnded += OnAttackEnded;

			state = UnitState.Fighting;
		}

		private void SetDefeated()
		{
			if (!CanChangeState())
				return;

			// Clear any listeners
			if (targetUnit != null)
			{
				// Remove UnitDefeated listener
				targetUnit.team.Controller.EventDispatcher.UnitDefeated -= OnUnitDefeated;
			}

			navAgent.enabled = false;
			animationController.StopRagdollVelocity();
			animationController.SetDefeated();
			state = UnitState.Defeated;
		}

		private bool CanChangeState()
		{
			if (this == null)
				return false;

			if (state == UnitState.Defeated)
				return false;

			return true;
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
			animationController.AnimationHandler.AttackImpact -= OnAttackImpact;
			animationController.AnimationHandler.AttackEnded -= OnAttackEnded;
		}

		private void OnAttackImpact()
		{
			if (targetUnit != null)
			{
				// At correct point in animation Damage target
				targetUnit.ReduceHP(CombatUtils.CalculateDamage(unitData.Attack, targetUnit.unitData.Armour));

				// Repeat until target is defeated, unit loses or unit is issued new orders
				if (targetUnit.remainingHp <= 0)
				{
					targetUnit.SetDefeated();
					EndCombat();
					targetUnit = null;
					SetIdleState();
				}
			}
			else
			{
				EndCombat();
				SetIdleState();
			}
		}

		private void ReduceHP(int reduction)
		{
			//Debug.Log($"Reducing HP [reduction={reduction}]");

			if (reduction > 0)
			{
				remainingHp -= reduction;
				unitUI.SetHp(remainingHp, unitData.MaxHP);
				unitUI.SetHpVisible(remainingHp < unitData.MaxHP);
			}
			else
			{
				Debug.LogError($"Invalid Hp Reduction [reduction={reduction}]");
			}
		}

		private void OnAttackEnded()
		{

		}

		private void DefeatedUpdate()
		{
			if (HasFinishedRagdoll())
			{
				// Fire event that the unit was defeated
				team.Controller.EventDispatcher.FireUnitDefeated(this);

				team.RemoveUnit(this);
				Destroy(gameObject);
			}
		}
		#endregion

		public void ResetToSafePosition()
		{
			if (SetNavMeshPosition())
			{
				animationController.StopRagdollVelocity();

				navAgent.enabled = false;
				navAgent.enabled = true;
			}
		}

		private bool SetNavMeshPosition()
		{
			if (NavMesh.SamplePosition(TargetTransform.position, out NavMeshHit hit, 1000f, NavMesh.AllAreas))
			{
				// Warp to the position on the NavMesh
				navAgent.transform.position = hit.position;
				TargetTransform.position = hit.position;
				return true;
			}

			return false;
		}
	}
}
