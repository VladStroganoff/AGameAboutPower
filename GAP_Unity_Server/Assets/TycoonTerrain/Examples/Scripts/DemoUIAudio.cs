using UnityEngine;
using UnityEngine.UI;

namespace Assets.TycoonTerrain.Examples.Scripts {
	[RequireComponent(typeof(AudioController))]
	public class DemoUIAudio : MonoBehaviour {
		public AudioClip ButtonClickSound;

		private AudioController controller;

		private void Awake() {
			controller = GetComponent<AudioController>();

			foreach (Button button in GetComponentsInChildren<Button>(true)) {
				button.onClick.AddListener(OnButtonClick);
			}
		}

		private void OnButtonClick() {
			controller.PlayOneShot(ButtonClickSound);
		}
	}
}
