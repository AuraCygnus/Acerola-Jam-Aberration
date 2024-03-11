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

		private void Awake()
		{
			startButton.onClick.AddListener(OnStartClick);
			quitButton.onClick.AddListener(OnQuitClick);
		}

		private void OnStartClick()
		{
			// Load into the first scene
			SceneManager.LoadScene(1);
		}

		private void OnQuitClick()
		{
			Application.Quit();
		}
	}
}
