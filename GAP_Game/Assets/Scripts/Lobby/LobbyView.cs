using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LobbyView : MonoBehaviour
{

    public Camera LobbyCamera;


    public void CheckState(GameStateChangedSignal signal)
    {
        if (signal.state == GameState.InLobby)
            return;

        if (signal.state == GameState.InGame)
            gameObject.SetActive(false);
    }

}
