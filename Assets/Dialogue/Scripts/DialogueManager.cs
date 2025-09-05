using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI; //Parent UI for dialgue
    public TMP_Text speakerNameUI; //UI TMPro to hold Speaker Name
    public TMP_Text dialogueTextUI; //UI TMPro to hold Dialogue Text
    public Image portraitUI; //UI image to hold sprite
    public GameObject choicePanel; //hook up UI panel that holds Y/N Buttons here

    [Header("Data")]
    public DialogueConversation currentConversation;

    private DialogueNode currentNode;
    public bool conversationStarted = false;
    private bool waitingForRelease = false;


    void Update()
    {
        if (currentNode == null) return;

        //Wait for release before waiting for next press
        if (waitingForRelease)
        {
            //TODO: hard coded e press
            if (!Keyboard.current.eKey.isPressed)
                waitingForRelease = false;
            return;
        }

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
        //If this is the last node
        else if (currentNode.nextNodes == null || currentNode.nextNodes.Length == 0)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) EndConversation();
        }
    }

    public void StartConversation(DialogueConversation conversation)
    {
        conversationStarted = true;
        currentConversation = conversation;
        Debug.Log("Conversation Started: " + conversationStarted);
        currentNode = conversation.nodes[0];
        dialogueUI.SetActive(true);
        ShowNode(currentNode);

        waitingForRelease = true;
    }

    void ShowNode(DialogueNode node)
    {
        currentNode = node;

        //Set Speaker Name & Portrait
        speakerNameUI.text = node.speakerId;
        portraitUI.sprite = node.portrait;

        //Updates text in UI
        node.text.StringChanged += (localized) => dialogueTextUI.text = localized;
        node.text.RefreshString();

        //If there is more than 1 next nodes, show choices panel
        if (node.nextNodes != null && node.nextNodes.Length > 1)
        {
            choicePanel.SetActive(true);
            //In the inspector, hook up Y&N buttons onClick events to choose Next Node 0 or 1 via NextNode method
        }
        else
        {
            choicePanel.SetActive(false);
        }

        waitingForRelease = true;
    }

    //Defaults to 0 if there is only 1 next nodes, ends if 0, 
    public void NextNode(int choiceIndex = 0)
    {
        choicePanel.SetActive(false); //hide buttons in case they were shown from previous selection
        if (currentNode.nextNodes != null && currentNode.nextNodes.Length > 0)
        {
            if (choiceIndex < currentNode.nextNodes.Length)
            {
                ShowNode(currentNode.nextNodes[choiceIndex]);
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
        Debug.Log("Dialogue finished");
        conversationStarted = false;
        Debug.Log("Conversation Started: " + conversationStarted);
        choicePanel.SetActive(false);
        dialogueUI.SetActive(false);
        //Hide dialogue ui & return control to player here
    }
}

