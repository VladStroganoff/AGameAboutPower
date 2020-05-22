using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ConstructionView : MonoBehaviour
{
     IConstrcuController ConControl;
     ICursorController CursorControl;
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
}
