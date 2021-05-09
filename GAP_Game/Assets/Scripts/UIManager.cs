using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;


public interface IUIManager
{
    void CheckCameraState(CameraStateSignal signal);
}

public class UIManager : MonoBehaviour, IUIManager
{
    public static UIManager instance;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public string Name = "Stroganoff";

    public RectTransform RTSCanvas;
    public GameObject startMenu;
    Vector2 startMenuOrigin;
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
        startMenuOrigin = startMenu.GetComponent<RectTransform>().anchoredPosition;
        startMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(startMenu.GetComponent<RectTransform>().anchoredPosition.x, 0);
    }

    public void CheckCameraState(CameraStateSignal signal)
    {
        switch (signal.state)
        {
            case CameraState.RTS:
                {
                    RTSCanvas.gameObject.SetActive(true);
                    RTSCanvas.anchoredPosition = new Vector3(0, 0, 0);
                    return;
                }
            case CameraState.TPS:
                {
                    RTSCanvas.gameObject.SetActive(false);
                    RTSCanvas.anchoredPosition = new Vector3(0, -2000, 0);
                    return;
                }
        }
    }



    public void ConnectToServer() // from join button
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
        startMenu.GetComponent<RectTransform>().anchoredPosition = startMenuOrigin;
        usernameField.interactable = false;
        Port.interactable = false;
        IP.interactable = false;
        GameClient.instance.ConnectToServer(IP.text, port);
    }
}
