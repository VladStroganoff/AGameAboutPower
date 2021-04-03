using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuildingButton : MonoBehaviour
{
    public BuildingView MyBuilding;
    IConstructionView _constructionView;
    IConstructionController ConControl;

    [Inject]
    public void Construct(IConstructionController _conControl, IConstructionView conView)
    {
        ConControl = _conControl;
        _constructionView = conView;
    }

    public void PickBuilding()
    {
        MoveMarker();
        _constructionView.SelectBuilding(MyBuilding);
    }

    void MoveMarker()
    {
        if (_constructionView.GetBuilding() == MyBuilding)
        {
            Deselect();
            return;
        }

        _constructionView.GetMarker().gameObject.SetActive(true);
        _constructionView.GetMarker().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    void Deselect()
    {
        _constructionView.GetMarker().gameObject.SetActive(false);
    }
}
