using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface ICameraController
{ }


public class CameraController : ScriptableObject, ICameraController, IInitializable
{
  

    [Inject]
    IGameManager gameManager;
    [Inject]
    ICameraView cameraView;


    public void Initialize()
    {
        Setup();
    }

    void Setup()
    {
        gameManager.Model.GameStateChange += CheckGameState;
    }

    void CheckGameState(GameState state)
    {
        if (state != GameState.InGame)
            return;
    }

  
}
