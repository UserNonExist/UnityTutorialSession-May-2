using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject itemPrefab;
    public bool canPickup = false;
    public Canvas pickupCanvas;
    public TMP_Text pickupText;
    
    private Transform _player;

    public void Start()
    {
        pickupCanvas.gameObject.SetActive(false);
        _player = GameObject.FindWithTag("Player").transform;
    }
    
    private void Update()
    {
        var distance = Vector3.Distance(transform.position, _player.position);
        
        
        if (distance < 5f)
        {
            canPickup = true;
            pickupCanvas.gameObject.SetActive(true);
            pickupCanvas.transform.LookAt(Camera.main.transform);
        }
        else
        {
            canPickup = false;
            pickupCanvas.gameObject.SetActive(false);
        }
        
        if (canPickup && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
        }
    }
    
    private void PickupItem()
    {
        var inventorySystem = _player.GetComponent<InventorySystem>();
        
        if (inventorySystem.weapons.Count >= 3)
        {
            pickupText.text = "Inventory is full!";
            Timing.CallDelayed(5f, () => pickupText.text = "Press E to pickup");
            return;
        }

        inventorySystem.AddWeapon(itemPrefab);
        Destroy(gameObject);
    }
}
