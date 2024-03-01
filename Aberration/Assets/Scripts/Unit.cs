using System;
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
		public Collider collider;
		public Rigidbody rigidBody;
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
		private NavMeshAgent navAgent;

		/// <summary>
		/// Last set move location.
		/// </summary>
		private Vector3 moveLocation;

		/// <summary>
		/// Current Unit state.
		/// </summary>
		private UnitState state;

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
				state = UnitState.Moving;
			}
			else
			{
				Debug.LogError("Failed to request destination");
			}
		}

		private void OnYeet()
		{
			selectionCollider.enabled = false;
			SetRagdollEnabled(true);
		}

		private void SetRagdollEnabled(bool isEnabled)
		{
			foreach (RagdollElement element in ragdollElements)
			{
				element.collider.enabled = isEnabled;
			}
		}

		private void OnRecover()
		{
			selectionCollider.enabled = true;
			SetRagdollEnabled(false);
		}

		private void Update()
		{
			switch (state)
			{
				case UnitState.Moving:
					MovingState();
					break;
			}
		}

		private void MovingState()
		{
			// Check if we've reached the destination
			if (!navAgent.pathPending)
			{
				if (navAgent.remainingDistance <= navAgent.stoppingDistance)
				{
					if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
					{
						state = UnitState.Idle;
					}
				}
			}
		}
	}
}
