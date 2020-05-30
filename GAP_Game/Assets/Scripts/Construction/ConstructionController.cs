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
    public void Construct(SignalBus bus)
    {
        signalBus = bus;
    }

    public void CheckCameraState(CameraStateSignal signal) // if camera is in RTS mode I subscribe to the cursor
    {
        if (signal.state != CameraState.RTS)
        {
            Selection = null;
            Destroy(buldngInstance);
            return;
        }
    }

    public void ListenForClick(CursorClickSignal signal) // Im in RTS mode and there is a click
    {
        Instantiate(buldngInstance, signal.pos, Quaternion.identity);
    }

    public void ListenForPos(CursorWorldPosSignal signal)
    {
        if (Selection == null)
            return;

        buldngInstance.SetActive(true);
        buldngInstance.transform.position = signal.pos;
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
