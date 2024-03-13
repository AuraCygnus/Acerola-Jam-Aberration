using UnityEngine;
using UnityEngine.Audio;

namespace Aberration.Assets.Scripts.Utils
{
	public static class SoundUtils
	{
		private const int VolumeScale = 1000;
		private const float InvVolumeScale = 1f / VolumeScale;

		public static void ApplyVolumeParam(AudioMixer audioMixer, string settingKey, float value)
		{
			if (audioMixer != null)
			{
				// Convert to db
				float dbVolume = Linear01ToDB(value);
				//LogUtils.LogDebug("dbVolume=" + dbVolume);
				// Set value
				audioMixer.SetFloat(settingKey, dbVolume);

				SetStoredVolume(settingKey, value);
			}
		}

		public static void SetStoredVolume(string key, float volume)
		{
			// Clamp volume for sanity
			volume = Mathf.Clamp01(volume);
			PlayerPrefs.SetInt(key, (int)(VolumeScale * volume));
		}

		public static float GetStoredVolume(string key, float defaultVolume = 0.5f)
		{
			int defaultVolumeInt = (int)(VolumeScale * defaultVolume);
			int volume = PlayerPrefs.GetInt(key, defaultVolumeInt);
			// Clamp volume for sanity
			return Mathf.Clamp01(InvVolumeScale * volume);
		}

		/// <summary>
		/// Convert a [0,1] volume value to volume in db.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		public static float Linear01ToDB(float rawValue)
		{
			// Remap from linear to db value (handle 0f as a special case)
			return (rawValue != 0f) ? 20 * Mathf.Log10(rawValue) : -80f;
		}
	}
}
