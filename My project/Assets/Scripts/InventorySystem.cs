using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using TMPro;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();
    public GameObject currentWeapon;
    public Weapon currentWeaponScript;
    public bool requestExit = false;
    public bool isExitDone = false;
    
    public TMP_Text weaponText;
    
    public Transform weaponHolder;
    
    public int grenadeCount = 300;
    public int stanbyRequest = -1;
    
    public GameObject grenadePrefab;
    
    private void Awake()
    {
        foreach (Transform transform in weaponHolder)
        {
            if (transform.CompareTag("Weapon"))
            {
                Weapon weapon = transform.GetComponent<Weapon>();
                weapons.Add(transform.gameObject);
            }
        }
    }

    private void Update()
    {
        if (currentWeapon != null)
        {
            weaponText.text = $"{currentWeaponScript.Name}\n" +
                              $"Ammo: {currentWeaponScript.Ammo}/{currentWeaponScript.MaxAmmo}";
        }
        else
        {
            weaponText.text = "None\nAmmo: 0/0";
        }
        
        if (GameManager.Instance.isPaused)
            return;
        
        int weaponRequest = -1;
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (grenadeCount > 0)
            {
                grenadeCount--;
                GameObject grenade = Instantiate(grenadePrefab, Camera.main.transform.position + Camera.main.transform.forward * 2f, Camera.main.transform.rotation);
                grenade.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponRequest = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponRequest = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponRequest = 2;
        }
        
        if (requestExit && !isExitDone)
            return;
        
        if (requestExit && isExitDone)
        {
            currentWeapon.SetActive(false);
            currentWeapon = null;
            currentWeaponScript = null;
            requestExit = false;
            isExitDone = false;
            return;
        }

        if (stanbyRequest != -1)
        {
            weaponRequest = stanbyRequest;
        }
        
        if (weaponRequest == -1)
            return;
        
        if (weaponRequest >= weapons.Count)
            return;

        if (weaponRequest == weapons.IndexOf(currentWeapon))
        {
            requestExit = true;
            currentWeaponScript.HolsterWeapon();
            return;
        }
        
        if (currentWeapon != null)
        {
            requestExit = true;
            stanbyRequest = weaponRequest;
            currentWeaponScript.HolsterWeapon();
        }
        
        if (currentWeapon == null)
        {
            stanbyRequest = -1;
            currentWeapon = weapons[weaponRequest];
            currentWeaponScript = currentWeapon.GetComponent<Weapon>();
            
            if (currentWeaponScript.Name == "Rifle")
                weaponHolder.transform.localRotation = Quaternion.Euler(0, 90, 0);
            else
                weaponHolder.transform.localRotation = Quaternion.Euler(0, 0, 0);
            
            currentWeaponScript.EquipWeapon();
        }
    }
    
    public void AddWeapon(GameObject weapon)
    {
        if (weapon == null)
            return;
        
        GameObject newWeapon = Instantiate(weapon, weaponHolder);
        
        weapons.Add(newWeapon);
        stanbyRequest = weapons.IndexOf(newWeapon);
    }
}
