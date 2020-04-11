using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager instance;
    public GameObject PlayerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

#if UNITY_EDITOR
        Debug.Log("fuck off, build and run the server! no running the server from the editor you fool.");
#else
        Server.Start(50, 26950);

#endif

    }

    public Player InstantiatePlayer()
    {
        return Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }

}
