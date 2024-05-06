using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour {

    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;

    ParticleSystem _ps;
    float _timer = 0;
    Renderer _renderer;

    int _shaderProperty;

	void Start ()
    {
        _shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        _ps = GetComponentInChildren <ParticleSystem>();

        var main = _ps.main;
        main.duration = spawnEffectTime;

        _ps.Play();

    }
	
	void Update ()
    {
        if (_timer < spawnEffectTime + pause)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _ps.Play();
            _timer = 0;
        }


        _renderer.material.SetFloat(_shaderProperty, fadeIn.Evaluate( Mathf.InverseLerp(0, spawnEffectTime, _timer)));
        
    }
}
