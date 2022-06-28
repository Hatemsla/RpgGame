using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float shiftSpeed => speed * 2;
    public float roationSpeed;
    public float maxStamina = 100;
    public float maxHealth = 100;
    public float maxMana = 100;

    [HideInInspector] public float currentStamina;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentMana;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Camera _camera;


    private PhotonView _view;
    private float _vertical, _horizontal;
    private float _gravity = -9.8f;
    private float _groundDistance = 0.4f;
    private float _staminaAttrition = 50;
    private float _staminaRecovery = 5;
    private float _jumpCost = 20f;
    private float _reachDistance;
    private Vector3 _velocity;
    private bool _isGrounded;

    void Start()
    {
        _view = GetComponent<PhotonView>();
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        currentMana = maxMana;

        if (!_view.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }

    void Update()
    {
        if (_view.IsMine)
        {
            Movement();
        }
    }

    private void Movement()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2;

        _vertical = Input.GetAxis(Axis.Vertical);
        _horizontal = Input.GetAxis(Axis.Horizontal);

        if (Input.GetButtonDown(Axis.Jump) && _isGrounded && currentStamina > _jumpCost)
        {
            currentStamina -= _jumpCost;
            _velocity.y = Mathf.Sqrt(jumpForce * -2 * _gravity);
        }

        Vector3 move = transform.right * _horizontal + transform.forward * _vertical;
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        if (currentStamina <= maxStamina)
            currentStamina += Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            currentStamina -= Time.deltaTime * _staminaAttrition;
            _controller.Move(move * shiftSpeed * Time.deltaTime);
        }
        else
        {
            _controller.Move(move * speed * Time.deltaTime);
            if (currentStamina <= maxStamina)
                currentStamina += Time.deltaTime * _staminaRecovery;
        }

        if (Input.GetKey(Axis.C))
            _controller.height = 1f;
        else
            _controller.height = 2f;
    }
}
