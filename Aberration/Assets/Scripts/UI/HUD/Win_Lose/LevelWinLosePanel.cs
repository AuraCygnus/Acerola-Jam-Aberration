using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Aberration.Assets.Scripts.UI.HUD.Win_Lose
{
	public class LevelWinLosePanel : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI headerText;

		[Header("Button")]
		[SerializeField]
		private Button button;

		[SerializeField]
		private TextMeshProUGUI buttonText;

		public void DisplayWin()
		{
			int currentLevel = SceneManager.GetActiveScene().buildIndex;
			int numLevels = SceneManager.sceneCountInBuildSettings;
			bool isFinalLevel = currentLevel >= numLevels;
			Debug.Log($"Level [current={currentLevel}, count={numLevels}]");
			if (isFinalLevel)
			{
				headerText.SetText("CONGLATURATION!!!\nA WINNER IS YOU");
				buttonText.SetText("Main Menu");
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnMainMenuClick);
			}
			else
			{
				headerText.SetText("Level Complete!");
				buttonText.SetText("Continue");
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(OnContinueClick);
			}
		}

		public void DisplayLose()
		{
			headerText.SetText("Level Failed!");
			buttonText.SetText("Retry");
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(OnRetryClick);
		}

		private void OnMainMenuClick()
		{
			SceneManager.LoadScene(0);
		}

		private void OnContinueClick()
		{

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		private void OnRetryClick()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
