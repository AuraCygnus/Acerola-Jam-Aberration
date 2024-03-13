using Aberration.Assets.Scripts;
using System.Collections;
using UnityEngine;

namespace Aberration
{
	public class AIController : Controller
    {
        [SerializeField]
        private GameState gameState;

        [SerializeField]
        private Team team;

        [SerializeField]
        private AIModule module;

        [SerializeField]
        private float updateRateSecs = 0.2f;

        private bool updating;

		public override TeamOwnerType OwnerType
		{
			get { return team.OwnerType;  }
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

                module.UpdateAI(gameState, team);
            }
		}
    }
}
