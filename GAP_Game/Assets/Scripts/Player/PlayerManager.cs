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
    IInventoryController _inventoryControl;
    string jsonInventory { get; set; }

    [Inject]
    public void Inject(ILoadController loadControl, IInventoryController inventoryView)
    {
        Debug.Log($"player {gameObject.name} injected");
        _loadControl = loadControl;
        _inventoryControl = inventoryView;
    }

    public void Initialize(int _id, string playerData)
    {
        id = _id;
        //username = playerData;
        jsonInventory = playerData;
        Dictionary<string, Item> items = _loadControl.LoadInventory(playerData);
        _inventoryControl.SpawnPlayer(items, this);
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
