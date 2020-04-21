using UnityEngine;

namespace Assets.TycoonTerrain.Examples.Scripts {
	[RequireComponent(typeof(AudioSource))]
	public class AudioController : MonoBehaviour {
		[Tooltip("The minimum play time between consecutive audio events (in seconds)")]
		public float MinimumPlayTime;
		[Range(0f, 100f)]
		[Tooltip("The maximum pitch variation in %")]
		public float PitchVariation;

		private AudioSource audioSource;
		private float lastPlayTime;

		private void Awake() {
			audioSource = GetComponent<AudioSource>();
		}

		public void PlayOneShot(AudioClip clip) {
			//Prevent audio glitching by ignoring audio events that fire too closely after eachother
			if (audioSource.isPlaying && Time.time - lastPlayTime < MinimumPlayTime) {
				return;
			}

			//Add slight variation to audio clip pitch to make sound less repetitive
			float pitchVar = PitchVariation / 100f;
			audioSource.pitch = Random.Range(1f - pitchVar, 1f + pitchVar);

			//Play the clip
			audioSource.PlayOneShot(clip);
			lastPlayTime = Time.time;
		}
	}
}