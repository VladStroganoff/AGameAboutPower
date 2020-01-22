using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNavigationView : MonoBehaviour
{

    public float MoveMentSpeed;

    void Update()
    {


        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + new Vector3(MoveMentSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + new Vector3(-MoveMentSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + new Vector3(0, 0, MoveMentSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + new Vector3(0, 0, -MoveMentSpeed * Time.deltaTime);
        }
    }
}
