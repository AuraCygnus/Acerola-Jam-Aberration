using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration.Assets.Scripts
{
	public enum TeamOwnerType : byte
	{
		Player,
		AI
	}

	/// <summary>
	/// Class for storing all team data.
	/// Perhaps doesn't need to be a MonoBehaviour long term.
	/// </summary>
	public class Team : MonoBehaviour
	{
		[SerializeField]
		private GameState gameState;
		public GameState GameState
		{
			get { return gameState; }
		}

		[SerializeField]
		private TeamOwnerType ownerType;
		public TeamOwnerType OwnerType
		{
			get { return ownerType; }
		}

		[SerializeField]
		private Controller controller;
		public Controller Controller
		{
			get { return controller; }
		}

		[SerializeField]
		private byte teamId;
		public byte TeamID
		{
			get { return teamId; }
		}

		[SerializeField]
		private List<Unit> units;
		public IEnumerable<Unit> Units
		{
			get { return units; }
		}

		[SerializeField]
		private List<TeamActionState> actions;
		public IEnumerable<TeamActionState> Actions
		{
			get { return actions; }
		}

		public void AddUnit(Unit unit)
		{
			if (!units.SafeContains(unit))
				ListUtils.SafeAdd(ref units, unit);
		}

		public void RemoveUnit(Unit unit)
		{
			units.SafeRemove(unit);
		}

		public bool HasUnits()
		{
			return units.SafeCount() > 0;
		}

		public void SetSelectedAction(TeamActionState actionState)
		{
			ClearSelectedAction();

			PlayerController playerController = controller as PlayerController;
			if (playerController != null)
			{
				if (actionState != null && actionState.CanSelect())
					actionState.Select(playerController.SelectionCamera);

				playerController.SetToActionTargetSelection(actionState);
			}

			controller.EventDispatcher.FireActionSelected(actionState);
		}

		public void ClearSelectedAction()
		{
			foreach (TeamActionState teamActionState in actions)
			{
				teamActionState.Deselect();
			}
		}
	}
}
