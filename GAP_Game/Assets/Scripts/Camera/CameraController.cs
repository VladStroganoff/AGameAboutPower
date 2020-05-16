using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface ICameraController
{ }


public class CameraController : MonoBehaviour, ICameraController
{
    public Camera LocalPlayerCamera;
    public vThirdPersonCamera TPCCam;
    public RTSCamera RTSCam;
    public Transform RTSPos;

    [Inject]
    public IGameManager gameManager;

 

    private void Start()
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
