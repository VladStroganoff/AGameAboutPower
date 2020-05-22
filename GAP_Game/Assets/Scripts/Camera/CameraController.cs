using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;



public interface ICameraController
{
    CameraStateChanged CameraStateChange { get; set; }
}


public delegate void CameraStateChanged(CameraState state);
public enum CameraState { TPS, RTS, Lobby }


public class CameraController : MonoBehaviour, ICameraController
{
    public CameraStateChanged CameraStateChange { get; set; }
    bool toggleRTS = true;


    [Inject]
    public void Inject(IGameManager manager)
    {
        manager.Model.GameStateChange += CheckGameState;
    }

    void CheckGameState(GameState state)
    {
        if (state != GameState.InGame)
            return;
    }


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            if(toggleRTS)
            {
                CameraStateChange.Invoke(CameraState.RTS);
                toggleRTS = !toggleRTS;
            }
            else
            {
                CameraStateChange.Invoke(CameraState.TPS);
                toggleRTS = !toggleRTS;
            }
        }
    }


}
