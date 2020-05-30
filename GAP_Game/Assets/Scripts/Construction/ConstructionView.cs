using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstructionView
{
    RectTransform GetMarker();
}

public class ConstructionView : MonoBehaviour, IConstructionView
{
    ICursorController CursorControl;
    public RectTransform SelectionFrame;
    GameObject PickedBuilding;

    [Inject]
    public void Construct(ICursorController _cursor)
    {
        CursorControl = _cursor;
    }

    public void PickBuilding(PickedBuildingSignal signal)
    {
        PickedBuilding = Resources.Load(@"Buildings\" + signal.building) as GameObject;
    }

    public void BuildBuilding(BuildBuildingSignal signal)
    {

    }

    public RectTransform GetMarker()
    {
        return SelectionFrame;
    }
}
