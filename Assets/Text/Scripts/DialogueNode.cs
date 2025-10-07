using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class DialogueNode
{
    [Header("Dialogue Info")]
    public string speakerId; //The name that will show up above dialogue
    public Sprite portrait; //The character sprite
    public LocalizedString text; //Reference to the localized text field

    [Header("Branches")]
    public DialogueNode[] nextNodes;  //index 0 = yes, index 1 = no
    //dialogue manager will handle choices and button instancing
}

