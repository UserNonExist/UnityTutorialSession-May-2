using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class NpcCharacter : MonoBehaviour
{
    
    [FormerlySerializedAs("_target")] [SerializeField] private GameObject target;
    
    private HealthSystem _healthSystem;
    private NavMeshAgent _navMeshAgent;
    private TMP_Text _statusText;
    private Canvas _canvas;
    
    private bool _canAttack = true;
    
    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _healthSystem = GetComponent<HealthSystem>();
        _statusText = GetComponentInChildren<TMP_Text>();
        _canvas = GetComponentInChildren<Canvas>();
        
        _canvas.worldCamera = Camera.main;
        target = GameObject.FindGameObjectWithTag("Player");
        Timing.RunCoroutine(UpdateTarget(), $"updateTarget-{name}");
    }
    
    private void Update()
    {
        _statusText.text = $"HP: {Mathf.Round(_healthSystem.Health)}";
        _canvas.transform.LookAt(_canvas.worldCamera.transform);
        
        if (_healthSystem.Health <= 0)
        {
            _navMeshAgent.enabled = false;
            _canAttack = false;
            Timing.KillCoroutines($"updateTarget-{name}");
            Destroy(_canvas.gameObject);
            Destroy(this);
            return;
        }
        
        var overlapCapsule = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 2, 1.1f);

        foreach (var collider in overlapCapsule)
        {
            if (collider.transform == this.transform)
                continue;
            
            if (!collider.CompareTag("Player"))
                continue;
            
            if (collider.TryGetComponent(out HealthSystem healthSystem))
            {
                healthSystem.TakeDamage(7f * Time.deltaTime);
            }
        }
    }
    
    private IEnumerator<float> UpdateTarget()
    {
        while (target != null)
        {
            _navMeshAgent.SetDestination(target.transform.position);
            yield return Timing.WaitForSeconds(1.5f);
        }
    }
}
