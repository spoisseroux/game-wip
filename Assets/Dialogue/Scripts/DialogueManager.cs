using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

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

    [Header("Data")]
    public DialogueConversation currentConversation;
    private DialogueNode currentNode;
    public bool conversationStarted = false;
    public bool waitingForRelease = false;

    void Start()
    {
        playerActionMap = playerInput.actions.FindActionMap("Player");
    }

    void Update()
    {
        // Only process dialogue if we're in a conversation
        if (!conversationStarted) return;

        // Wait for release of key before processing next input
        if (waitingForRelease)
        {
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

    void ChooseNextNode()
    {
        // If this node has multiple choices (branching)
        if (currentNode.nextNodes != null && currentNode.nextNodes.Length > 1)
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                NextNode(0);
            }
            else if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                NextNode(1);
            }
        }
        // If this node has exactly one choice; linear
        else if (currentNode.nextNodes != null && currentNode.nextNodes.Length == 1)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                NextNode();
            }
        }
        // Only last node case
        else if ((currentNode.nextNodes == null || currentNode.nextNodes.Length == 0) &&
                 Keyboard.current.eKey.wasPressedThisFrame)
        {
            EndConversation();
        }
    }

    public void StartConversation(DialogueConversation conversation, Transform npc)
    {
        conversationStarted = true;

        playerActionMap?.Disable();

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

        node.text.StringChanged += (localized) => dialogueTextUI.text = localized;
        node.text.RefreshString();

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

        // Hide UI immediately
        choicePanel.SetActive(false);
        dialogueUI.SetActive(false);

        conversationStarted = false;
        currentNode = null;
        waitingForRelease = false;

        playerActionMap?.Enable();
    }
}