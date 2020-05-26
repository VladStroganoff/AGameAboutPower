using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstrcuController
{
    Building Selection { get; set; }
    void SelectBuilding(Building building);
}

public class PickedBuildingSignal
{
    public Building building;
}
public class BuildBuildingSignal{}
public class DeselectBuildingSignal{}


public class ConstructionController : MonoBehaviour, IConstrcuController
{
    ConstructionModel Model;
    ICursorController CursorControl;

    SignalBus signalBus;


    public Building Selection { get; set; }

    GameObject buldngInstance;

    [Inject]
    public void Construct(ICameraController _camCon, ICursorController cursor, SignalBus bus)
    {
        _camCon.CameraStateChange += CheckCameraState;
        CursorControl = cursor;
        signalBus = bus;
    }

    void CheckCameraState(CameraState state) // if camera is in RTS mode I subscribe to the cursor
    {
        if (state != CameraState.RTS)
        {
            CursorControl.click -= ListenForClick;
            CursorControl.cursorWorldPos -= ListenForPos;
            Selection = null;
            Destroy(buldngInstance);
            return;
        }
        else
        {
            CursorControl.click += ListenForClick;
            CursorControl.cursorWorldPos += ListenForPos;
        }
    }

    void ListenForClick(Vector3 _click) // Im in RTS mode and there is a click
    {
        Instantiate(buldngInstance, _click, Quaternion.identity);
    }

    void ListenForPos(Vector3 pos)
    {
        if (Selection == null)
            return;

        buldngInstance.SetActive(true);
        buldngInstance.transform.position = pos;
    }

    public void SelectBuilding(Building _building)
    {
        Selection = _building;
        Destroy(buldngInstance);
        buldngInstance = Instantiate(Resources.Load<GameObject>("Buildings/" + Selection.Name) as GameObject, Vector3.zero, Quaternion.identity);
        buldngInstance.name = Selection.Name;
        buldngInstance.SetActive(false);

         signalBus.Fire(new PickedBuildingSignal() { building = Selection });
    }
}
