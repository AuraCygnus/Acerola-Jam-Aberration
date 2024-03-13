using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Aberration
{
	public class ActionsPanel : MonoBehaviour
    {
        [SerializeField]
        private ActionButton actionButtonPrefab;

        private List<ActionButton> actionButtons;

        public void Setup(Team team)
		{
            UnityUtils.DestroyMonoBehaviorGOs(actionButtons);

            foreach (TeamActionState actionState in team.Actions)
			{
                ActionButton button = Instantiate(actionButtonPrefab, transform);
                button.Setup(team, actionState);
                ListUtils.SafeAdd(ref actionButtons, button);
			}
		}
    }
}
