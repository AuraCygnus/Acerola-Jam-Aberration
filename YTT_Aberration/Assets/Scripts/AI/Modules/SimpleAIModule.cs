using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.Utils;
using UnityEngine;

namespace Aberration
{
	public class SimpleAIModule : AIModule
    {
        [SerializeField]
        private float awarenessRange = 10f;

        public override void UpdateAI(GameState gameState, Team team)
		{
            foreach (Unit unit in team.Units)
			{
                if (unit.State != UnitState.Fighting)
				{
                    Unit targetUnit = GetNearestInRangeTarget(gameState, team, unit);
                    if (targetUnit != null)
                    {
                        unit.SetTarget(targetUnit);
                    }
                }
            }
		}

        private Unit GetNearestInRangeTarget(GameState gameState, Team team, Unit teamUnit)
		{
            float awarenessRangeSq = awarenessRange * awarenessRange;
            Unit nearestUnit = null;
            float shortestDistanceSq = float.MaxValue;
            Vector3 teamUnitPosition = teamUnit.transform.position;
            foreach (Team otherTeam in gameState.GetOtherTeams(team))
            {
                foreach (Unit unit in otherTeam.Units)
                {
                    Vector3 between = teamUnitPosition - unit.transform.position;
                    float distanceSq = Vector3.SqrMagnitude(between);
                    if (distanceSq < awarenessRangeSq && distanceSq < shortestDistanceSq && CombatUtils.IsValidTarget(unit))
                    {
                        nearestUnit = unit;
                        shortestDistanceSq = distanceSq;
                    }
                }
            }

            return nearestUnit;
		}
    }
}
