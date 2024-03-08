using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aberration.Assets.Scripts.Utils
{
	public static class CombatUtils
	{
		public static int CalculateDamage(int attack, int armour)
		{
			return attack - armour;
		}
	}
}
