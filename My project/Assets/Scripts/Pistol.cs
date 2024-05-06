using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class Pistol : Weapon
{
    public override int Id { get; set; } = 1;
    public override string Name { get; set; } = "Pistol";
    public override float Damage { get; set; } = 30f;
    public override float Range { get; set; } = 50f;
    public override int FireRate { get; set; } = 3;

    public override int MaxAmmo { get; set; } = 13;
    public override int Ammo { get; set; } = 12;
    public override float Weight { get; set; } = 1f;
    public override Animator Animator { get; set; }
    
    [SerializeField] private Transform camera;
    
    private bool _isReloading = false;
    private bool _isWeaponDry = false;
    private float _timeSinceLastShot = 0f;
    [NonSerialized] private bool _isReady = false;
    
    public override void EquipWeapon()
    {
        gameObject.SetActive(true);
        Timing.CallDelayed(1f, () => _isReady = true);
    }
    
    private void Start()
    {
        Animator = GetComponent<Animator>();
        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPaused)
            return;
        
        _timeSinceLastShot += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.R) && Ammo < MaxAmmo && _isReady && !_isReloading)
        {
            Timing.RunCoroutine(Reload());
        }
        
        if (Input.GetMouseButtonDown(0) && !_isReloading && !_isWeaponDry && _timeSinceLastShot >= 1f / FireRate && _isReady)
        {
            Shoot();
        }
        
        if (Input.GetKeyDown(KeyCode.F) && _isReady && !_isReloading)
        {
            Animator.Play("Inspect");
        }
    }

    private IEnumerator<float> Reload()
    {
        _isReloading = true;
        if (_isWeaponDry)
        {
            Animator.Play("DryReload");
        }
        else
        {
            Animator.Play("Reload");
        }

        yield return Timing.WaitForSeconds(3f);
        if (Ammo == 0)
        {
            Ammo = MaxAmmo - 1;
        }
        else
        {
            Ammo = MaxAmmo;
        }

        _isWeaponDry = false;
        _isReloading = false;
    }

    private void Shoot()
    {
        if (Ammo <= 0)
        {
            if (!_isWeaponDry)
            {
                Animator.Play("DryFire");
                _isWeaponDry = true;
            }
            return;
        }
        
        Animator.Play("Fire");
        Ammo--;
        
        RaycastHit hit;
        
        if (!Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Range))
        {
            return;
        }
        
        if (hit.transform.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeDamage(Damage);
        }
            
        Debug.Log(hit.transform.name);
    }

    public override void HolsterWeapon()
    {
        _isReady = false;
        Animator.Play("Holster");
    }
}
