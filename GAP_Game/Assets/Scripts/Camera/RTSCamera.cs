using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RTSCamera : MonoBehaviour
{
    public enum NavigationState
    {
        Pan, Rotate, Zoom, TightRotate, None
    }
    public NavigationState currentState;
    public Transform Target;
    public float Sensitivity;

    private float horizontal = 0;
    private float vertical = 0;

    private Vector3 _mouseReference;
    private bool startPan;
    private Vector3 _mouseOffset;
    Vector3 localTarget;
    private Vector3 verticalRot;

    private Vector3 startCamPos;
    private Quaternion stratCamRot;

    private Vector3 startCurPos;
    private Quaternion stratCurRot;

    private Vector3 oldPosition;


    void Start()
    {
        Setup();
    }

    void Setup()
    {
        transform.LookAt(Target.transform);

        startCamPos = transform.position;
        stratCamRot = transform.rotation;

        startCurPos = Target.position;
        stratCurRot = Target.rotation;

        localTarget = transform.InverseTransformPoint(Target.position);
        _mouseReference = Input.mousePosition;
    }
    void Update()
    {



        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyUp(KeyCode.Z))
            ResetPosition();

        MouseInput();

        switch (currentState)
        {
            case NavigationState.None:
                return;
            case NavigationState.Rotate:
                Rotate();
                return;
            case NavigationState.Pan:
                Pan();
                return;
            case NavigationState.Zoom:
                Zoom();
                return;
        }
    }


    public void AssumePosition(Transform position)
    {
        transform.position = position.position;
        transform.rotation = position.rotation;
        Target.position = transform.TransformPoint(localTarget);
    }

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(2))
        {
            StoreMouse(true);
        }

        if (Input.GetMouseButtonUp(2))
        {
            StoreMouse(false);
        }


        if (Input.GetMouseButton(0))
        {
            currentState = NavigationState.Rotate;
            return;
        }
        else if (Input.GetMouseButton(2))
        {
            currentState = NavigationState.Pan;
            return;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentState = NavigationState.TightRotate;
            return;
        }

        currentState = NavigationState.Zoom;
    }

    void Pan()
    {
        Vector3 delta = (Input.mousePosition - _mouseReference)*2;
        transform.Translate(-delta.x * 0.02f, -delta.y * 0.02f, 0);

        Target.position = transform.TransformPoint(localTarget);
        _mouseReference = Input.mousePosition;
    }

    void Zoom()
    {

        if (Input.mouseScrollDelta.y == 0)
            return;

        Vector3 zoomInDest = Vector3.zero;

        if (Input.mouseScrollDelta.y < 0)
        {
            transform.localPosition = Vector3.MoveTowards(transform.position, transform.TransformPoint(localTarget), Input.mouseScrollDelta.y * Vector3.Distance(transform.position, Target.position) / 30); // zoom out
        }
        else
        {
            zoomInDest = Vector3.MoveTowards(transform.position, transform.TransformPoint(localTarget), Input.mouseScrollDelta.y * Vector3.Distance(transform.position, Target.position) / 30);
        }


        if (Input.mouseScrollDelta.y > 0) // zoom in
        {
            transform.localPosition = zoomInDest;
        }
    }

    void StoreMouse(bool yenah)
    {
        if (yenah && !startPan)
        {
            startPan = true;
            verticalRot = transform.TransformVector(Vector3.left);
            _mouseReference = Input.mousePosition;
        }
        else
        {
            startPan = false;
        }
    }


    void Rotate()
    {

        horizontal = Input.GetAxis("Mouse X") * Sensitivity;
        vertical = Input.GetAxis("Mouse Y") * Sensitivity;

        transform.RotateAround(Target.transform.position, Vector3.up, horizontal);
        transform.RotateAround(Target.transform.position, verticalRot, vertical);
        transform.LookAt(Target.transform);
        localTarget = transform.InverseTransformPoint(Target.position);
    }

    public void ResetPosition()
    {
        transform.position = startCamPos;
        transform.rotation = stratCamRot;

        Target.transform.position = startCurPos;
        Target.transform.rotation = stratCurRot;

        localTarget = transform.InverseTransformPoint(startCurPos);
    }


}
