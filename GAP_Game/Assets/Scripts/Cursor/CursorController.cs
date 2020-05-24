using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface ICursorController
{
    CursorClick click { get; set; }
    CursorWorldPos cursorWorldPos { get; set; }
}
public delegate void CursorClick(Vector3 pos);
public delegate void CursorWorldPos(Vector3 pos);

public class CursorController : MonoBehaviour, ICursorController
{
    public CursorClick click { get; set; }
    public CursorWorldPos cursorWorldPos { get; set; }

    RaycastHit hit;
    public GameObject CursorPrefab;
    GameObject cursorInstance;
    Ray ray;
    bool cursorActive;

    [Inject]
    public void Construct(ICameraController _camControl)
    {
        _camControl.CameraStateChange += CheckForRTSMode;
    }

    void CheckForRTSMode(CameraState state)
    {
        if(state != CameraState.RTS)
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
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Camera.main == null && Camera.current == null)
            return;

        Shoot3DCursor();
    }


    void Shoot3DCursor()
    {
        if(Camera.current != null)
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
            cursorWorldPos.Invoke(hit.point);
        }



        if (Input.GetMouseButtonUp(0))
        {
            if (click != null)
                click.Invoke(cursorInstance.transform.position);
        }
    }

}