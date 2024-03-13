using Aberration.Assets.Scripts;
using UnityEngine;

namespace Aberration
{
	public abstract class AIModule : ScriptableObject
    {
        public abstract void UpdateAI(GameState gameState, Team team);
    }
}
