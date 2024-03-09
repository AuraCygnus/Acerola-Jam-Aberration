namespace Aberration.Assets.Scripts.Utils
{
	public static class CombatUtils
	{
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
