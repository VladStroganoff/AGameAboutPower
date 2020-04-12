using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;
    public string name;
    public CharacterController characterController;
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

        Move(inputDirection);

    }

    private void Move(Vector2 inputDirection)
    {
        Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        moveDirection *= movespeed;


        if(characterController.isGrounded)
        {
            yVelocity = 0;
            if(inputs[4])
            {
                yVelocity = jumpspeed;
            }
        }
        yVelocity += gravity;

        moveDirection.y = yVelocity;

        characterController.Move(moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }


    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
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
            characterController.enabled = false;
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
        characterController.enabled = true;
        ServerSend.PlayerRespawn(this);
    }
}
