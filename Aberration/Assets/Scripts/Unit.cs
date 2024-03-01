using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aberration.Assets.Scripts
{
	[Serializable]
	public struct RagdollElement
	{
		public Collider collider;
		public Rigidbody rigidBody;
	}

	public class Unit : MonoBehaviour
	{
		[SerializeField]
		private Collider selectionCollider;

		[SerializeField]
		private RagdollElement[] ragdollElements;

		[SerializeField]
		private NavMeshAgent navAgent;

		private Vector3 moveLocation;

		public void SetMoveLocation(Vector3 moveLocation)
		{
			// Store for our own usage
			this.moveLocation = moveLocation;

			// Set nav agent move location
			if (!navAgent.SetDestination(moveLocation))
			{
				Debug.LogError("Failed to request destination");
			}
		}
	}
}
