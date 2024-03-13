using UnityEngine;

namespace Aberration.Assets.Scripts.Actions
{
	public class SpawnUnitAction : TeamAction
	{
		[SerializeField]
		protected float rangeFromUnit;

		[SerializeField]
		protected Unit unitPrefab;

		public override bool IsValid(ActionParams actionParams)
		{
			// Check if target location is near enough to another Unit to spawn
			if (IsRangeValid(actionParams.sourceTeam, actionParams.location))
			{
				return true;
			}

			return false;
		}

		public override void Execute(ActionParams actionParams)
		{
			// Spawn the unit is position
			Unit newUnit = Instantiate(unitPrefab, actionParams.location, Quaternion.identity);
			// Initialise it
			newUnit.Setup(actionParams.sourceTeam, actionParams.camera);

			// Fire event that the unit spawned
			actionParams.sourceTeam.Controller.EventDispatcher.FireUnitSpawned(newUnit);
		}

		private bool IsRangeValid(Team team, Vector3 targetLocation)
		{
			float rangeSq = rangeFromUnit * rangeFromUnit;
			foreach (Unit unit in team.Units)
			{
				Vector3 between = unit.TargetTransform.position - targetLocation;
				if (Vector3.SqrMagnitude(between) < rangeSq)
					return true;
			}

			return false;
		}
	}
}
