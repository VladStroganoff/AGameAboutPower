﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;



public interface ICameraController
{
    void CheckGameState(GameStateChangedSignal signal);
}
 
public enum CameraState { TPS, RTS, Lobby, Menu }

public class CameraStateSignal
{
    public CameraState state;
}


public class CameraController : MonoBehaviour, ICameraController
{
    bool toggleRTS = true;
    SignalBus _signalBus;


    [Inject]
    public void Inject(SignalBus bus)
    {
        //Debug.Log("Camera controller gets injected");
        _signalBus = bus;
    }

    public void CheckGameState(GameStateChangedSignal signal)
    {
        if (signal.state != GameState.InGame)
            return;
    }


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.B))
        {
            if(toggleRTS)
            {
                _signalBus.Fire(new CameraStateSignal() { state = CameraState.RTS });
                toggleRTS = !toggleRTS;
            }
            else
            {
                _signalBus.Fire(new CameraStateSignal() { state = CameraState.TPS });
                toggleRTS = !toggleRTS;
            }
        }
    }


}
