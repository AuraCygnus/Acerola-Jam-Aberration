using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration.Assets.Scripts
{
	/// <summary>
	/// Class for storing all team data.
	/// Perhaps doesn't need to be a MonoBehaviour long term.
	/// </summary>
	public class Team : MonoBehaviour
	{
		private byte teamId;

		private List<Unit> units;

		public void AddUnit(Unit unit)
		{
			ListUtils.SafeAdd(ref units, unit);
		}
	}
}
