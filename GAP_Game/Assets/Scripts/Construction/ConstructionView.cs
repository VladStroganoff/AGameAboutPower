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
    IConstrcuController ConControl;
    ICursorController CursorControl;
    public RectTransform SelectionFrame;
    GameObject PickedBuilding;

    [Inject]
    public void Construct(IConstrcuController _conControl, ICursorController _cursor)
    {
        ConControl = _conControl;
        CursorControl = _cursor;
        ConControl.PickBuilding += PickBuilding;
        ConControl.BuildBuilding += BuildBuilding;
    }

    void PickBuilding(Building _building)
    {
        PickedBuilding = Resources.Load(@"Buildings\" + _building.Name) as GameObject;
    }

    public void BuildBuilding()
    {

    }

    public RectTransform GetMarker()
    {
        return SelectionFrame;
    }
}
