using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstrcuController
{
    Building Selection { get; set; }
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


    public Building Selection { get; set; }
    GameObject buldngInstance;

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

        if (PickBuilding != null)
            PickBuilding.Invoke(Selection);
    }
}
