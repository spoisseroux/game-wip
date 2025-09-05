using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Conversation")]
public class DialogueConversation : ScriptableObject
{
    public DialogueNode[] nodes; //all dialogue nodes in the conversation
    //we can create a new dialogue conversation asset from the right click menu
}

