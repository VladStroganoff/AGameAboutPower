using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICameraView
{
}


public class CameraView : MonoBehaviour, ICameraView
{
    public Camera LocalPlayerCamera;
    public vThirdPersonCamera TPSCam;
    public vThirdPersonInput TPSInput;
    public RTSCamera RTSCam;
    public Transform RTSPos;
    public Transform CameraParent;
    Vector3 oldPos;
    Quaternion oldRot;

    float startTime = 0f;
    float journeyLength = 0;


    [Inject]
    public void Construct(ICameraController _camControl)
    {
        _camControl.CameraStateChange += CheckCameraState;
    }


    public void CheckCameraState(CameraState state)
    {
        switch(state)
        {
            case CameraState.RTS:
                {
                    TPSCam.enabled = false;
                    TPSInput.enabled = false;
                    oldPos = LocalPlayerCamera.transform.position;
                    oldRot = LocalPlayerCamera.transform.rotation;
                    LocalPlayerCamera.transform.position = RTSPos.position;
                    LocalPlayerCamera.transform.rotation = RTSPos.rotation;
                    RTSCam.enabled = true;
                    return;
                }
            case CameraState.TPS:
                {
                    RTSCam.enabled = false;
                    CameraParent.transform.localPosition = Vector3.zero;
                    LocalPlayerCamera.transform.position = oldPos;
                    LocalPlayerCamera.transform.rotation = oldRot;
                    TPSInput.enabled = true;
                    TPSCam.enabled = true;
                    return;
                }
        }
    }


    public IEnumerator BackAndForth(Vector3 pos, Quaternion rot)
    {
        journeyLength = Vector3.Distance(pos, LocalPlayerCamera.transform.position);

        while (Vector3.Distance(LocalPlayerCamera.transform.position, pos) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * 5;
            float fractionOfJourney = distCovered / journeyLength;
            LocalPlayerCamera.transform.position = Vector3.Lerp(LocalPlayerCamera.transform.position, pos, fractionOfJourney);
            LocalPlayerCamera.transform.rotation = Quaternion.Lerp(LocalPlayerCamera.transform.rotation, rot, fractionOfJourney);
            yield return new WaitForEndOfFrame();
        }
    }

}
