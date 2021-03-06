﻿using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void PlayerDisconnect(PlayerManager player);
public class PlayerManager : MonoBehaviour
{
    public int ID;
    public string Name;
    public vThirdPersonInput thirdPersonInput;
    public vThirdPersonMotor thirdPersonMotor;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float movespeed = 5f;
    public float jumpspeed = 5f;
    public float health;
    public float maxHealth;
    public NetAnimator animator;
    public string playerData;
    public DressController DressControl;
    public PlayerDisconnect PlayerDisconnect;

    private bool[] inputs;


    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        movespeed *= Time.fixedDeltaTime;
        jumpspeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int id, string playerData)
    {
        ID = id;
        Name = playerData;
        health = maxHealth;
        NetworkManager.instance.InventoryControl.AddPlayer(this);

        inputs = new bool[5];
        thirdPersonInput.tpCamera.transform.SetParent(null);
    }

    public void Disconnect()
    {
        PlayerDisconnect.Invoke(this);
        Destroy(gameObject);
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


        thirdPersonMotor.SetMoveDirection(thirdPersonInput.tpCamera.transform.right * inputDirection.x + thirdPersonInput.tpCamera.transform.forward * inputDirection.y);

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
        thirdPersonInput.tpCamera.transform.rotation = _rotation;
    }

    public void Shoot(Vector3 viewDirection)
    {
        Debug.Log("I shoot");
        if (Physics.Raycast(shootOrigin.position, viewDirection, out RaycastHit Hit, 50f))
        {
            if(Hit.collider.CompareTag("Player"))
            {
                Hit.collider.GetComponent<PlayerManager>().TakeDamage(10f);
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
