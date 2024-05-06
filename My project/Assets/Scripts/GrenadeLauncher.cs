using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : Weapon
{
    public override int Id { get; set; }
    public override string Name { get; set; } = "Grenade Launcher";
    public override float Damage { get; set; } = 100f;
    public override float Range { get; set; } = 100f;
    public override int FireRate { get; set; } = 1;
    public override int MaxAmmo { get; set; } = 1;
    public override int Ammo { get; set; } = 1;
    public override Animator Animator { get; set; }
    public override float Weight { get; set; }

    public GameObject grenadePrefab;
    public Transform firePoint;
    public float throwForce = 40f;

    public override void EquipWeapon()
    {
        throw new System.NotImplementedException();
    }

    public override void HolsterWeapon()
    {
        throw new System.NotImplementedException();
    }
    
    public void Shoot()
    {
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * throwForce, ForceMode.VelocityChange);
        
        Ammo--;
    }
}
