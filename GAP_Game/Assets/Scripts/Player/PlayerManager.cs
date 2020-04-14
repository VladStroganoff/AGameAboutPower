using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public GameObject model;
    public GameObject Camera;


    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        model.SetActive(false);
    }

    public void Respawn()
    {
        model.SetActive(true);
        SetHealth(maxHealth);
    }
}
