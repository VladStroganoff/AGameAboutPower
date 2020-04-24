using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyView : MonoBehaviour
{

    public Camera LobbyCamera;

    void Start()
    {
        GameManager.instance.Model.GameStateChange += CheckState;
    }

    void CheckState(GameState state)
    {
        if (state == GameState.InLobby)
            return;

        if (state == GameState.InGame)
            gameObject.SetActive(false);
    }

}
