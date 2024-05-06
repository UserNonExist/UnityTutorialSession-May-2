using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

public class ImpactGrenade : MonoBehaviour
{
    public GameObject explosionEffect;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float upwardsModifier = 1f;
    public float damage = 100f;
    
    public Collider[] collidersToIgnore;
    
    public MeshRenderer meshRenderer;
    
    private bool _isExploded = false;

    public void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < -100)
        {
            Destroy(gameObject);
            Destroy(this);
        }
        
    }
    
    void OnCollisionEnter(Collision other)
    {
        if (collidersToIgnore.Contains(other.collider))
            return;
        
        if (_isExploded)
            return;
        
        _isExploded = true;
        
        Timing.CallDelayed(2f, () =>
        {
            var currentPos = transform.position;
            var explosion = Physics.OverlapSphere(currentPos, explosionRadius);
        
            var explodeEff = Instantiate(explosionEffect, currentPos, Quaternion.identity);

            var mainModule = explodeEff.GetComponent<ParticleSystem>().main;
            mainModule.startSize = explosionRadius * 2;
            mainModule.loop = false;
            mainModule.stopAction = ParticleSystemStopAction.Destroy;
            foreach (var child in explodeEff.GetComponentsInChildren<ParticleSystem>())
            {
                var childMainModule = child.main;
                childMainModule.startSize = explosionRadius * 2;
                childMainModule.loop = false;
            }
        
            foreach (var hit in explosion)
            {
                if (hit.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(explosionForce, currentPos, explosionRadius, upwardsModifier, ForceMode.Impulse);
                }
            
                if (hit.TryGetComponent(out HealthSystem healthSystem))
                {
                    float distance = Vector3.Distance(currentPos, healthSystem.transform.position);
                    float damageMultiplier = 1 - Math.Clamp(distance / explosionRadius, 0, 1);
                    healthSystem.TakeDamage(damage * damageMultiplier);
                }
            }
            
            Destroy(gameObject);
        });
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
