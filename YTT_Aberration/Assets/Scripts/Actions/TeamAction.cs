using Aberration.Assets.Scripts;
using UnityEngine;

namespace Aberration
{
    public struct ActionParams
	{
        /// <summary>
        /// Usually the main Camera.
        /// </summary>
        public Camera camera;

        /// <summary>
        /// Team executing the action.
        /// </summary>
        public Team sourceTeam;

        /// <summary>
        /// Target location for the action.
        /// </summary>
        public Vector3 location;

        /// <summary>
        /// Target unit for the action.
        /// </summary>
        public Unit unit;
	}

    public enum ActionTargetType : byte
	{
        Global,
        Position,
        Unit
	}

	public abstract class TeamAction : ScriptableObject
    {
        [SerializeField]
        protected float cooldownSecs;
        public float CooldownSecs
		{
            get { return cooldownSecs; }
		}

        [SerializeField]
        protected float executeTime;
        public float ExecuteTime
		{
            get { return executeTime; }
		}

        [SerializeField]
        protected ActionTargetType actionTargetType;
        public ActionTargetType ActionTargetType
		{
            get { return actionTargetType; }
		}

		public abstract bool IsValid(ActionParams actionParams);

		public abstract void Execute(ActionParams actionParams);
    }
}
