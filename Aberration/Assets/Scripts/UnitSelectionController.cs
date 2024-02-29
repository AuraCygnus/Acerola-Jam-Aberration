using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration
{
	public class UnitSelectionController : MonoBehaviour
	{
		[SerializeField]
		private Camera selectionCamera;

		[SerializeField]
		private int selectionMask;

		[SerializeField]
		private float maxRayDistance = 1000;

		[SerializeField]
		private float yeetForceMultiplier = 3f;

		private List<Collider> selectedObjects;

		private Vector3 selectStartLocation;
		private Vector3 selectRay;

		private Vector3 dragStartLocation;

		private const float SingleTargetSelectionRange = 0.1f;
		private const float SingleTargetSelectionRangeSq = SingleTargetSelectionRange * SingleTargetSelectionRange;

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				selectStartLocation = GetMouseClickPosition();
			}

			if (Input.GetMouseButtonUp(0))
			{
				Vector3 selectEndLocation = GetMouseClickPosition();

				Vector3 diff = selectEndLocation - selectStartLocation;
				float lengthSq = Vector3.SqrMagnitude(diff);
				if (lengthSq <= SingleTargetSelectionRangeSq)
				{
					TrySelectSingleObject(selectEndLocation);
				}
				else
				{
					// try selecting multiple objects in a box
				}
			}

			int numSelectedObjects = selectedObjects.SafeCount();
			if (numSelectedObjects > 0)
			{
				if (Input.GetMouseButtonDown(1))
				{
					dragStartLocation = GetMouseClickPosition();
				}

				if (Input.GetMouseButtonUp(1))
				{
					Vector3 dragEndLocation = GetMouseClickPosition();
					Vector3 force = (dragEndLocation - dragStartLocation) * yeetForceMultiplier;
					foreach (var collider in selectedObjects)
					{
						if (collider.attachedRigidbody != null)
							collider.attachedRigidbody.AddForce(force);
					}
				}
			}
		}

		private void TrySelectSingleObject(Vector3 selectLocation)
		{
			Vector3 cameraLocation = selectionCamera.transform.position;
			selectRay = selectLocation - cameraLocation;
			if (Physics.Raycast(cameraLocation, selectRay, out RaycastHit hit, maxRayDistance, ~selectionMask))
			{
				selectedObjects.SafeClear();
				ListUtils.SafeAdd(ref selectedObjects, hit.collider);
			}
			else
			{
				selectedObjects.SafeClear();
			}
		}

		private Vector3 GetMouseClickPosition()
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = selectionCamera.farClipPlane;
			return selectionCamera.ScreenToWorldPoint(mousePos);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Vector3 cameraPos = selectionCamera.transform.position;
			Vector3 ray = Vector3.Normalize(selectRay);
			ray *= maxRayDistance;
			Gizmos.DrawLine(cameraPos, cameraPos + ray);
		}
	}

}
