using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICameraView
{
    void ActivateRTSCamera();
    void ActivateTPSCamera();
}


public class CameraView : MonoBehaviour, ICameraView
{
    public Camera LocalPlayerCamera;
    public vThirdPersonCamera TPCCam;
    public RTSCamera RTSCam;
    public Transform RTSPos;
    

    [Inject]
    public void InjectGameManager(ICameraController camControl)
    {
        camControl.InjectCameraView(this);
    }


    public void ActivateRTSCamera()
    {

    }

    public void ActivateTPSCamera()
    {

    }
}
