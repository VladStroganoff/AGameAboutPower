using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;
    public string name;
    public vThirdPersonInput thirdPersonInput;
    public vThirdPersonMotor thirdPersonMotor;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float movespeed = 5f;
    public float jumpspeed = 5f;
    public float health;
    public float maxHealth;

    private bool[] inputs;
    private float yVelocity = 0;


    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        movespeed *= Time.fixedDeltaTime;
        jumpspeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int id, string username)
    {
        ID = id;
        name = username;
        health = maxHealth;

        inputs = new bool[5];
        thirdPersonInput.tpCamera.transform.SetParent(null);
    }


    public void FixedUpdate()
    {
        if (health <= 0)
            return;


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


        thirdPersonInput.inputs = inputs;


        //Vector3 rotation = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        thirdPersonMotor.moveDirection = thirdPersonInput.tpCamera.transform.right * inputDirection.x + thirdPersonInput.tpCamera.transform.forward * inputDirection.y;
        //thirdPersonInput.tpCamera.RotateCamera(rotation.x, rotation.y);

        Send();
    }

    private void Send()
    {
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }


    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        //thirdPersonInput.tpCamera.RotateCamera(_rotation.x, _rotation.y);
        thirdPersonInput.tpCamera.transform.rotation = _rotation;

        //transform.rotation = _rotation;
    }

    public void Shoot(Vector3 viewDirection)
    {
        Debug.Log("I shoot");
        if (Physics.Raycast(shootOrigin.position, viewDirection, out RaycastHit Hit, 50f))
        {
            if(Hit.collider.CompareTag("Player"))
            {
                Hit.collider.GetComponent<Player>().TakeDamage(10f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0)
            return;

        health -= damage;

        if (health <= 0f)
        {
            health = 0;
            thirdPersonInput.enabled = false;
            transform.position = new Vector3(0, 15f, 0);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);

        Debug.Log("I took damage");
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        health = maxHealth;
        thirdPersonInput.enabled = true;
        ServerSend.PlayerRespawn(this);
    }
}
