using Aberration.Assets.Scripts;
using UnityEngine;

namespace Aberration
{
	public class TeamActionState : MonoBehaviour
    {
        [SerializeField]
        private Team team;

        [SerializeField]
        private TeamAction action;

        /// <summary>
        /// Time when the action finishes executing.
        /// </summary>
        private float executionCompleteTime;

        /// <summary>
        /// Time when the action can be executed again.
        /// </summary>
        private float cooldownTime;
        public float CooldownTime
		{
            get { return cooldownTime; }
		}

        private bool isSelected;
        public bool IsSelected
		{
            get { return isSelected; }
		}

        public bool CanSelect()
		{
            if (Time.time < cooldownTime)
                return false;

            return true;
        }

		public void Select(GameState gameState)
		{
			if (action.ActionTargetType == ActionTargetType.Global)
			{
                // Execute immediately
                ActionParams actionParams = new ActionParams();
                actionParams.gameState = gameState;
                actionParams.sourceTeam = team;
                if (CanExecute(actionParams))
                    Execute(actionParams);

                isSelected = false;
            }
            else
			{
                // Clear any previously selected action

                // Set selected action
                isSelected = true;
            }
		}

		public bool CanExecute(ActionParams actionParams)
		{
            if (!CanSelect())
                return false;

            return action.IsValid(actionParams);
		}

        public void Execute(ActionParams actionParams)
		{
            action.Execute(actionParams);
            executionCompleteTime = Time.time + action.ExecuteTime;
            cooldownTime = Time.time + action.CooldownSecs;
		}

        public void Deselect()
		{
            isSelected = false;
		}
    }
}
