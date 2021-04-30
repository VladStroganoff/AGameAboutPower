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
    GameStateChangedSignal gameState;

    SignalBus _signalBus;
    [Inject]
    public WorldModel(SignalBus bus)
    {
        _signalBus = bus;
    }


    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        _signalBus.Fire(new GameStateChangedSignal() { state = newState });
    }

    public void AddPlayer(GameObject player)
    {
        Players.Add(player);
    }

    
}
