using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public string Name = "Stroganoff";


    public GameObject startMenu;
    public TMP_InputField usernameField;
    public TMP_InputField Port;
    public TMP_InputField IP;

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
        usernameField.text = Name;
        Port.text = port.ToString();
        IP.text = ip;
    }

    public void ConnectToServer()
    {
        try
        {
            int.TryParse(Port.text, out port);
        }
        catch
        {
            FDebug.Log.Message("Port number cound lont be read");
            return;
        }

        startMenu.SetActive(false);
        usernameField.interactable = false;
        Port.interactable = false;
        IP.interactable = false;
        Client.instance.ConnectToServer(IP.text, port);
    }
}
