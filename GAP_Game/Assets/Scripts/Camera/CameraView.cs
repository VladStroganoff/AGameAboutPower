using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICameraView
{
    void CheckCameraState(CameraStateSignal signal);
}

public class CameraView : MonoBehaviour, ICameraView
{
    public Camera LocalPlayerCamera;
    public vThirdPersonCamera TPSCam;
    public RTSCamera RTSCam;
    public Transform RTSPos;
    public Transform TPSPos;
    public Transform CameraParent;
    CameraState stateHistory = CameraState.TPS;

    float startTime = 0f;
    float journeyLength = 0;
    bool traveling = false;

    [Inject]
    public void Inject(SignalBus bus)
    {
        bus.Subscribe<CameraStateSignal>(CheckCameraState);
        CameraParent.parent = null;
        RTSCam.enabled = false;
        CameraParent.transform.position = transform.position;
    }

    public void CheckCameraState(CameraStateSignal signal)
    {
        switch (signal.state)
        {
            case CameraState.RTS:
                {
                    if (stateHistory != CameraState.Menu)
                    {
                        StartCoroutine(BackAndForth(RTSPos.localPosition, RTSPos.rotation, false));
                        LocalPlayerCamera.transform.position = TPSPos.position;
                    }
                    TPSCam.enabled = false;
                    RTSCam.enabled = true;
                    break;
                }
            case CameraState.TPS:
                {
                    if (stateHistory != CameraState.Menu)
                    {
                        LocalPlayerCamera.transform.position = RTSPos.position;
                        StartCoroutine(BackAndForth(TPSPos.localPosition, TPSPos.rotation, true));
                    }
                    TPSCam.enabled = true;
                    RTSCam.enabled = false;
                    break;
                }
            case CameraState.Menu:
                {
                    TPSCam.enabled = false;
                    RTSCam.enabled = false;
                    break;
                }
        }

        stateHistory = signal.state;

    }


    public IEnumerator BackAndForth(Vector3 pos, Quaternion rot, bool yenah)
    {
        if (traveling)
            yield return null;


        traveling = true;
        journeyLength = Vector3.Distance(pos, LocalPlayerCamera.transform.localPosition);

        while (Vector3.Distance(LocalPlayerCamera.transform.localPosition, pos) > 0.3f)
        {
            float distCovered = (Time.time - startTime) * 0.5f;
            float fractionOfJourney = distCovered / journeyLength;
            LocalPlayerCamera.transform.localPosition = Vector3.Lerp(LocalPlayerCamera.transform.localPosition, pos, fractionOfJourney);
            LocalPlayerCamera.transform.rotation = Quaternion.Lerp(LocalPlayerCamera.transform.rotation, rot, fractionOfJourney);
            yield return new WaitForEndOfFrame();
        }

        if (yenah)
        {
            TPSCam.enabled = true;
        }
        else
        {
            RTSCam.transform.LookAt(RTSCam.Target.transform.position);
            RTSCam.enabled = true;
        }

        traveling = false;
    }

}
