using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aberration
{
	public enum SelectionState : byte
	{
		Free,
		SelectedOwnUnit,
		SelectedEnemyUnit,
		ActionTargetSelection
	}

	public class PlayerController : Controller
	{
		[SerializeField]
		private Camera selectionCamera;
		public Camera SelectionCamera
		{
			get { return selectionCamera; }
		}

		[SerializeField]
		private int unitMask;

		[SerializeField]
		private int groundMask;

		[SerializeField]
		private int uiLayer;

		[SerializeField]
		private float maxRayDistance = 1000;

		[SerializeField]
		private float yeetForceMultiplier = 3f;

		[SerializeField]
		private float mouseYeetTriggerRange = 800f;

		[SerializeField]
		private Team ownTeam;

		[SerializeField]
		private HUD hud;

		/// <summary>
		/// How muhc upward force to add to yeet
		/// </summary>
		[SerializeField]
		private float yeetRise = 3f;

		private List<Unit> selectedObjects;

		private Vector2 startPos;
		private Vector3 selectStartLocation;


		private Vector3 selectRay;

		private Vector3 dragStartLocation;

		private Vector3 yeetForce;

		private SelectionState state;

		private TeamActionState currentAction;

		private const float SingleTargetSelectionRange = 0.1f;
		private const float SingleTargetSelectionRangeSq = SingleTargetSelectionRange * SingleTargetSelectionRange;

		public override TeamOwnerType OwnerType
		{
			get { return ownTeam.OwnerType; }
		}

		protected override void Awake()
		{
			base.Awake();

			EventDispatcher.UnitDefeated += OnUnitDefeated;
		}

		private void OnUnitDefeated(Unit unit)
		{
			selectedObjects.SafeRemove(unit);

			if (state == SelectionState.SelectedEnemyUnit || state == SelectionState.SelectedOwnUnit)
			{
				if (selectedObjects.SafeCount() == 0)
				{
					SetFreeState();
				}
			}
		}

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

				case SelectionState.ActionTargetSelection:
					HandleActionTargetSelection();
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
				startPos = Input.mousePosition;
				selectStartLocation = GetMouseClickPosition();
			}

			if (Input.GetMouseButton(0))
			{
				UpdateSelectionBox(Input.mousePosition);
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
					ReleaseSelectionBox();
				}
			}
		}

		/// <summary>
		/// Based on: https://gamedevacademy.org/rts-unity-tutorial/
		/// </summary>
		/// <param name="curMousePos"></param>
		private void UpdateSelectionBox(Vector2 curMousePos)
		{
			if (!hud.SelectionBox.gameObject.activeInHierarchy)
				hud.SelectionBox.gameObject.SetActive(true);

			CanvasScaler scaler = hud.Canvas.GetComponent<CanvasScaler>();
			if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
			{
				// Have to divide by Canvas scaleFactor
				float scaleFactor = hud.Canvas.scaleFactor;
				float width = (curMousePos.x - startPos.x) / scaleFactor;
				float height = (curMousePos.y - startPos.y) / scaleFactor;
				hud.SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
				hud.SelectionBox.anchoredPosition = (startPos / scaleFactor) + new Vector2(width / 2, height / 2);
			}
			else
			{
				float width = (curMousePos.x - startPos.x);
				float height = (curMousePos.y - startPos.y);
				hud.SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
				hud.SelectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
			}
		}

		/// <summary>
		/// Based on https://gamedevacademy.org/rts-unity-tutorial/
		/// </summary>
		private void ReleaseSelectionBox()
		{
			hud.SelectionBox.gameObject.SetActive(false);
			Vector2 min = hud.SelectionBox.anchoredPosition - (hud.SelectionBox.sizeDelta / 2);
			Vector2 max = hud.SelectionBox.anchoredPosition + (hud.SelectionBox.sizeDelta / 2);
			foreach (Unit unit in ownTeam.Units)
			{
				Vector3 screenPos = selectionCamera.WorldToScreenPoint(unit.transform.position);

				if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
				{
					ListUtils.SafeAdd(ref selectedObjects, unit);

					SetOwnUnitSelectedState();
					SetUnitSelected(unit);
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
					startPos = Input.mousePosition;
					dragStartLocation = GetMouseClickPosition();
					ownTeam.Controller.EventDispatcher.FireYeetStart();
				}

				if (Input.GetMouseButton(1))
				{
					HandleYeetArrowUpdate(Input.mousePosition);
				}

				if (rightClickUp)
				{
					Vector3 dragEndLocation = GetMouseClickPosition();

					if (CheckForEnemyTarget(dragEndLocation, out Unit targetUnit))
					{
						foreach (Unit unit in selectedObjects)
						{
							unit.SetTarget(targetUnit);
						}
					}
					else
					{
						Vector3 diff = dragEndLocation - dragStartLocation;
						float lengthSq = Vector3.SqrMagnitude(diff);
						if (lengthSq <= (mouseYeetTriggerRange * mouseYeetTriggerRange))
						{
							TrySettingMoveDestination(dragEndLocation);
							ownTeam.Controller.EventDispatcher.FireYeetCancel();
						}
						else
						{
							TryYeeting(diff);
							ownTeam.Controller.EventDispatcher.FireYeetEnd();
						}
					}

					HandleYeetFinish();
				}
			}
		}

		private void HandleYeetArrowUpdate(Vector2 curMousePos)
		{
			if (!hud.YeetImage.gameObject.activeInHierarchy)
				hud.YeetImage.gameObject.SetActive(true);

			Vector2 between = curMousePos - startPos;
			float angle = Vector2.SignedAngle(Vector2.right, between);

			CanvasScaler scaler = hud.Canvas.GetComponent<CanvasScaler>();
			if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
			{
				// Have to divide by Canvas scaleFactor
				float scaleFactor = hud.Canvas.scaleFactor;
				float width = between.x / scaleFactor;
				float height = between.y / scaleFactor;
				hud.YeetImage.sizeDelta = new Vector2(Mathf.Abs(between.magnitude), hud.YeetImage.sizeDelta.y);
				hud.YeetImage.anchoredPosition = (startPos / scaleFactor) + new Vector2(width / 2, height / 2);
				hud.YeetImage.localRotation = Quaternion.Euler(0f, 0f, angle);
			}
			else
			{
				float width = between.x;
				float height = between.y;
				hud.YeetImage.sizeDelta = new Vector2(Mathf.Abs(between.magnitude), hud.YeetImage.sizeDelta.y);
				hud.YeetImage.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
				hud.YeetImage.localRotation = Quaternion.Euler(0f, 0f, angle);
			}
		}

		private void HandleYeetFinish()
		{
			if (hud.YeetImage.gameObject.activeInHierarchy)
				hud.YeetImage.gameObject.SetActive(false);
		}

		private void HandleSelectedEnemyUnit()
		{
			HandleSelection();
		}

		#region Set State
		public void SetToActionTargetSelection(TeamActionState currentAction)
		{
			this.currentAction = currentAction;
			state = SelectionState.ActionTargetSelection;
		}

		private void SetFreeState()
		{
			// Clear any selection
			selectedObjects.SafeClear();

			// Clear selected state of action
			ownTeam.ClearSelectedAction();

			// Switch back to free state
			state = SelectionState.Free;
		}

		private void SetOwnUnitSelectedState()
		{
			state = SelectionState.SelectedOwnUnit;
		}

		private void SetEnemyUnitSelectedState()
		{
			state = SelectionState.SelectedEnemyUnit;
		}
		#endregion

		private void HandleActionTargetSelection()
		{
			bool leftClickDown = Input.GetMouseButtonDown(0);
			bool leftClickUp = Input.GetMouseButtonUp(0);

			if (leftClickDown && !IsPointerOverUIElement())
			{
				startPos = Input.mousePosition;
				selectStartLocation = GetMouseClickPosition();
			}

			if (leftClickUp && !IsPointerOverUIElement())
			{
				Debug.Log("Switching to Free");

				EventDispatcher.FireActionCancelled(currentAction);
				SetFreeState();

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

			bool rightClickUp = Input.GetMouseButtonUp(1);

			if (rightClickUp && currentAction != null)
			{
				Vector3 clickPosition = GetMouseClickPosition();

				// Get World Position At target

				// Get Unit at target
				Unit unit = null;
				if (TrySelect(clickPosition, out RaycastHit unitHit, ~(1 >> unitMask)))
				{
					unit = unitHit.collider.GetComponent<Unit>();
				}

				Vector3 position = new Vector3();
				if (TrySelect(clickPosition, out RaycastHit groundHit, ~(1 >> groundMask)))
				{
					position = groundHit.point;
				}

				// Try To Execute action
				ActionParams actionParams = new ActionParams();
				actionParams.camera = selectionCamera;
				actionParams.sourceTeam = ownTeam;
				actionParams.unit = unit;
				actionParams.location = position;
				if (currentAction.CanExecute(actionParams))
				{
					currentAction.Execute(actionParams);
					EventDispatcher.FireActionExecuted(currentAction);
				}
			}
		}

		private void TrySelectSingleObject(Vector3 selectLocation)
		{
			if (TrySelect(selectLocation, out RaycastHit unitHit, ~(1 >> unitMask)))
			{
				ClearSelection();

				Unit unit = unitHit.collider.GetComponent<Unit>();
				if (unit != null)
				{
					if (ownTeam.TeamID == unit.TeamID)
					{
						SetOwnUnitSelectedState();
					}
					else
					{
						SetEnemyUnitSelectedState();
					}

					SetUnitSelected(unit);
				}
			}
			else
			{
				ClearSelection();
			}
		}

		private void SetUnitSelected(Unit unit)
		{
			unit.SetSelected(true, ownTeam);
			ownTeam.Controller.EventDispatcher.FireUnitSelected(unit);

			ListUtils.SafeAdd(ref selectedObjects, unit);
		}

		private bool TrySelect(Vector3 selectLocation, out RaycastHit unitHit, int layerMask)
		{
			Vector3 cameraLocation = selectionCamera.transform.position;
			selectRay = selectLocation - cameraLocation;
			if (Physics.Raycast(cameraLocation, selectRay, out unitHit, maxRayDistance, layerMask))
			{
				return true;
			}

			return false;
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
			if (TrySelect(selectLocation, out RaycastHit moveHit, ~(1 >> groundMask)))
			{
				foreach (Unit unit in selectedObjects)
				{
					unit.SetMoveLocation(moveHit.point);
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

				foreach (Unit unit in selectedObjects)
				{
					unit.Yeet(yeetForce);
				}
			}
		}

		private Vector3 GetMouseClickPosition()
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = selectionCamera.farClipPlane;
			return selectionCamera.ScreenToWorldPoint(mousePos);
		}

		/// <summary>
		/// Based on https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
		/// </summary>
		/// <returns></returns>
		public bool IsPointerOverUIElement()
		{
			return IsPointerOverUIElement(GetEventSystemRaycastResults());
		}

		//Returns 'true' if we touched or hovering on Unity UI element.
		private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
		{
			for (int index = 0; index < eventSystemRaycastResults.Count; index++)
			{
				RaycastResult curRaysastResult = eventSystemRaycastResults[index];
				if (curRaysastResult.gameObject.layer == uiLayer)
					return true;
			}
			return false;
		}

		//Gets all event system raycast results of current mouse or touch position.
		static List<RaycastResult> GetEventSystemRaycastResults()
		{
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			eventData.position = Input.mousePosition;
			List<RaycastResult> raycastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventData, raycastResults);
			return raycastResults;
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
