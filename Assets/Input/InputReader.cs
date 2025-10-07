using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

[CreateAssetMenu(fileName = "NewInputSO", menuName = "Custom/InputSO")]
public class InputReader : ScriptableObject, IPlayerActions
{
    // reference controls of the Player
    PlayerControls controls;

    // TODO: Implement mode aware inputs! --> free moving & combat, menu/dialogue, card battle
    // maybe coupling too tightly, but it'd be cool to have a system that reads current game state, 
    // then pipes input to the player's corresponding system
    // have a thinky later
    // the above probably will have to do with "button" actions, rather than actions mapping to buttons...

    // movement vars
    [SerializeField] Vector2 movementInput;
    public float horizontalInput; // x
    public float verticalInput; // z
    public float moveAmount; // basically, is there input?

    // camera vars
    [SerializeField] Vector2 cameraInput;
    public float cameraHorizontalInput;
    public float cameraVerticalInput;

    // action request event
    public event Action<ActionRequest, bool> PollInputRequest = delegate { };


    #region Monobehavior
    public void EnablePlayerActions()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Player.SetCallbacks(this);
        }
        controls.Enable();
    }
    #endregion

    #region Input Handling
    public void OnMovement(InputAction.CallbackContext context)
    {
        // read data
        movementInput = context.ReadValue<Vector2>();
        // set relevant info
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }

    public void OnCameraControls(InputAction.CallbackContext context)
    {
        // read data
        cameraInput = context.ReadValue<Vector2>();
        // set relevant info
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // HAVE TO TOOL AROUND WITH THIS, HOLDING THE BUTTON DOWN JUST SPAMS IT ON COOLDOWN!!!
        // want snappy timed inputs!!
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollInputRequest?.Invoke(ActionRequest.Dash, true);
                break;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollInputRequest?.Invoke(ActionRequest.Jump, true);
                break;
        }
    }

    public void OnWallJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollInputRequest?.Invoke(ActionRequest.WallJump, true);
                break;
        }
    }

    public void OnBasicAttack(InputAction.CallbackContext context)
    {
        //pass.performed
    }

    #endregion
}


// for schmoving
public enum ActionRequest
{
    None,
    Jump,
    Dash,
    WallJump,
    Attack,
    Interact
}

