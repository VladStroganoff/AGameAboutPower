using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    public vThirdPersonInput playerInput;

    public void Inject(int localID, int id)
    {
        if(id != localID)
        {
            playerInput.isLocalPlayer = false;
        }
        else
        {
            playerInput.isLocalPlayer = true;
        }
    }

}
