using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;
    private ChangeDetector _changeDetector;

    public Camera playerCamera;
    public MeshRenderer meshRenderer;
    public float playerSpeed = 2f;

    [Networked]
    public Color networkedColor { get; set; }

    public float jumpForce = 5f;
    public float gravityValue = -9.81f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump")) _jumpPressed = true;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            networkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(networkedColor):
                    meshRenderer.material.color = networkedColor;
                    break;
            }
        }
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        
        if (HasStateAuthority)
        {
            playerCamera = Camera.main;
            playerCamera.GetComponent<FirstPersonCamera>().target = transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false) return;

        if (_controller.isGrounded) _velocity = new Vector3(0, -1, 0);

        var cameraRotationY = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
        var move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * playerSpeed;

        _velocity.y += gravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded) _velocity.y += jumpForce;
        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero) gameObject.transform.forward = move;

        _jumpPressed = false;
    }
}