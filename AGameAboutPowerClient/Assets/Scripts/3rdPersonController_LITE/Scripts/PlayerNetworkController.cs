using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    public vThirdPersonInput playerInput;

    public void Inject(bool isLocal)
    {
        if(isLocal)
        {
            playerInput.isLocalPlayer = true;
        }
        else
        {
            playerInput.isLocalPlayer = false;
        }
    }

}
