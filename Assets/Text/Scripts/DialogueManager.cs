using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

// maybe move all cases of enable/disable action maps back to trigger, only store the input
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI;
    public TMP_Text speakerNameUI;
    public TMP_Text dialogueTextUI;
    public Image portraitUI;
    public GameObject choicePanel;

    [Header("Player References")]
    public PlayerInput playerInput;
    private InputActionMap playerActionMap;
    public PlayerMovementManager player;

    // edits
    [SerializeField] InputReader input;

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

    protected class UIRequests
    {
        Dictionary<UIRequest, bool> req;

        public UIRequests()
        {
            req = new Dictionary<UIRequest, bool>
            {
                {UIRequest.Exit, false},
                {UIRequest.Select, false},
                // this might just be solved by reading horizontal and vertical movement values
                {UIRequest.MoveDown, false},
                {UIRequest.MoveUp, false},
                {UIRequest.MoveLeft, false},
                {UIRequest.MoveRight, false}
            };
        }

        public void SetRequest(UIRequest u, bool val)
        {
            req[u] = val;
        }

        public bool Check(UIRequest u)
        {
            bool isRequesting = req[u];
            SetRequest(u, false); // reset our input
            return isRequesting;
        }
    }
    UIRequests inputRequests;

    #region Monobehaviors
    void Awake()
    {
        // action requests holder
        input.EnablePlayerActions();
        inputRequests = new UIRequests();

        // player, Serialize later
        player = Object.FindFirstObjectByType<PlayerMovementManager>();

        // typewriter
        typewriter = GetComponent<Typewriter>();

        // camera
        orbitalCamera = Object.FindFirstObjectByType<CinemachineOrbitalFollow>();
        zoomOutRadius = orbitalCamera.Radius;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!conversationStarted || currentNode == null) return;

        // wait for release of advance key before processing next input
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
    }

    void OnEnable()
    {
        input.PollUIRequest += OnInputRequest;
    }

    void OnDisable()
    {
        input.PollUIRequest += OnInputRequest;
    }
    #endregion

    #region Misc. Helpers
    private void OnInputRequest(UIRequest action, bool performed)
    {
        inputRequests.SetRequest(action, performed);

        // could just do switch on action
    } 

    #endregion

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

    public void StartConversation(DialogueConversation conversation, Transform npc)
    {
        conversationStarted = true;
        // change to inputReader
        playerActionMap?.Disable();

        DialogueZoomIn();

        currentConversation = conversation;
        currentNode = conversation.nodes[0];
        dialogueUI.SetActive(true);
        ShowNode(currentNode);
        waitingForRelease = true;
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

        // change to inputReader
        playerActionMap?.Enable();
        // free player
        player.ResetInteract();

        StartCoroutine(WaitForKeyRelease());
    }

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