using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstructionView
{
    RectTransform GetMarker();
    void CheckCameraState(CameraStateSignal signal);
    void ListenForClick(CursorClickSignal signal);
    void ListenForPos(CursorWorldPosSignal signal);
    void SelectBuilding(BuildingView building);
    BuildingView GetBuilding();
}

public class ConstructionView : MonoBehaviour, IConstructionView
{
    SignalBus _signalBus;
    public RectTransform SelectionFrame;
    BuildingView _selection { get; set; }
    BuildingView _buldngCursor;
    public int GridStep;
    Dictionary<string, BuildingView> _selectedBuildings = new Dictionary<string, BuildingView>();

    [Inject]
    public void Inject(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void CheckCameraState(CameraStateSignal signal) // if camera is in RTS mode I subscribe to the cursor
    {
        if (signal.state != CameraState.RTS)
        {
            _selection = null;
            Destroy(_buldngCursor);
            return;
        }
    }

    public void ListenForClick(CursorClickSignal signal)
    {
        if (_buldngCursor == null)
            return;

        BuildingData data = _buldngCursor.GetData();
        _signalBus.Fire(new SendBuildingSignal() { Building = data });
    }

    public RectTransform GetMarker() => SelectionFrame;

    public void ListenForPos(CursorWorldPosSignal signal)
    {
        if (_selection == null)
            return;

        _buldngCursor.gameObject.SetActive(true);
        _buldngCursor.transform.position = ConformToGrid(signal.pos);
    }


    Vector3 ConformToGrid(Vector3 pos) => new Vector3(Mathf.Round(pos.x / GridStep) * GridStep, Mathf.Round(pos.y / GridStep) * GridStep, Mathf.Round(pos.z / GridStep) * GridStep);

    public void SelectBuilding(BuildingView building)
    {
        if (_selection != building && _selection != null)
            _buldngCursor.gameObject.SetActive(false);

        if(!_selectedBuildings.ContainsKey(building.name))
        {
            _selection = building;
            BuildingView cursor = Instantiate(_selection);
            _selectedBuildings.Add(building.name, cursor);
            _buldngCursor = cursor;
        }
        else
        {
            _selection = _selectedBuildings[building.name];
            _buldngCursor = _selection;
            _buldngCursor.gameObject.SetActive(true);
        }
    }
    public BuildingView GetBuilding() =>  _selection;
}
