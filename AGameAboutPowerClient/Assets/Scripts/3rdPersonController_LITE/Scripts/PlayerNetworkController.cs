using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    public vThirdPersonInput PlayerInput;
    public Rigidbody PlayerPhysics;

    public void Inject(bool isLocal)
    {
        if(isLocal)
        {
            PlayerInput.isLocalPlayer = true;
        }
        else
        {
            PlayerInput.isLocalPlayer = false;
            Destroy(PlayerPhysics);
        }
    }

}
