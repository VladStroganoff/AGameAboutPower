using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldModel
{
    GameState CurrentState;
    public delegate void GameStateChanged(GameState state);
    public GameStateChanged GameStateChange;
    public List<GameObject> Players = new List<GameObject>();

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        GameStateChange.Invoke(newState);
    }

    public void AddPlayer(GameObject player)
    {
        Players.Add(player);
    }


}
