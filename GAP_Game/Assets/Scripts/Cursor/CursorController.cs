using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;



public class CursorClickSignal
{
    public Vector3 pos;
}


public class CursorWorldPosSignal
{
    public Vector3 pos;
}

public interface ICursorController
{
    void CheckForRTSMode(CameraStateSignal signal);
}

public class CursorController : MonoBehaviour, ICursorController
{

    RaycastHit hit;
    public GameObject CursorPrefab;
    GameObject cursorInstance;
    Ray ray;
    bool cursorActive;
    SignalBus signalBus;

    [Inject]
    public void Inject(SignalBus bus)
    {
        //Debug.Log("cursor controller gets injected");
        signalBus = bus;
    }

    public void CheckForRTSMode(CameraStateSignal signal)
    {
        if (signal.state != CameraState.RTS)
        {

            cursorActive = false;
            cursorInstance.gameObject.SetActive(false);
            return;
        }

        if (cursorInstance != null)
            cursorInstance.gameObject.SetActive(true);

        cursorActive = true;
    }

    public void Update()
    {
        if (!cursorActive)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                PlayerShoot();
            }
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Camera.main == null && Camera.current == null)
            return;

        Shoot3DCursor();
    }

    void PlayerShoot()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        ClientSend.PlayerShoot(Camera.main.transform.forward);

    }
    void Shoot3DCursor()
    {
        if (Camera.current != null)
            ray = Camera.current.ScreenPointToRay(Input.mousePosition);

        if (Camera.main != null)
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (cursorInstance == null)
            {
                cursorInstance = Instantiate(CursorPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal));
                cursorInstance.gameObject.name = "Cursor";
            }

            cursorInstance.transform.position = hit.point;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            signalBus.Fire(new CursorWorldPosSignal() { pos = hit.point });
        }



        if (Input.GetMouseButtonUp(0))
        {
            signalBus.Fire(new CursorClickSignal() { pos = cursorInstance.transform.position });
        }
    }

}