using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUIView : MonoBehaviour
{
    public RectTransform LoginPanel;
    public Camera StartScreenCamera;

    public void HideLogin()
    {
        LoginPanel.gameObject.SetActive(false);
        StartScreenCamera.enabled = false;
        StartScreenCamera.gameObject.SetActive(false);
    }

}
