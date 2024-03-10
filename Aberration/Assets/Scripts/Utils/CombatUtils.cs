using UnityEngine;

namespace Aberration.Assets.Scripts.Utils
{
	public static class CombatUtils
	{
		public static bool IsUnitInAttackRange(Vector3 between, Unit sourceUnit, Unit targetUnit)
		{
			float distance = between.magnitude;

			// Target in range
			return (distance - sourceUnit.UnitData.Range - targetUnit.UnitData.Radius) < 0f;
		}

		public static int CalculateDamage(int attack, int armour)
		{
			return attack - armour;
		}

		public static bool IsValidTarget(Unit unit)
		{
			if (unit == null)
				return false;

			if (unit.RemainingHP <= 0)
				return false;

			return true;
		}
	}
}
