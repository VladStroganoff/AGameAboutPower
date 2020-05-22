using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public WorldModel Model { get; set; } = new WorldModel();
    public static GameManager instance;


    private void Awake()
    {
        instance = this;
        Model.SetGameState(GameState.InLobby);
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
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
