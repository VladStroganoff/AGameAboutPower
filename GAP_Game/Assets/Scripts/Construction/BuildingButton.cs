using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuildingButton : MonoBehaviour
{
    public BuildingData MyBuilding;
    IConstructionView ConstructionView;
    IConstructionController ConControl;

    [Inject]
    public void Construct(IConstructionController _conControl, IConstructionView _conView)
    {
        ConControl = _conControl;
        ConstructionView = _conView;
    }

    public void PickBuilding()
    {
        MoveMarker();
        ConControl.SelectBuilding(MyBuilding);
    }

    void MoveMarker()
    {
        if (ConControl.Selection == MyBuilding)
        {
            Deselect();
            return;
        }

        ConstructionView.GetMarker().gameObject.SetActive(true);
        ConstructionView.GetMarker().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    void Deselect()
    {
        ConstructionView.GetMarker().gameObject.SetActive(false);
    }
}
