using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aberration.Assets.Scripts.Utils
{
	public static class SceneUtils
	{
		public const string LevelProgressKey = "LevelProgress";

		public static void LoadMainMenu()
		{
			SceneManager.LoadScene(0);
		}

		public static void NextLevel()
		{
			int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
			PlayerPrefs.SetInt(LevelProgressKey, nextLevelIndex);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		public static bool TryGetStoredLevel(out int level)
		{
			level = PlayerPrefs.GetInt(LevelProgressKey, 0);
			return level > 0;
		}

		public static void LoadStoredLevel()
		{
			int level = PlayerPrefs.GetInt(LevelProgressKey, 1);
			SceneManager.LoadScene(level);
		}

		public static void RestartLevel()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public static bool IsFinalLevel()
		{
			return IsFinalLevel(SceneManager.GetActiveScene().buildIndex);
		}

		public static bool IsFinalLevel(int levelIndex)
		{
			int numLevels = SceneManager.sceneCountInBuildSettings;
			Debug.Log($"Level [levelIndex={levelIndex}, count={numLevels}]");
			return levelIndex >= (numLevels - 1);
		}
	}
}
