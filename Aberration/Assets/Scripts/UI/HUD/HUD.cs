using Aberration.Assets.Scripts;
using UnityEngine;

namespace Aberration
{
	public class HUD : MonoBehaviour
    {
        [SerializeField]
        private GameState gameState;

        [SerializeField]
        private Team team;

		[SerializeField]
		private ActionsPanel actionsPanel;

		private void OnEnable()
		{
			if (team != null)
				Setup(team);
		}

		public void Setup(Team team)
		{
			actionsPanel.Setup(team);
		}
    }
}
