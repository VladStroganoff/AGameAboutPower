using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameSignView : MonoBehaviour
{

    public RectTransform PlayerCanvas;
    public TextMeshProUGUI Text;
    public Camera mainCamera;
    public int ID;
    public string Name;

    public void Inject(int playerID)
    {
        mainCamera = FindObjectOfType<Camera>();
        ID = playerID;
        Text.text = playerID.ToString();
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        if (mainCamera.transform != null)
        {
            this.enabled = false;
            return;
        }


        PlayerCanvas.transform.LookAt(mainCamera.transform);
    }
}
