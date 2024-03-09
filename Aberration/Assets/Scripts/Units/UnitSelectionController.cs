using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration
{
	public enum SelectionState
	{
		Free,
		SelectedOwnUnit,
		SelectedEnemyUnit
	}

	public class UnitSelectionController : MonoBehaviour
	{
		[SerializeField]
		private Camera selectionCamera;

		[SerializeField]
		private int unitMask;

		[SerializeField]
		private int groundMask;

		[SerializeField]
		private float maxRayDistance = 1000;

		[SerializeField]
		private float yeetForceMultiplier = 3f;

		[SerializeField]
		private float mouseYeetTriggerRange = 800f;

		[SerializeField]
		private Team ownTeam;

		/// <summary>
		/// How muhc upward force to add to yeet
		/// </summary>
		[SerializeField]
		private float yeetRise = 3f;

		private List<Collider> selectedObjects;

		private Vector3 selectStartLocation;
		private Vector3 selectRay;

		private Vector3 dragStartLocation;

		private Vector3 yeetForce;

		private SelectionState state;

		private const float SingleTargetSelectionRange = 0.1f;
		private const float SingleTargetSelectionRangeSq = SingleTargetSelectionRange * SingleTargetSelectionRange;

		private void Update()
		{
			switch (state)
			{
				case SelectionState.Free:
					HandleFreeState();
					break;

				case SelectionState.SelectedOwnUnit:
					HandleSelectedOwnUnits();
					break;

				case SelectionState.SelectedEnemyUnit:
					HandleSelectedEnemyUnit();
					break;
			}
		}

		private void HandleFreeState()
		{
			HandleSelection();
		}

		private void HandleSelection()
		{
			bool leftClickDown = Input.GetMouseButtonDown(0);
			bool leftClickUp = Input.GetMouseButtonUp(0);

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
		}

		private void HandleSelectedOwnUnits()
		{
			// Still need to consider selection
			HandleSelection();

			// Also need to consider Move, Attack & Yeet actions
			bool rightClickDown = Input.GetMouseButtonDown(1);
			bool rightClickUp = Input.GetMouseButtonUp(1);

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

					if (CheckForEnemyTarget(dragEndLocation, out Unit targetUnit))
					{
						foreach (Collider collider in selectedObjects)
						{
							Unit unit = collider.GetComponent<Unit>();
							if (unit != null)
							{
								unit.SetTarget(targetUnit);
							}
						}
					}
					else
					{
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
		}

		private void HandleSelectedEnemyUnit()
		{
			HandleSelection();
		}

		private void TrySelectSingleObject(Vector3 selectLocation)
		{
			Vector3 cameraLocation = selectionCamera.transform.position;
			selectRay = selectLocation - cameraLocation;
			if (Physics.Raycast(cameraLocation, selectRay, out RaycastHit unitHit, maxRayDistance, ~(1 >> unitMask)))
			{
				ClearSelection();

				Unit unit = unitHit.collider.GetComponent<Unit>();
				if (unit != null)
				{
					if (ownTeam.TeamID == unit.TeamID)
					{
						state = SelectionState.SelectedOwnUnit;
					}
					else
					{
						state = SelectionState.SelectedEnemyUnit;
					}

					unit.SetSelected(true, ownTeam);

					ListUtils.SafeAdd(ref selectedObjects, unitHit.collider);
				}
			}
			else
			{
				ClearSelection();
			}
		}

		private void ClearSelection()
		{
			int numSelected = selectedObjects.SafeCount();
			for (int i = 0; i < numSelected; i++)
			{
				Unit unit = selectedObjects[i].GetComponent<Unit>();
				if (unit != null)
				{
					unit.SetSelected(false, ownTeam);
				}
			}

			selectedObjects.SafeClear();
		}

		private bool CheckForEnemyTarget(Vector3 selectLocation, out Unit unit)
		{
			Vector3 cameraLocation = selectionCamera.transform.position;
			selectRay = selectLocation - cameraLocation;
			if (Physics.Raycast(cameraLocation, selectRay, out RaycastHit unitHit, maxRayDistance, ~(1 >> unitMask)))
			{
				Unit targetUnit = unitHit.collider.GetComponent<Unit>();
				if (targetUnit != null)
				{
					if (ownTeam.TeamID != targetUnit.TeamID)
					{
						unit = targetUnit;
						return true;
					}
				}
			}

			unit = null;
			return false;
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
			if (selectedObjects != null)
			{
				diff.y = 0;
				float yeetXZMagnitude = diff.magnitude;
				float yeetUpwards = yeetRise * yeetXZMagnitude;
				diff.y = yeetUpwards;

				yeetForce = diff * yeetForceMultiplier;

				foreach (Collider collider in selectedObjects)
				{
					Unit unit = collider.GetComponent<Unit>();
					if (unit != null)
					{
						unit.Yeet(yeetForce);
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

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(dragStartLocation, dragStartLocation + yeetForce);
		}
	}
}
