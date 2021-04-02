using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IConstructionController
{
    void BuildBuilding(BuildBuildingSignal signal);
}

public class PickedBuildingSignal
{
    public BuildingData building;
}
public class BuildBuildingSignal
{
    public BuildingData Building;
}
public class DeselectBuildingSignal{}


public class ConstructionController : MonoBehaviour, IConstructionController
{
    SignalBus _signalBus;

    [Inject]
    public void Inject(SignalBus bus)
    {
        _signalBus = bus;
    }

    public void BuildBuilding(BuildBuildingSignal signal)
    {
        Debug.Log("Send buildoing to server: " + signal.Building.Name);

        using (Packet packet = new Packet((int)ServerPackets.jsonObject))
        {
            packet.Write(player.ID);

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string json = JsonConvert.SerializeObject(player.animator, settings);
            packet.Write(json);


            ClientSend.SendTCPData(packet);
        }
    }
}
