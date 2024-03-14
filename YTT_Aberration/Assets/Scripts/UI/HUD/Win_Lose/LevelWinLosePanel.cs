using Aberration.Assets.Scripts.Utils;
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

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private AudioClip confirmSound;

		public void DisplayWin()
		{
			if (SceneUtils.IsFinalLevel())
			{
				headerText.SetText("CONGRATULATIONS!!!\nThank you for completing the game!");
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
			audioSource.PlayOneShot(confirmSound, 0.5f);
			SceneUtils.LoadMainMenu();
		}

		private void OnContinueClick()
		{
			audioSource.PlayOneShot(confirmSound, 0.5f);
			SceneUtils.NextLevel();
		}

		private void OnRetryClick()
		{
			audioSource.PlayOneShot(confirmSound, 0.5f);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
