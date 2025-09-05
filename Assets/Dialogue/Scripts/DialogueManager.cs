using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour {
    [Header("UI References")]
    public GameObject dialogueUI; //Parent UI for dialgue
    public TMP_Text speakerNameUI; //UI TMPro to hold Speaker Name
    public TMP_Text dialogueTextUI; //UI TMPro to hold Dialogue Text
    public Image portraitUI; //UI image to hold sprite
    public GameObject choicePanel; //hook up UI panel that holds Y/N Buttons here

    [Header("Data")]
    public DialogueConversation currentConversation;

    private DialogueNode currentNode;
    
    //TODO: Update this with actual game input and not just space eventually
    void Update() {
        // Only advance if there is exactly one next node (linear dialogue)
        if (currentNode != null && currentNode.nextNodes != null && currentNode.nextNodes.Length == 1) {
            // Check if space key was pressed using new Input System
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                NextNode(); // Advances to the next node
            }
        }
    }

    public void StartConversation(DialogueConversation conversation)
    {
        currentConversation = conversation;
        currentNode = conversation.nodes[0];
        dialogueUI.SetActive(true);
        ShowNode(currentNode);
    }

    void ShowNode(DialogueNode node) {
        currentNode = node;

        //Set Speaker Name & Portrait
        speakerNameUI.text = node.speakerId;
        portraitUI.sprite = node.portrait;

        //Updates text in UI
        node.text.StringChanged += (localized) => dialogueTextUI.text = localized;
        node.text.RefreshString();

        //If there is more than 1 next nodes, show choices panel
        if (node.nextNodes != null && node.nextNodes.Length > 1) {
            choicePanel.SetActive(true);
            //In the inspector, hook up Y&N buttons onClick events to choose Next Node 0 or 1 via NextNode method
        } else {
            choicePanel.SetActive(false);
        }
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
        choicePanel.SetActive(false);
        dialogueUI.SetActive(false);
        //Hide dialogue ui & return control to player here
    }
}

