using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.Systems;
using Aberration.Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration
{
	public class GameState : MonoBehaviour
    {
        /// <summary>
        /// Team of the player.
        /// </summary>
        [SerializeField]
        private Team ownTeam;

        /// <summary>
        /// All teams in the game
        /// </summary>
        [SerializeField]
        private List<Team> teams;

        /// <summary>
        /// Height at which units need to be reset because they're out of bounds.
        /// </summary>
        [SerializeField]
        private float deathPlane = -10f;

        /// <summary>
        /// Update rate for game state checks.
        /// </summary>
        [SerializeField]
        private float updateRateSecs = 1f;

        private bool updating;

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

        protected void OnEnable()
        {
            updating = true;
            StartCoroutine(OnUpdateTick());
        }

        private IEnumerator OnUpdateTick()
        {
            while (updating)
            {
                yield return new WaitForSeconds(updateRateSecs);

                CheckWinLoss();
                CheckDeathPlane();
            }
        }

        /// <summary>
        /// Check whether the game has been lost or won
        /// </summary>
        private void CheckWinLoss()
		{
            if (!ownTeam.HasUnits())
			{
                Lose();
			}

            if (AreOtherTeamsWipedOut())
			{

			}
		}

        private void Lose()
		{

		}

        private void Win()
		{

		}

        private bool AreOtherTeamsWipedOut()
		{
            foreach (Team team in GetOtherTeams(ownTeam))
			{
                if (team.HasUnits())
                    return false;
			}

            return true;
		}

        /// <summary>
        /// Check if any units have dropped below the death/reset plane.
        /// </summary>
        private void CheckDeathPlane()
		{
            foreach (Team team in teams)
			{
                foreach (Unit unit in team.Units)
				{
                    if (unit.TargetTransform.position.y < deathPlane)
					{
                        unit.ResetToSafePosition();
					}
				}
			}
		}
    }
}
