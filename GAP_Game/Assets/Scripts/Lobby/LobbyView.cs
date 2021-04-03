using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ILobbyView
{
    void CheckGameState(GameStateChangedSignal signal);
}


public class LobbyView : MonoBehaviour, ILobbyView
{

    public Camera LobbyCamera;

    public void CheckGameState(GameStateChangedSignal signal)
    {
        if (signal.state == GameState.InLobby)
            return;

        if (signal.state == GameState.InGame)
            gameObject.SetActive(false);
    }

}
