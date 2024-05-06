using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;
using Task = System.Threading.Tasks.Task;

public class Knife : Weapon
{
    public override int Id { get; set; } = 2;
    public override string Name { get; set; } = "Knife";
    public override float Damage { get; set; } = 60f;
    public override float Range { get; set; } = 2f;
    public override int FireRate { get; set; } = 1;
    public override Animator Animator { get; set; }
    public override int Ammo { get; set; } = 1;
    public override int MaxAmmo { get; set; } = 1;
    public override float Weight { get; set; } = 1f;

    [SerializeField] private Transform camera;
    
    private bool _isAttacking = false;
    private bool _canAttack = true;
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
    
    private void Update()
    {
        if (GameManager.Instance.isPaused)
            return;
        
        if (Input.GetMouseButtonDown(0) && _canAttack && _isReady)
        {
            _isAttacking = true;
        }
        
        if (Input.GetKeyDown(KeyCode.F) && _isReady && !_isAttacking)
        {
            Animator.Play("Inspect");
        }
    }
    
    private void FixedUpdate()
    {
        if (_isAttacking)
        {
            Attack();
        }
    }
    
    private void Attack()
    {
        if (!gameObject.activeSelf || !_canAttack)
            return;
        
        _canAttack = false;
        
        RaycastHit hit;
        
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Range))
        {
            if (hit.transform.TryGetComponent(out HealthSystem healthSystem))
            {
                healthSystem.TakeDamage(Damage);
            }
            
            if (hit.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hit.normal * 10f, ForceMode.Impulse);
            }
            
            Debug.Log(hit.transform.name);
        }
        
        Animator.Play("Attack");
        
        Timing.RunCoroutine(AttackCooldown(FireRate));
        _isAttacking = false;
    }
    
    private IEnumerator<float> AttackCooldown(float cooldown)
    {
        yield return Timing.WaitForSeconds(cooldown);
        
        _isAttacking = false;
        _canAttack = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(camera.transform.position, camera.transform.forward * Range);
    }
    
    public override void HolsterWeapon()
    {
        _isReady = false;
        Animator.Play("Holster");
    }
}
