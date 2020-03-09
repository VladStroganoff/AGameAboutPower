using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public EntityController EntityControl;
    public int ConnectionID;

    public WorldController PlayerControl;
    public InputField IpField;
    public InputField PortField;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();


        IpField.text = "10.0.0.4";
        PortField.text = "5587";
    }

    public void TryInitializeConnection()
    {

        ClientHandleData.InitializePackets();
        FDebug.Log.Message("Initializing Packets...");
        ClientTCP.InitializeNetworking(IpField.text, int.Parse(PortField.text));
        FDebug.Log.Message("Initializing Networking...");
    }


    private void OnApplicationQuit()
    {
        ClientTCP.Disconnect();
    }


    public void HandleEntity(NetEntity entity)
    {
        EntityControl.HandleEntity(entity);
    }

    public void LocalPlayerConnectionID(int id)
    {
        ConnectionID = id;
    }

    public void ErrorMessage(string msg)
    {
        Debug.Log(msg);
    }


   

    
}
