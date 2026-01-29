using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;


// individual interfaces for type of receiver, NEED TO FIGURE OUT STRUCTURE FOR THIS
public abstract class InputReceiver {}
public interface IWantsInput {}


[CreateAssetMenu(fileName = "NewInputSO", menuName = "Custom/InputSO")]
public class InputReader : ScriptableObject, IPlayerActions, IUIActions
{
    // reference controls of the Player
    static PlayerControls controls;

    // TODO: Implement mode aware inputs! --> free moving & combat, menu/dialogue, card battle
    // maybe coupling too tightly, but it'd be cool to have a system that reads current game state, 
    // then pipes input to the player's corresponding system
    // have a thinky later
    // the above probably will have to do with "button" actions, rather than actions mapping to buttons...

    [Header("Player")]
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


    [Header("UI")]
    public Vector2 mouseInput;
    // ui request event
    public event Action OnSelectInput;
    public event Action OnExitInput;
    public event Action OnMoveUpInput;
    public event Action OnMoveDownInput;
    public event Action OnMoveLeftInput;
    public event Action OnMoveRightInput;

    #region Enable Disable
    // a fine default to have is EnablePlayerActions() i guess, but think about how to do this ig
    /*
        Either 1 or 2
        1: Separate call to disable by name, then another call to enable by name
        2: Enable by name, cycles through all, disabling every other map, enabling name passed in
    */
    public void EnablePlayerActions()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Player.SetCallbacks(this);
            controls.UI.SetCallbacks(this);
        }
        controls.Player.Enable();
    }

    public void EnableActionMapByName(string mapName)
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

    public void DisableActionMapByName(string name)
    {
        var maps = controls.asset.actionMaps;
        foreach (var map in maps)
        {
            if (map.name == name) {
                map.Disable();
                return;
            }
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
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollInputRequest?.Invoke(ActionRequest.Attack, true);
                break;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PollInputRequest?.Invoke(ActionRequest.Interact, true);
                break;
        }
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
        OnSelectInput?.Invoke();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        OnExitInput?.Invoke();
    }

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        OnMoveUpInput?.Invoke();
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        OnMoveDownInput?.Invoke();
    }
    
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        OnMoveLeftInput?.Invoke();
    }
    
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        OnMoveRightInput?.Invoke();
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }
    #endregion

    #region Static Enable/Disable For Entire Object
    public static void ActivateControls()
    {
        controls.Enable();
    }

    public static void DeactivateControls()
    {
        controls.Disable();
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

