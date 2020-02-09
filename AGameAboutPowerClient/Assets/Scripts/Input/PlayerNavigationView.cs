using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNavigationView : MonoBehaviour
{

    public NetworkManager NetManager;

    void Update()
    {


        if (Input.GetKey(KeyCode.A))
        {
            PlayerData data = new PlayerData();

            data.Name = "My name?";

            NetManager.UpdatePlayerPosition(data);
        }
    }
}
