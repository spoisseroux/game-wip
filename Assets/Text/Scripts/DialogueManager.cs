using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// maybe move all cases of enable/disable action maps back to trigger, only store the input
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI;
    public TMP_Text speakerNameUI;
    public TMP_Text dialogueTextUI;
    public Image portraitUI;
    public GameObject choicePanel;

    [Header("Input References")]
    [SerializeField] InputReader input;

    [Header("Player References")]
    [SerializeField] PlayerMovementManager player;

    [Header("Data")]
    public DialogueConversation currentConversation;
    private DialogueNode currentNode;
    public bool conversationStarted = false;
    public bool waitingForRelease = false;

    [Header("Dependencies")]
    private Typewriter typewriter;

    [Header("Camera Zoom Settings")]
    private CinemachineOrbitalFollow orbitalCamera;
    public float zoomInRadius = 8f;
    private float zoomOutRadius;
    public float zoomSpeed = 8f;

    private Coroutine zoomCoroutine;

    #region Monobehaviors
    void Awake()
    {
        // player, should be serialized field
        // player = Object.FindFirstObjectByType<PlayerMovementManager>();

        // typewriter
        typewriter = GetComponent<Typewriter>();

        // need to fix this camera
        orbitalCamera = Object.FindFirstObjectByType<CinemachineOrbitalFollow>();
        zoomOutRadius = orbitalCamera.Radius;
    }

    void Start()
    {
        
    }

    void Update()
    {
        /*

            change to event based polling

        */

        // wait for release of advance key before processing next input
        /*
        if (waitingForRelease)
        {
            //TODO: Hardcoded inputs
            if (!Keyboard.current.eKey.isPressed &&
                !Keyboard.current.yKey.isPressed &&
                !Keyboard.current.nKey.isPressed)
            {
                waitingForRelease = false;
            }
            return;
        }

        ChooseNextNode();
        */
    }
    #endregion

    #region UI Requests    
    private void AttemptSelect()
    {
        if (!conversationStarted || currentNode == null) return;
        
        // what states do we have here?
        // 1. Typing --> if (typewriter.isTyping) --> typewriter.Skip()
        // 2. Ready for Next --> else ChooseNode(currentOptionNumber)
    }

    private void AttemptExit()
    {
        // anything to do here besides EndConversation()?
    }


    // for all of these, just change index based on.... yeah
    // decide whether we do up/down or left/right for dialogue options
    // left/right for menus like runeUI? up/down for dialogue options? idk think on it baby
    private void AttemptUpMove()
    {
        
    }

    private void AttemptDownMove()
    {
        
    }

    private void AttemptLeftMove()
    {
        return;
    }

    private void AttemptRightMove()
    {
        return;
    }

    // need a mouse click event??

    // need a speed up dialogue type key!!!! --> Typewriter.SpeedUp() --> line63 delay *= 0.25f;

    #endregion

    #region Event Link
    private void LinkToInputEvents()
    {
        input.EnableActionMapByName("UI");

        // hook up events
        input.OnSelectInput += AttemptSelect;
        input.OnExitInput += AttemptExit;
        input.OnMoveUpInput += AttemptUpMove;
        input.OnMoveDownInput += AttemptDownMove;
        input.OnMoveLeftInput += AttemptLeftMove;
        input.OnMoveRightInput += AttemptRightMove;

        // mouse input is public Vector2 input.mouseInput;
    }

    private void DisconnectFromInputEvents()
    {
        input.OnSelectInput -= AttemptSelect;
        input.OnExitInput -= AttemptExit;
        input.OnMoveUpInput -= AttemptUpMove;
        input.OnMoveDownInput -= AttemptDownMove;
        input.OnMoveLeftInput -= AttemptLeftMove;
        input.OnMoveRightInput -= AttemptRightMove;

        input.DisableActionMapByName("UI");
    }
    #endregion

    #region Conversation Management
    public void StartConversation(DialogueConversation conversation, Transform npc)
    {
        // state
        conversationStarted = true;
        
        // change to inputReader
        input.EnableActionMapByName("UI");

        // camera routine
        DialogueZoomIn();

        // set conversation and dialogue
        currentConversation = conversation;
        currentNode = conversation.nodes[0];
        dialogueUI.SetActive(true);
        ShowNode(currentNode);
        waitingForRelease = true;

        // link inputs later so no janky skip
        LinkToInputEvents();
    }

    void ShowNode(DialogueNode node)
    {
        currentNode = node;
        speakerNameUI.text = node.speakerId;
        portraitUI.sprite = node.portrait;

        node.text.StringChanged += (localized) =>
        {
            typewriter.StartTyping(dialogueTextUI, localized);
        };

        choicePanel.SetActive(node.nextNodes != null && node.nextNodes.Length > 1);
        waitingForRelease = true;
    }

    public void NextNode(int choiceIndex = 0)
    {
        choicePanel.SetActive(false);

        if (currentNode.nextNodes != null && currentNode.nextNodes.Length > 0)
        {
            if (choiceIndex < currentNode.nextNodes.Length)
            {
                currentNode = currentNode.nextNodes[choiceIndex];
                ShowNode(currentNode);
                waitingForRelease = true;
            }
            else
            {
                EndConversation();
            }
        }
        else
        {
            EndConversation();
        }
    }

    // should unnecessary because of how option selection works now with up/down/left/right move and select
    void ChooseNextNode()
    {
        if (currentNode == null) return;

        // prevent advancing if still typing & handle skipping
        if (typewriter.IsTyping)
        {
            //TODO: Hardcoded skip dialogue key press
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                typewriter.Skip();
            }
            return;
        }

        // if this node has multiple choices
        if (currentNode.nextNodes != null && currentNode.nextNodes.Length > 1)
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                NextNode(0);
                return;
            }
            else if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                NextNode(1);
                return;
            }
        }
        // if this node haslinear choice
        else if (currentNode.nextNodes != null && currentNode.nextNodes.Length == 1)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                NextNode();
                return;
            }
        }
        // last node case
        else if ((currentNode.nextNodes == null || currentNode.nextNodes.Length == 0) &&
                 Keyboard.current.eKey.wasPressedThisFrame)
        {
            EndConversation();
            return;
        }
    }

    void EndConversation()
    {
        conversationStarted = false;

        // Hide UI
        choicePanel.SetActive(false);
        dialogueUI.SetActive(false);

        // Reset states
        currentNode = null;
        currentConversation = null;

        DialogueZoomOut();

        // free player
        player.ResetInteract();

        // change to inputReader
        input.EnableActionMapByName("Player");

        StartCoroutine(WaitForKeyRelease()); // ???
    }
    #endregion

    private IEnumerator WaitForKeyRelease()
    {
        waitingForRelease = true;

        while (Keyboard.current.eKey.isPressed ||
               Keyboard.current.yKey.isPressed ||
               Keyboard.current.nKey.isPressed)
        {
            yield return null; //wait a frame
        }

        waitingForRelease = false;
    }

    #region Zooming Helpers & Functions
    public void DialogueZoomIn()
    {
        StartZoom(zoomInRadius);
    }

    public void DialogueZoomOut()
    {
        StartZoom(zoomOutRadius);
    }

    private void StartZoom(float targetRadius)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomRoutine(targetRadius));
    }

    private IEnumerator ZoomRoutine(float targetRadius)
    {
        if (orbitalCamera == null) yield break;

        float startRadius = orbitalCamera.Radius;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            orbitalCamera.Radius = Mathf.Lerp(startRadius, targetRadius, t);
            yield return null;
        }

        orbitalCamera.Radius = targetRadius;
        zoomCoroutine = null;
    }
    #endregion
}