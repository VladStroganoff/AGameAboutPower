using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstructionController
{
    void SendBuilding(SendBuildingSignal signal);
    void ReceiveBuilding(BuildingData signal);
}

public class PickedBuildingSignal
{
    public BuildingData building;
}
public class SendBuildingSignal
{
    public BuildingData Building;
}

public class DeselectBuildingSignal{}


public class ConstructionController : MonoBehaviour, IConstructionController
{

    ILoadController LoadControl;


    [Inject]
    public void Inject(ILoadController loader)
    {
        LoadControl = loader;
    }

    public void SendBuilding(SendBuildingSignal signal)
    {
        Debug.Log("Send buildoing to server: " + signal.Building.Name);
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        string json = JsonConvert.SerializeObject(signal.Building, settings);
        ClientSend.SendJsonPackage(json);
    }

    public void ReceiveBuilding(BuildingData building)
    {
        Debug.Log($"Building building: {building.Name} at position: {building.Position}, and rotation: {building.Rotation}");
        LoadControl.LoadBuilding(building);
    }
}
