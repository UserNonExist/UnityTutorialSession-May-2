using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class Rifle : Weapon
{
    [field: NonSerialized] public override int Id { get; set; } = 0;
    [field: NonSerialized] public override string Name { get; set; } = "Rifle";
    [field: NonSerialized] public override float Damage { get; set; } = 12f;
    [field: NonSerialized] public override float Range { get; set; } = 100f;
    [field: NonSerialized] public override int FireRate { get; set; } = 7;
    [field: NonSerialized] public override int MaxAmmo { get; set; } = 31;
    [field: NonSerialized] public override int Ammo { get; set; } = 30;
    [field: NonSerialized] public override float Weight { get; set; } = 1.75f;
    public override Animator Animator { get; set; }

    [SerializeField] private Transform camera;
    
    private bool _isReloading = false;
    private float _timeSinceLastShot = 0f;
    [NonSerialized] private bool _isReady = false;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        camera = Camera.main.transform;
    }

    public override void EquipWeapon()
    {
        gameObject.SetActive(true);
        _isReady = true;
    }

    public override void HolsterWeapon()
    {
        _isReady = false;
        Animator.Play("Holster");
    }

    public void Update()
    {
        if (GameManager.Instance.isPaused)
            return;
        
        _timeSinceLastShot += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.R) && Ammo < MaxAmmo && _isReady && !_isReloading)
        {
            Timing.RunCoroutine(Reload());
        }
        
        if (Input.GetMouseButton(0) && !_isReloading && _timeSinceLastShot >= 1f / FireRate && _isReady)
        {
            Shoot();
        }
        
        if (Input.GetKeyDown(KeyCode.F) && _isReady && !_isReloading)
        {
            Animator.Play("Inspect");
        }
    }
    
    private void Shoot()
    {
        if (Ammo <= 0)
        {
            Animator.Play("DryFire");
            return;
        }
        
        Ammo--;
        _timeSinceLastShot = 0f;
        Animator.Play("Fire");
        
        RaycastHit hit;
        
        if (!Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Range))
        {
            return;
        }
        
        if (hit.transform.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeDamage(Damage);
        }
        
        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * 100f);
        }
            
        Debug.Log(hit.transform.name);
    }
    
    private IEnumerator<float> Reload()
    {
        _isReloading = true;
        Animator.Play("Reload");
        yield return Timing.WaitForSeconds(5f);
        
        if (Ammo == 0)
        {
            Ammo = MaxAmmo - 1;
        }
        else
        {
            Ammo = MaxAmmo;
        }
        
        _isReloading = false;
    }
}
