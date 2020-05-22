using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuildingButton : MonoBehaviour
{
    public Building MyBuilding;
    public IConstrcuController ConControl;

    [Inject]
    public void Construct(IConstrcuController _conControl)
    {
        ConControl = _conControl;
    }

    public void PickBuilding()
    {
        ConControl.SelectBuilding(MyBuilding);
    }
}
