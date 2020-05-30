using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class GameStateChangedSignal
{
    public GameState state;
}

public interface IWorldModel
{}

public class WorldModel: IWorldModel
{
    GameState CurrentState;
    public List<GameObject> Players = new List<GameObject>();

    SignalBus signalBus;

    public WorldModel(SignalBus bus)
    {
        signalBus = bus;
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        signalBus.Fire(new GameStateChangedSignal() { state = newState });
    }

    public void AddPlayer(GameObject player)
    {
        Players.Add(player);
    }

    
}
