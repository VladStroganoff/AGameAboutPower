using System;
using System.Collections;
using System.Collections.Generic;
using TycoonTerrain;
using UnityEngine;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour {
	public Button TerraformButton;
	public Button PaintButton;
	public Button WaterButton;

	public Button TerraformerHillButton;
	public Button TerraformerCliffButton;

	public TycoonTileRaycaster Raycaster;
	public TerraformingTool TerraformingTool;
	public PainterTool PainterTool;
	public WaterTool WaterTool;
	public TerrainSelectionPreviewer TerraformerPreviewer;
	public TerrainPainterPreviewer PainterPreviewer;
	public Slider BrushSizeSlider;
	public Text BrushSizeLabel;
	public int MaxBrushSize = 6;

	public GameObject TerraformPanel;
	public GameObject PaintPanel;
	public GameObject WaterPanel;

    // Start is called before the first frame update
    void Start()
    {
	    TerraformButton.onClick.AddListener(OnClickEditButton);
	    PaintButton.onClick.AddListener(OnClickPaintButton);
	    WaterButton.onClick.AddListener(OnClickWaterButton);
		
		TerraformerHillButton.onClick.AddListener(OnSelectHillTerraforming);
		TerraformerCliffButton.onClick.AddListener(OnSelectCliffTerraforming);

	    var paintButtons = PaintPanel.GetComponentsInChildren<DemoPaintUI>(true);

	    foreach (DemoPaintUI paintButton in paintButtons) {
		    paintButton.SelectTerrainTypeEvent.AddListener(OnSelectedTerrainType);
	    }

		DisableAll();
    }

    void Update () {
	    if (Input.GetAxis("Mouse ScrollWheel") > 0 && Raycaster.BrushSize < MaxBrushSize) {
		    Raycaster.BrushSize += 1;
		    UpdateBrushSizeLabel();
	    }
	    else if (Input.GetAxis("Mouse ScrollWheel") < 0 && Raycaster.BrushSize > 0) {
		    Raycaster.BrushSize -= 1;
		    UpdateBrushSizeLabel();
	    }
	    else if((int)BrushSizeSlider.value != Raycaster.BrushSize) {
		    Raycaster.BrushSize = (int) BrushSizeSlider.value;
		    UpdateBrushSizeLabel();
	    }
    }

	private void UpdateBrushSizeLabel() {
		BrushSizeLabel.text = Raycaster.BrushSize.ToString();
		BrushSizeSlider.value = Raycaster.BrushSize;
	}

	private void OnClickEditButton() {
	    if (TerraformPanel.activeInHierarchy) {
			DisableAll();
			return;
	    }

	    TerraformerPreviewer.enabled = true;
	    PainterPreviewer.enabled = false;
	    TerraformingTool.enabled = true;
	    PainterTool.enabled = false;
	    WaterTool.enabled = false;
		Raycaster.UnlockSelection();
		
	    TerraformPanel.SetActive(true);
	    PaintPanel.SetActive(false);
	    WaterPanel.SetActive(false);
    }

    private void OnClickPaintButton() {
	    if (PaintPanel.activeInHierarchy) {
			DisableAll();
			return;
	    }

	    TerraformerPreviewer.enabled = false;
	    PainterPreviewer.enabled = true;
	    TerraformingTool.enabled = false;
	    PainterTool.enabled = true;
	    WaterTool.enabled = false;
		Raycaster.UnlockSelection();
		
	    TerraformPanel.SetActive(false);
	    PaintPanel.SetActive(true);
	    WaterPanel.SetActive(false);
    }

    private void OnClickWaterButton() {
	    if (WaterPanel.activeInHierarchy) {
			DisableAll();
			return;
	    }

	    TerraformerPreviewer.enabled = false;
	    PainterPreviewer.enabled = false;
	    TerraformingTool.enabled = false;
	    PainterTool.enabled = false;
	    WaterTool.enabled = true;
		
	    TerraformPanel.SetActive(false);
	    PaintPanel.SetActive(false);
		WaterPanel.SetActive(true);
    }

    private void DisableAll() {
	    TerraformerPreviewer.enabled = false;
	    PainterPreviewer.enabled = false;
	    TerraformingTool.enabled = false;
	    PainterTool.enabled = false;
	    WaterTool.enabled = false;
	    TerraformPanel.SetActive(false);
	    PaintPanel.SetActive(false);
	    WaterPanel.SetActive(false);
    }

    private void OnSelectedTerrainType(ushort terrainType) {
	    PainterTool.selectedTerrainType = terrainType;
    }

    private void OnSelectCliffTerraforming() {
	    TerraformingTool.UseSmooth = false;
    }

    private void OnSelectHillTerraforming() {
	    TerraformingTool.UseSmooth = true;
    }
}
