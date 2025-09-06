using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runMultiplier = 2f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.1f;
    public float rotationSpeed = 10f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public LayerMask groundMask = 1; // Default layer

    [Header("References")]
    public Camera mainCamera;

    // Components
    private CharacterController characterController;
    private Transform cameraTransform;

    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isRunning;
    private bool jumpPressed;

    // Movement state
    private Vector3 velocity;
    private bool isGrounded;
    private float verticalRotation;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = mainCamera.transform;
        ResetPlayerState();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        ResetPlayerState(); // safe toggle
    }

    void ResetPlayerState()
    {
        velocity = Vector3.zero;
        verticalRotation = 0f;
    }

    void Update()
    {
        HandleInput();
        HandleMouseLook();
        HandleMovement();
        HandleGravityAndJumping();
    }

    void HandleInput()
    {
        // WASD movement
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;

        // Running
        isRunning = Keyboard.current.leftShiftKey.isPressed;

        // Jump
        jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;

        // Mouse look
        lookInput = Mouse.current.delta.ReadValue();

        // ESC to unlock cursor
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        // Player rotation handled in movement
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(
            transform.position + Vector3.down * (characterController.height * 0.5f),
            groundCheckDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // keep grounded

        // Calculate movement
        Vector3 moveDirection = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        }
        else
        {
            moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }

        // Rotate player to face movement
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply speed
        float currentSpeed = moveSpeed * (isRunning ? runMultiplier : 1f);
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void HandleGravityAndJumping()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 spherePos = transform.position + Vector3.down * (GetComponent<CharacterController>().height * 0.5f);
        Gizmos.DrawWireSphere(spherePos, groundCheckDistance);
    }

    // Public properties
    public bool IsGrounded => isGrounded;
    public bool IsMoving => moveInput.magnitude > 0.1f;
    public bool IsRunning => isRunning && IsMoving;
    public Vector3 MoveDirection => new Vector3(moveInput.x, 0, moveInput.y);
    public float VerticalRotation => verticalRotation;
}
