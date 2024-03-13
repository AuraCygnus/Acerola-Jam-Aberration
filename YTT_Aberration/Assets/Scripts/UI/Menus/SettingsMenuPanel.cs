using Aberration.Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Aberration.Assets.Scripts.UI.Menus
{
	public class SettingsMenuPanel : MonoBehaviour
	{
		[SerializeField]
		private AudioMixer audioMixer;

		[SerializeField]
		private Slider masterVolumeSlider;

		[SerializeField]
		private Slider musicVolumeSlider;

		[SerializeField]
		private Slider sfxVolumeSlider;

		public Button backButton;

		[SerializeField]
		private string masterVolumeKey = "MasterVolume";
		[SerializeField]
		private string musicVolumeKey = "MusicVolume";
		[SerializeField]
		private string sfxVolumeKey = "SFXVolume";

		protected void Awake()
		{
			masterVolumeSlider.value = SoundUtils.GetStoredVolume(masterVolumeKey);
			musicVolumeSlider.value = SoundUtils.GetStoredVolume(musicVolumeKey);
			sfxVolumeSlider.value = SoundUtils.GetStoredVolume(sfxVolumeKey);

			masterVolumeSlider.onValueChanged.AddListener(OnMasterChanged);
			musicVolumeSlider.onValueChanged.AddListener(OnMusicChanged);
			sfxVolumeSlider.onValueChanged.AddListener(OnSFXChanged);
		}

		private void OnMasterChanged(float value)
		{
			SoundUtils.ApplyVolumeParam(audioMixer, masterVolumeKey, value);
		}

		private void OnMusicChanged(float value)
		{
			SoundUtils.ApplyVolumeParam(audioMixer, musicVolumeKey, value);
		}

		private void OnSFXChanged(float value)
		{
			SoundUtils.ApplyVolumeParam(audioMixer, sfxVolumeKey, value);
		}
	}
}
