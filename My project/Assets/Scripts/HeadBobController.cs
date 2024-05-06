using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool _enableHeadBob = true;
    [SerializeField, Range(0,0.1f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0,30)] private float _frequency = 10.0f;
    
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = _camera.localPosition;
    }

    private void Update()
    {
        if (!_enableHeadBob) return;
        
        _cameraHolder.LookAt(FocusTarget());
    }
    
    public Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude;
        return pos;
    }

    public void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y,
            transform.position.z);
        pos += _cameraHolder.forward * 15f;
        return pos;
    }

    public void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }
}
