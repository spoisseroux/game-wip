using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

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

    void ChooseNextNode()
    {
        if (currentNode == null) return;

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

        playerActionMap?.Enable();

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
}