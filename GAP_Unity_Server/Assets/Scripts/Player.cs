using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;
    public string name;



    private float movespeed = 5f / Constants.TicksPerSec;
    private bool[] inputs;

    public void Initialize(int id, string username)
    {
        ID = id;
        name = username;

        inputs = new bool[4];
    }


    public void FixedUpdate()
    {
        Vector2 inputDirection = Vector2.zero;
        if (inputs[0])
        {
            inputDirection.y += 1;
        }
        if (inputs[1])
        {
            inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            inputDirection.x += 1;
        }

        Move(inputDirection);

    }

    private void Move(Vector2 inputDirection)
    {



        Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        transform.position += moveDirection * movespeed;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }


    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
}
