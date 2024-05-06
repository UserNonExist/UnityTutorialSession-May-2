using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    
    public float healthRegenCooldown = 8f;
    public float healthRegenRate = 10f;
    
    [FormerlySerializedAs("_health")] [SerializeField] private float health = 100f;
    [FormerlySerializedAs("_maxHealth")] [SerializeField] private float maxHealth = 100f;
    
    private Rigidbody _rigidbody;
    private bool _healthRegen = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Health = health;
        MaxHealth = maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_healthRegen)
        {
            Heal(healthRegenRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (!_healthRegen)
        {
            Timing.KillCoroutines("healthRegen-" + name);
        }
        
        Timing.RunCoroutine(HealthRegenerationCooldown(healthRegenCooldown), $"healthRegen-{name}");
        
        if (Health <= 0)
        {
            _healthRegen = false;
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);

            if (gameObject.CompareTag("Enemy"))
            {
                GameManager.Instance.Enemies.Remove(this.gameObject);
            }
            
            if (gameObject.CompareTag("Player"))
            {
                GameManager.Instance.isGameOver = true;
            }
        }
    }
    
    public void Heal(float amount)
    {
        Health += amount;
        
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    
    public void SetHealth(float health)
    {
        Health = health;
        
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    
    public IEnumerator<float> HealthRegenerationCooldown(float cooldown)
    {
        _healthRegen = false;
        yield return Timing.WaitForSeconds(cooldown);
        _healthRegen = true;
    }
}
