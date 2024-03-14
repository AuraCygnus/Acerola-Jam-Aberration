using Aberration.Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YTT.Aberration.Assets.Scripts.UI.Menus;

namespace Aberration.Assets.Scripts.UI.Menus
{
	public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

		[SerializeField]
		private Button continueButton;

		[SerializeField]
		private Button instructionsButton;

		[SerializeField]
		private Button settingsButton;

		[SerializeField]
        private Button quitButton;

		[Header("Panels")]
		[SerializeField]
		private GameObject titlePanel;

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

		[SerializeField]
		private InstructionsPanel instructionsPanel;

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

			instructionsButton.onClick.AddListener(OnInstructionsClick);
			settingsButton.onClick.AddListener(OnSettings);
			quitButton.onClick.AddListener(OnQuitClick);

			settingsPanel.backButton.onClick.AddListener(ReturnFromSettings);
			instructionsPanel.backButton.onClick.AddListener(ReturnFromInstructions);
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

		private void OnInstructionsClick()
		{
			titlePanel.gameObject.SetActive(false);
			buttonPanel.gameObject.SetActive(false);
			instructionsPanel.gameObject.SetActive(true);
			ConfirmSound();
		}

		private void OnSettings()
		{
			titlePanel.gameObject.SetActive(false);
			buttonPanel.gameObject.SetActive(false);
			settingsPanel.gameObject.SetActive(true);
			ConfirmSound();
		}

		private void OnQuitClick()
		{
			Application.Quit();
			CancelSound();
		}

		private void ReturnFromInstructions()
		{
			titlePanel.gameObject.SetActive(true);
			buttonPanel.gameObject.SetActive(true);
			instructionsPanel.gameObject.SetActive(false);
			CancelSound();
		}

		private void ReturnFromSettings()
		{
			titlePanel.gameObject.SetActive(true);
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
