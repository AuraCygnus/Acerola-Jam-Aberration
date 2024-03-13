using Aberration.Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Aberration.Assets.Scripts.UI.Menus
{
	public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

		[SerializeField]
		private Button continueButton;

		[SerializeField]
		private Button settingsButton;

		[SerializeField]
        private Button quitButton;

		[Header("Panels")]
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
		private AudioMixer audioMixer;

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private AudioClip confirmSound;

		[SerializeField]
		private AudioClip cancelSound;

		[SerializeField]
		private string sfxVolumeKey = "SFXVolume";

		private void Awake()
		{
			startButton.onClick.AddListener(OnStartClick);

			if (SceneUtils.TryGetStoredLevel(out int level))
			{
				continueButton.onClick.AddListener(OnContinueClick);
				continueButton.gameObject.SetActive(true);
			}
			else
			{
				continueButton.gameObject.SetActive(false);
			}

			settingsButton.onClick.AddListener(OnSettings);
			quitButton.onClick.AddListener(OnQuitClick);

			settingsPanel.backButton.onClick.AddListener(ReturnFromSettings);
		}

		private void OnStartClick()
		{
			// Load into the first scene
			SceneManager.LoadScene(1);
			ConfirmSound();
		}

		private void OnContinueClick()
		{
			SceneUtils.LoadStoredLevel();
			ConfirmSound();
		}

		private void OnSettings()
		{
			buttonPanel.gameObject.SetActive(false);
			settingsPanel.gameObject.SetActive(true);
			ConfirmSound();
		}

		private void OnQuitClick()
		{
			Application.Quit();
			CancelSound();
		}

		private void ReturnFromSettings()
		{
			buttonPanel.gameObject.SetActive(true);
			settingsPanel.gameObject.SetActive(false);
			CancelSound();
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
