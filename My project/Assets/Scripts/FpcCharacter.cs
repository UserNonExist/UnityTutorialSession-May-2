using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FpcCharacter : MonoBehaviour
{
    public float Speed { get; set; }
    public float JumpForce { get; set; }
    
    public float mouseSensitivity = 1f;
    public bool isGrounded = false;
    public TMP_Text statusText;
    
    private Rigidbody _rigidbody;
    private HealthSystem _healthSystem;
    private HeadBobController _headBobController;
    private InventorySystem _inventorySystem;
    
    private bool _jumpPressed = false;
    private bool _isSprinting = false;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float staminaRegenRate = 0.2f;
    [SerializeField] private float staminaDepletionRate = 0.4f;
    [SerializeField] private GameObject cameraHolder;
    
    private float _yRotation = 0f;
    private float _xRotation = 0f;
    private float stamina = 100f;
    private float staminaCap = 100f;
    private bool staminaRegenCooldown = false;
    private bool staminaRegenerating = true;
    
    private CoroutineHandle _staminaRegenerationCooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _healthSystem = GetComponent<HealthSystem>();
        _headBobController = GetComponent<HeadBobController>();
        _inventorySystem = GetComponent<InventorySystem>();
        
        
        Speed = speed;
        JumpForce = jumpForce;
        _xRotation = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPaused)
            return;
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _jumpPressed = true;
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isSprinting = true;
            staminaRegenerating = false;
        }
        else
        {
            _isSprinting = false;
            staminaRegenerating = true;
        }
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        _yRotation -= mouseY;
        _yRotation = Mathf.Clamp(_yRotation, -90f, 90f);
        
        _headBobController.ResetPosition();
        var velocity = _rigidbody.velocity;
        velocity.y *= 0;
        
        if (velocity.magnitude > 0.5f)
        {
            _headBobController.PlayMotion(_headBobController.FootStepMotion());
        }
        
        cameraHolder.transform.localRotation = Quaternion.Euler(_yRotation, 0f, 0f);
        
        _xRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0f, _xRotation, 0f);
        
        statusText.text = $"Health: {_healthSystem.Health}\nStamina: {stamina}";
    }

    private void FixedUpdate()
    {
        if (_healthSystem.Health <= 0)
        {
            _rigidbody.velocity = Vector3.zero;
            Destroy(this);
            return;
        }
        
        Vector2 momentum = new Vector2(0, 0);
        
        if (Input.GetKey(KeyCode.W))
        {
            momentum.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            momentum.y -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            momentum.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            momentum.x += 1;
        }
        
        momentum.Normalize();
        
        Vector3 movement = new Vector3(momentum.x, 0, momentum.y);
        
        movement = transform.TransformDirection(movement);
        
        if (_inventorySystem.currentWeapon != null)
        {
            movement /= _inventorySystem.currentWeaponScript.Weight;
            movement *= Speed;
        }
        else
        {
            movement *= Speed;
        }
        
        if (_isSprinting && stamina > 0)
        {
            movement *= 2;
            stamina -= staminaDepletionRate;
        }
        else if (stamina < staminaCap && !_isSprinting)
        {
            stamina += staminaRegenRate;
        }
        
        if (stamina >= staminaCap)
        {
            stamina = staminaCap;
        }
        
        //movement *= Speed;
        
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
        //Debug.Log(_rigidbody.velocity);
        
        if (_jumpPressed)
        {
            _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            _jumpPressed = false;
        }
        
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
