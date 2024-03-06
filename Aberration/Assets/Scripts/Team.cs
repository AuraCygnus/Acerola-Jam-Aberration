using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration.Assets.Scripts
{
	public enum TeamOwnerType
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
		private TeamOwnerType ownerType;

		[SerializeField]
		private byte teamId;
		public byte TeamID
		{
			get { return teamId; }
		}

		[SerializeField]
		private List<Unit> units;

		public void AddUnit(Unit unit)
		{
			ListUtils.SafeAdd(ref units, unit);
		}
	}
}
