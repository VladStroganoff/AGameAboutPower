using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DemoPaintUI : MonoBehaviour {
	public int SelectedMaterialId;
	public SelectTerrainTypeEvent SelectTerrainTypeEvent = new SelectTerrainTypeEvent();

	public void OnEnable() {
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	public void OnDisable() {
		GetComponent<Button>().onClick.RemoveListener(OnClick);
	}

	private void OnClick() {
		SelectTerrainTypeEvent.Invoke((ushort)SelectedMaterialId);
	}
}

public class SelectTerrainTypeEvent : UnityEvent<ushort> {}
