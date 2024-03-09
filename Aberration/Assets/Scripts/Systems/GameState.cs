using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.Systems;
using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration
{
	public class GameState : MonoBehaviour
    {
        [SerializeField]
        private List<Team> teams;

        public void AddTeam(Team team)
		{
            if (!teams.SafeContains(team))
                ListUtils.SafeAdd(ref teams, team);
		}

        public void RemoveTeam(Team team)
		{
            teams.SafeRemove(team);
		}

        public OtherTeamsEnumerator GetOtherTeams(Team team)
		{
            return new OtherTeamsEnumerator(teams, team);
		}
    }
}
