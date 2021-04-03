using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum GameState { InLobby, InGame, Paused,}

public interface IGameManager
{
    WorldModel Model { get; set; }
    void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation);
}

public class GameManager : MonoBehaviour, IGameManager
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public WorldModel Model { get; set; }
    public static GameManager instance;
    public CameraView.Factory _camFack;

    [Inject]
    public void Contruct(SignalBus bus, CameraView.Factory camFack)
    {
        Model  = new WorldModel(bus);
        instance = this;
        _camFack = camFack;
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == Client.instance.myId)
        {
            player = _camFack.Create().gameObject;
        }
        else
        {
            player = Instantiate(playerPrefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().Initialize(id, username);
        players.Add(id, player.GetComponent<PlayerManager>());

        Model.SetGameState(GameState.InGame);
        Model.AddPlayer(player);
    }
}
