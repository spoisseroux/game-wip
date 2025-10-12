using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

[CreateAssetMenu(fileName = "NewInputSO", menuName = "Custom/InputSO")]
public class InputReader : ScriptableObject, IPlayerActions, IUIActions
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
    // ui request event
    public event Action<UIRequest, bool> PollUIRequest = delegate { };

    #region Enable Disable
    public void EnablePlayerActions()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Player.SetCallbacks(this);
            controls.UI.SetCallbacks(this);
        }
        controls.Enable();
        controls.UI.Disable();
    }

    public void EnableActionMap(string mapName)
    {
        var maps = controls.asset.actionMaps;
        foreach (var aMap in maps)
        {
            if (aMap.name == mapName)
                aMap.Enable();
            else
                aMap.Disable();
        }
    }
    #endregion

    #region Action Input Handling
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        // no-op right now
    }
    #endregion

    #region UI Input Handling
    // MISSING:
    // 1. All mouse inputs (move, click)
    // 2. What buttons map to what logic
    // 
    // NEED TO THINKIES:
    // 1. What the Move functions actually correspond to given different Input receivers (dialogue vs inventory vs store)
    // 2. How to represent 'moving' along the UI, and how to store info such that it flows based on move logically

    public void OnSelect(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollUIRequest?.Invoke(UIRequest.Select, true);
                break;
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollUIRequest?.Invoke(UIRequest.Exit, true);
                break;
        }
    }

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollUIRequest?.Invoke(UIRequest.MoveUp, true);
                break;
        }
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollUIRequest?.Invoke(UIRequest.MoveDown, true);
                break;
        }
    }
    
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollUIRequest?.Invoke(UIRequest.MoveLeft, true);
                break;
        }
    }
    
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollUIRequest?.Invoke(UIRequest.MoveRight, true);
                break;
        }
    }
    #endregion
}


// for schmoving
public enum ActionRequest
{
    Jump,
    Dash,
    WallJump,
    Attack,
    Interact
}

public enum UIRequest
{
    Exit,
    Select,
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown
}

