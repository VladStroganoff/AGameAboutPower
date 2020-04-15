using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedAnimator : MonoBehaviour
{
    NetAnimator NetAnimations;
    NetAnimator oldState;

    private void FixedUpdate()
    {
        if (GetComponent<Animator>() == null)
            return;

        if (GetComponent<Player>() == null)
            return;

        NetAnimations = MakeNet.Animator(GetComponent<Animator>());


        if (NetAnimations == oldState)
            return;


        oldState = NetAnimations;


        GetComponent<Player>().animator = NetAnimations;
        ServerSend.JsonObject(GetComponent<Player>());
    }

}
