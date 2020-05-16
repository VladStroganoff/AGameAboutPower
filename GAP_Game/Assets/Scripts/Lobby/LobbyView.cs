using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LobbyView : MonoBehaviour
{

    public Camera LobbyCamera;


    [Inject]
    IGameManager gameManager;

    void Start()
    {
        gameManager.Model.GameStateChange += CheckState;
    }

    void CheckState(GameState state)
    {
        if (state == GameState.InLobby)
            return;

        if (state == GameState.InGame)
            gameObject.SetActive(false);
    }

}
