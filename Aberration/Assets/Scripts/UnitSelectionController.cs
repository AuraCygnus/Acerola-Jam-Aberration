using Aberration.Assets.Scripts;
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
		private int unitMask;

		[SerializeField]
		private int bodyPartMask;

		[SerializeField]
		private int groundMask;

		[SerializeField]
		private float maxRayDistance = 1000;

		[SerializeField]
		private float yeetForceMultiplier = 3f;

		[SerializeField]
		private float mouseYeetTriggerRange = 800f;

		/// <summary>
		/// How muhc upward force to add to yeet
		/// </summary>
		[SerializeField]
		private float yeetRise = 3f;

		private List<Collider> selectedObjects;
		private List<Collider> selectedBodyParts;

		private Vector3 selectStartLocation;
		private Vector3 selectRay;

		private Vector3 dragStartLocation;

		private const float SingleTargetSelectionRange = 0.1f;
		private const float SingleTargetSelectionRangeSq = SingleTargetSelectionRange * SingleTargetSelectionRange;

		private void Update()
		{
			bool leftClickDown = Input.GetMouseButtonDown(0);
			bool leftClickUp = Input.GetMouseButtonUp(0);
			bool rightClickDown = Input.GetMouseButtonDown(1);
			bool rightClickUp = Input.GetMouseButtonUp(1);

			if (leftClickDown)
			{
				selectStartLocation = GetMouseClickPosition();
			}

			if (leftClickUp)
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
				if (rightClickDown)
				{
					dragStartLocation = GetMouseClickPosition();
				}

				if (rightClickUp)
				{
					Vector3 dragEndLocation = GetMouseClickPosition();

					Vector3 diff = dragEndLocation - dragStartLocation;
					float lengthSq = Vector3.SqrMagnitude(diff);
					if (lengthSq <= (mouseYeetTriggerRange * mouseYeetTriggerRange))
					{
						TrySettingMoveDestination(dragEndLocation);
					}
					else
					{
						TryYeeting(diff);
					}
				}
			}
		}

		private void TrySelectSingleObject(Vector3 selectLocation)
		{
			Vector3 cameraLocation = selectionCamera.transform.position;
			selectRay = selectLocation - cameraLocation;
			if (Physics.Raycast(cameraLocation, selectRay, out RaycastHit unitHit, maxRayDistance, ~(1 >> unitMask)))
			{					
				selectedObjects.SafeClear();
				ListUtils.SafeAdd(ref selectedObjects, unitHit.collider);

				// Check for a body part selection
				if (Physics.Raycast(cameraLocation, selectRay, out RaycastHit bodyHit, maxRayDistance, ~(1 >> bodyPartMask)))
				{
					selectedBodyParts.SafeClear();
					ListUtils.SafeAdd(ref selectedBodyParts, bodyHit.collider);
				}
			}
			else
			{
				selectedObjects.SafeClear();
			}
		}

		private void TrySettingMoveDestination(Vector3 selectLocation)
		{
			Vector3 cameraLocation = selectionCamera.transform.position;
			selectRay = selectLocation - cameraLocation;
			if (Physics.Raycast(cameraLocation, selectRay, out RaycastHit moveHit, maxRayDistance, ~(1 >> groundMask)))
			{
				foreach (Collider collider in selectedObjects)
				{
					Unit unit = collider.GetComponent<Unit>();
					if (unit != null)
					{
						unit.SetMoveLocation(moveHit.point);
					}
				}
			}
		}

		private void TryYeeting(Vector3 diff)
		{
			if (selectedBodyParts != null)
			{
				diff.y = 0;
				float yeetXZMagnitude = diff.magnitude;
				float yeetUpwards = yeetRise * yeetXZMagnitude;
				diff.y = yeetUpwards;

				Vector3 force = diff * yeetForceMultiplier;

				foreach (Collider collider in selectedObjects)
				{
					Unit unit = collider.GetComponent<Unit>();
					if (unit != null)
					{
						unit.Yeet(force, selectedBodyParts);
					}
				}
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
