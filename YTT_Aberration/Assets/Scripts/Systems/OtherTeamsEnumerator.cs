using System.Collections;
using System.Collections.Generic;

namespace Aberration.Assets.Scripts.Systems
{
	public struct OtherTeamsEnumerator : IEnumerator<Team>, IEnumerable<Team>
	{
		private List<Team> teams;
		private Team ownTeam;

		private int position;
		private Team current;

		public OtherTeamsEnumerator(List<Team> teams, Team ownTeam)
		{
			this.teams = teams;
			this.ownTeam = ownTeam;
			position = 0;
			current = null;
		}

		public Team Current
		{
			get { return current; }
		}

		object IEnumerator.Current
		{
			get { return current; }
		}

		public OtherTeamsEnumerator GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		IEnumerator<Team> IEnumerable<Team>.GetEnumerator()
		{
			return this;
		}

		public bool MoveNext()
		{
			int numTeams = teams.Count;
			for (int i = position; i < numTeams; i++)
			{
				current = teams[i];
				if (current != ownTeam)
				{
					position = i + 1;
					return true;
				}
			}

			return false;
		}

		public void Dispose()
		{
			Reset();
		}

		public void Reset()
		{
			position = 0;
			current = null;
		}
	}
}
