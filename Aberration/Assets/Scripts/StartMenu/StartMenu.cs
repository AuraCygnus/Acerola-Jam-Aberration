using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Aberration
{
	public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button quitButton;

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private AudioClip confirmSound;

		[SerializeField]
		private AudioClip cancelSound;

		private void Awake()
		{
			startButton.onClick.AddListener(OnStartClick);
			quitButton.onClick.AddListener(OnQuitClick);
		}

		private void OnStartClick()
		{
			// Load into the first scene
			SceneManager.LoadScene(1);
			audioSource.PlayOneShot(confirmSound, 0.5f);
		}

		private void OnQuitClick()
		{
			Application.Quit();
			audioSource.PlayOneShot(cancelSound, 0.5f);
		}
	}
}
