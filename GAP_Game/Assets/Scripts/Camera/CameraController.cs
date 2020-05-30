using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;



public interface ICameraController{}
 
public enum CameraState { TPS, RTS, Lobby }

public class CameraStateSignal
{
    public CameraState state;
}


public class CameraController : MonoBehaviour, ICameraController
{
    bool toggleRTS = true;
    SignalBus signalBus;


    [Inject]
    public void Inject(SignalBus bus)
    {
        signalBus = bus;
    }

    public void CheckGameState(GameStateChangedSignal signal)
    {
        if (signal.state != GameState.InGame)
            return;
    }


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            if(toggleRTS)
            {
                signalBus.Fire(new CameraStateSignal() { state = CameraState.RTS });
                toggleRTS = !toggleRTS;
            }
            else
            {
                signalBus.Fire(new CameraStateSignal() { state = CameraState.TPS });
                toggleRTS = !toggleRTS;
            }
        }
    }


}
