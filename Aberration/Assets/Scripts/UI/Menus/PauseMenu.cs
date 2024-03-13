using Aberration.Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Aberration.Assets.Scripts.UI.Menus
{
	public class PauseMenu : MonoBehaviour
	{
		[SerializeField]
		private Button resumeButton;

		[SerializeField]
		private Button settingsButton;

		[SerializeField]
		private Button restartLevelButton;

		[SerializeField]
		private Button mainMenuButton;

		/// <summary>
		/// Button Panel for quick toggle to Settings menu.
		/// </summary>
		[SerializeField]
		private GameObject buttonPanel;

		/// <summary>
		/// Settings Panel for quick toggle to Settings menu.
		/// </summary>
		[SerializeField]
		private SettingsMenuPanel settingsPanel;

		[Header("Sounds")]
		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private AudioClip confirmSound;

		[SerializeField]
		private AudioClip cancelSound;

		private void Awake()
		{
			resumeButton.onClick.AddListener(OnResume);
			settingsButton.onClick.AddListener(OnSettings);
			restartLevelButton.onClick.AddListener(OnRestartLevel);
			mainMenuButton.onClick.AddListener(OnMainMenu);

			settingsPanel.backButton.onClick.AddListener(ReturnFromSettings);
		}

		private void OnResume()
		{
			GameSystemUtils.SetPaused(false);
			gameObject.SetActive(false);
			CancelSound();
		}

		private void OnSettings()
		{
			buttonPanel.gameObject.SetActive(false);
			settingsPanel.gameObject.SetActive(true);
			ConfirmSound();
		}

		private void ReturnFromSettings()
		{
			buttonPanel.gameObject.SetActive(true);
			settingsPanel.gameObject.SetActive(false);
			CancelSound();
		}

		private void OnRestartLevel()
		{
			GameSystemUtils.SetPaused(false);
			SceneUtils.RestartLevel();
			ConfirmSound();
		}

		private void OnMainMenu()
		{
			GameSystemUtils.SetPaused(false);
			SceneUtils.LoadMainMenu();
			ConfirmSound();
		}

		private void ConfirmSound()
		{
			audioSource.PlayOneShot(confirmSound, 0.5f);
		}

		private void CancelSound()
		{
			audioSource.PlayOneShot(cancelSound, 0.5f);
		}
	}
}
