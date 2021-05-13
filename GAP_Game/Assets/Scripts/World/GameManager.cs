using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum GameState { InLobby, InGame, Paused,}

public interface IGameManager
{
    WorldModel Model { get; set; }
    void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation);
    void SpawnStructure(BuildingData data);
}

public class GameManager : MonoBehaviour, IGameManager
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public GameObject Player; 
    public GameObject OtherPlayer;
    [SerializeField]
    private ConstructionController _constructionController;
    private IInventoryController _inventoryControl;
    public WorldModel Model { get; set; }
    public static GameManager instance;

    [Inject]
    public void Inject(SignalBus bus, IInventoryController inventoryControl)
    {
        Model  = new WorldModel(bus);
        instance = this;
        _inventoryControl = inventoryControl;
    }

    public void SpawnPlayer(int id, string playerData, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == GameClient.instance.myId)
        {
            player = Instantiate(Player, position, rotation);
        }
        else
        {
            player = Instantiate(OtherPlayer, position, rotation);
        }

        player.GetComponent<PlayerManager>().Initialize(id, playerData);
        players.Add(id, player.GetComponent<PlayerManager>());

        Model.SetGameState(GameState.InGame);
        Model.AddPlayer(player);
    }

    public void SpawnStructure(BuildingData data)
    {
        _constructionController.ReceiveBuilding(data);
    }

    public void ChangeInventory(int playerID, Item item)
    {
        _inventoryControl.ChangeInventory(playerID, item);
    }

    public void  TestConnectToServer()
    {
        GameClient.instance.ConnectToServer("127.0.0.1", 26950);
        GameObject.Find("Menu").SetActive(false);
    }
}
