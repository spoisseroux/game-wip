using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI; //Parent UI for dialgue
    public TMP_Text speakerNameUI; //UI TMPro to hold Speaker Name
    public TMP_Text dialogueTextUI; //UI TMPro to hold Dialogue Text
    public Image portraitUI; //UI image to hold sprite
    public GameObject choicePanel; //hook up UI panel that holds Y/N Buttons here

    [Header("Player References")]
    public PlayerInput playerInput;
    private InputActionMap playerActionMap;

    [Header("Camera")]
    public Camera mainCamera;
    public Transform player;
    private Vector3 cameraOriginalPosition;
    private Quaternion cameraOriginalRotation;
    private Vector3 cameraTargetPosition;
    private Quaternion cameraTargetRotation;
    public float cameraLerpSpeed = 3f;
    private bool cameraZoomIn = false;
    private bool cameraZoomOut = false;
    private Transform npcTransform;
    public CinemachineCamera dialogueCamera;
    public CinemachineTargetGroup conversationTargetGroup;

    [Header("Data")]
    public DialogueConversation currentConversation;

    private DialogueNode currentNode;
    public bool conversationStarted = false;
    public bool waitingForRelease = false;
    public bool isEndingConversation = false;
    private float endConversationTimer = 0f;
    private float maxEndConversationTime = 2f;

    void Start()
    {
        //TODO: Change player action map to what u made?
        playerActionMap = playerInput.actions.FindActionMap("Player");
    }

    void Update()
    {
        // Handle conversation ending with timeout
        if (isEndingConversation)
        {
            endConversationTimer += Time.deltaTime;
            handleCameraLerp();

            // Force end conversation if it takes too long
            if (endConversationTimer >= maxEndConversationTime)
            {
                Debug.Log("Conversation end timed out - forcing completion");
                CompleteConversationEnd();
            }
            return;
        }

        if (currentNode == null) return;

        //Wait for release of key before waiting for next press
        if (waitingForRelease)
        {
            if (!Keyboard.current.eKey.isPressed)
            {
                waitingForRelease = false;
            }
            return;
        }

        handleCameraLerp();
        chooseNextNode();
    }

    void chooseNextNode()
    {
        // If this node has multiple choices (branching)
        if (currentNode.nextNodes != null && currentNode.nextNodes.Length > 1)
        {
            //TODO: Better Y/N Input system
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                NextNode(0); // choose first branch
            }
            else if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                NextNode(1); // choose second branch
            }
        }
        // If this node has exactly one choice; linear
        else if (currentNode.nextNodes != null && currentNode.nextNodes.Length == 1)
        {
            //TODO: Advance dialog with input system not hardcoded
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                NextNode(); // default goes to 0
            }
        }
        // Only last node case
        else if ((currentNode.nextNodes == null || currentNode.nextNodes.Length == 0) && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EndConversation();
        }
    }

    void handleCameraLerp()
    {
        if (cameraZoomIn)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTargetPosition, Time.deltaTime * cameraLerpSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraTargetRotation, Time.deltaTime * cameraLerpSpeed);

            //handle stop lerping when close enough
            if (Vector3.Distance(mainCamera.transform.position, cameraTargetPosition) < 0.05f)
                cameraZoomIn = false;
        }
        if (cameraZoomOut)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraOriginalPosition, Time.deltaTime * cameraLerpSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraOriginalRotation, Time.deltaTime * cameraLerpSpeed);

            float distanceToTarget = Vector3.Distance(mainCamera.transform.position, cameraOriginalPosition);
            Debug.Log($"Camera zoom out distance remaining: {distanceToTarget}");

            if (distanceToTarget < 0.05f)
            {
                Debug.Log("Camera zoom out completed");
                cameraZoomOut = false;
                if (isEndingConversation)
                {
                    CompleteConversationEnd();
                }
            }
        }
    }

    public void StartConversation(DialogueConversation conversation, Transform npc)
    {
        conversationStarted = true;
        isEndingConversation = false;

        //TODO: Disable cinemachine camera (this may not be needed when we implement our own camera system)
        //dialogueCamera.enabled = false;

        //disable playermovement 
        playerActionMap.Disable();

        //track original camera transform
        cameraOriginalPosition = mainCamera.transform.position;
        cameraOriginalRotation = mainCamera.transform.rotation;
        npcTransform = npc;

        conversationTargetGroup.Targets[0].Object = player;
        conversationTargetGroup.Targets[0].Weight = 1f;
        conversationTargetGroup.Targets[0].Radius = 0.5f;

        conversationTargetGroup.Targets[1].Object = npc;
        conversationTargetGroup.Targets[1].Weight = 1f;
        conversationTargetGroup.Targets[1].Radius = 0.5f;


        /*
        //get target camera position between player & npc with proper offset calculation
        Vector3 midpoint = (player.position + npcTransform.position) / 2f;

        // Calculate direction from NPC to Player to determine proper camera offset
        Vector3 npcToPlayer = (player.position - npcTransform.position).normalized;
        Vector3 rightDirection = Vector3.Cross(npcToPlayer, Vector3.up).normalized; // Perpendicular to player-npc line

        // Position camera to the side, up, and back from the midpoint
        Vector3 offset = rightDirection * 1f + Vector3.up * 1f + (-npcToPlayer) * 1f;
        cameraTargetPosition = midpoint + offset;

        //look at NPC?
        //cameraTargetRotation = Quaternion.LookRotation(this.transform.position - cameraTargetPosition);
        */

        //start zoom in lerp
        cameraZoomIn = true;
        cameraZoomOut = false;

        //base conversation handling
        currentConversation = conversation;
        Debug.Log("Conversation Started: " + conversationStarted);
        currentNode = conversation.nodes[0];
        dialogueUI.SetActive(true);
        ShowNode(currentNode);

        //wait for key release
        waitingForRelease = true;
    }

    void ShowNode(DialogueNode node)
    {
        currentNode = node;

        speakerNameUI.text = node.speakerId;
        portraitUI.sprite = node.portrait;

        node.text.StringChanged += (localized) => dialogueTextUI.text = localized;
        node.text.RefreshString();

        choicePanel.SetActive(node.nextNodes != null && node.nextNodes.Length > 1);

        // Set waitingForRelease correctly
        waitingForRelease = true; // always wait for key release first
    }

    public void NextNode(int choiceIndex = 0)
    {
        choicePanel.SetActive(false); // hide buttons

        if (currentNode.nextNodes != null && currentNode.nextNodes.Length > 0)
        {
            if (choiceIndex < currentNode.nextNodes.Length)
            {
                currentNode = currentNode.nextNodes[choiceIndex];
                ShowNode(currentNode); // show next node
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
        Debug.Log("Dialogue finished - Starting end sequence");

        isEndingConversation = true;
        endConversationTimer = 0f;

        //Hide UI immediately
        choicePanel.SetActive(false);
        dialogueUI.SetActive(false);

        //Start zoom out lerp
        cameraZoomIn = false;
        cameraZoomOut = true;

        Debug.Log($"Camera zoom out started. Current pos: {mainCamera.transform.position}, Target pos: {cameraOriginalPosition}");
    }

    void CompleteConversationEnd()
    {
        Debug.Log("Conversation completely ended - Camera lerp finished");

        // Force camera to final position to ensure it's exactly right
        //mainCamera.transform.position = cameraOriginalPosition;
        //mainCamera.transform.rotation = cameraOriginalRotation;

        conversationTargetGroup.Targets[1].Object = null;
        conversationTargetGroup.Targets[1].Weight = 0f;
        conversationTargetGroup.Targets[1].Radius = 0f;

        conversationStarted = false;
        isEndingConversation = false;
        currentNode = null;
        waitingForRelease = false;
        cameraZoomOut = false;
        endConversationTimer = 0f;

        //TODO: Enable cinemachine camera (this may not be needed when we implement our own camera system)
        //dialogueCamera.enabled = true;

        //Re-enable playermovement ONLY when lerp is finished
        playerActionMap.Enable();

        Debug.Log("Player movement re-enabled after camera lerp completion");
    }

    void ForceCompleteConversationEnd()
    {
        Debug.Log("Conversation timed out - forcing end without re-enabling movement");

        // Force camera to final position
        mainCamera.transform.position = cameraOriginalPosition;
        mainCamera.transform.rotation = cameraOriginalRotation;

        conversationStarted = false;
        isEndingConversation = false;
        currentNode = null;
        waitingForRelease = false;
        cameraZoomOut = false;
        endConversationTimer = 0f;

        //Enable cinemachine camera
        dialogueCamera.enabled = true;

        // DO NOT re-enable player movement here - player stays locked if camera fails
        Debug.Log("Conversation forced to end - player movement NOT re-enabled due to camera timeout");
    }
}