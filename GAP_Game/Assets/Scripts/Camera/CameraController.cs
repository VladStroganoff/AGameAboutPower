using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface ICameraController
{
    void InjectCameraView(ICameraView camView);
}


public class CameraController : MonoBehaviour, ICameraController
{
  

    IGameManager gameManager;
    ICameraView cameraView;


    [Inject]
    public void InjectGameManager(IGameManager manager)
    {
        gameManager = manager;
    }

    public void InjectCameraView(ICameraView camView)
    {
        cameraView = cameraView;
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
