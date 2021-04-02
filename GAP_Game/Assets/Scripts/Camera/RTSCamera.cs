using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RTSCamera : MonoBehaviour
{
    public enum NavigationState
    {
        Pan, Rotate, Zoom, None
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

    private Transform startPose;
   


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

        startPose = transform;

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
        transform.LookAt(Target.position);
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

        currentState = NavigationState.Zoom;
    }

    void Pan()
    {
        Vector3 delta = (Input.mousePosition - _mouseReference)*2;
        transform.parent.Translate(-delta.x * 0.02f, 0, -delta.y * 0.02f, Space.Self);
        _mouseReference = Input.mousePosition;
    }

    void Zoom()
    {

        if (Input.mouseScrollDelta.y == 0)
            return;

        Vector3 zoomInDest = Vector3.zero;

        if (Input.mouseScrollDelta.y < 0)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Target.localPosition, Input.mouseScrollDelta.y * Vector3.Distance(transform.position, Target.position) / 30); // zoom out
        }
        else
        {
            zoomInDest = Vector3.MoveTowards(transform.localPosition, Target.localPosition, Input.mouseScrollDelta.y * Vector3.Distance(transform.position, Target.position) / 30);
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
            verticalRot = transform.parent.TransformVector(Vector3.left);
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
        transform.parent.RotateAround(Target.transform.position, Vector3.up, horizontal);

        vertical = Input.GetAxis("Mouse Y") * Sensitivity;
        transform.RotateAround(Target.transform.position, transform.parent.TransformVector(Vector3.left), vertical);

        transform.LookAt(Target.transform);
    }


   
}
