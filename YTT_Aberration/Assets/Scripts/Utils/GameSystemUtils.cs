using UnityEngine;

namespace Aberration.Assets.Scripts.Utils
{
	public static class GameSystemUtils
	{
		public static void SetPaused(bool isPaused)
		{
			Time.timeScale = (isPaused) ? 0f : 1f;
		}
	}
}
