using Aberration.Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace Aberration.Assets.Scripts.Systems.Sounds
{
	public class MusicPlayer : MonoBehaviour
	{
		[SerializeField]
		private AudioMixer audioMixer;

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private string musicVolumeKey = "MusicVolume";

		private void Start()
		{
			// Make sure Music volume is applied
			float volume = SoundUtils.GetStoredVolume(musicVolumeKey);
			SoundUtils.ApplyVolumeParam(audioMixer, musicVolumeKey, volume);

			audioSource.Play();
		}
	}
}
