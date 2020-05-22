using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstrcuController
{
    Building SelectedBuilding { get; set; }
    PickBuilding PickBuilding { get; set; }
    BuildBuilding BuildBuilding { get; set; }
    void SelectBuilding(Building building);
}

public delegate void PickBuilding(Building building);
public delegate void BuildBuilding();

public class ConstructionController : MonoBehaviour, IConstrcuController
{
    ConstructionModel Model;
    ICursorController CursorControl;

    public PickBuilding PickBuilding { get; set; }
    public BuildBuilding BuildBuilding { get; set; }

    public Building SelectedBuilding { get; set; }

    [Inject]
    public void Construct(ICameraController _camCon, ICursorController cursor)
    {
        _camCon.CameraStateChange += CheckCameraState;
        CursorControl = cursor;
    }

    void CheckCameraState(CameraState state) // if camera is in RTS mode I subscribe to the cursor
    {
        if (state != CameraState.RTS)
        {
            CursorControl.click -= ListenForClick;
            SelectedBuilding = null;
            return;
        }
        else
        {
            CursorControl.click += ListenForClick;
        }
    }

    void ListenForClick(Vector2 _click) // Im in RTS mode and there is a click
    {

    }

    public void SelectBuilding(Building _building)
    {
        SelectedBuilding = _building;

        if (PickBuilding != null)
            PickBuilding.Invoke(SelectedBuilding);
    }
}
