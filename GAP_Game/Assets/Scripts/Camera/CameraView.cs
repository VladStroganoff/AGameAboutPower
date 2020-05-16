using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public void ActivateRTSCamera()
    {
    }

    public void ActivateTPSCamera()
    {

    }
}
