using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public void BuildBuilding(int player, BuildingData building)
    {
        Debug.Log($"Building building: {building.Name} at position: {building.Position}, and rotation: {building.Rotation}");
        GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>($"Buildings/{building.Name}"), building.Position, building.Rotation);
        ServerSend.BuildStructure(player, building);
    }
}
