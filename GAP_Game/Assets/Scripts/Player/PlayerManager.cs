using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public GameObject model;
    public GameObject Camera;
    public NetworkedAnimator Animator;
    ILoadController _loadControl;
    IInventoryView _inventoryView;

    [Inject]
    public void Inject(ILoadController loadControl, IInventoryView inventoryView)
    {
        _loadControl = loadControl;
        _inventoryView = inventoryView;
    }

    public void Initialize(int _id, string playerData)
    {
        id = _id;
        //username = playerData;
        Dictionary<string, Item> items = _loadControl.LoadInventory(playerData);
        _inventoryView.LoadInventiry(items);
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

    public void Animate()
    {

    }

    public void Respawn()
    {
        model.SetActive(true);
        SetHealth(maxHealth);
    }
}
