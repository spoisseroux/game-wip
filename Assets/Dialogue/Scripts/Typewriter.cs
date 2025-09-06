using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Typewriter : MonoBehaviour
{
    [Header("Typewriter Settings")]
    public float baseDelay = 0.05f; //delay for each normal char
    public float punctuationDelay = 0.3f; //delay for punctuation
    public Key fastForwardKey = Key.Space; //key to ff

    private TMP_Text targetText;
    private Coroutine typingCoroutine;
    private string fullText; //store full node text for we can skip easier

    public bool IsTyping { get; private set; } = false; //public get, private set for safety
    public bool Completed { get; private set; } = false;

    public void StartTyping(TMP_Text textComponent, string content)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        targetText = textComponent;
        fullText = content;

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string content)
    {
        IsTyping = true;
        Completed = false;
        targetText.text = "";

        for (int i = 0; i < content.Length; i++)
        {
            targetText.text += content[i];

            float delay = baseDelay;

            //handle punctuation delay
            if (content[i] == '.' || content[i] == '!' || content[i] == '?')
            {
                if (i + 2 < content.Length && content[i] == '.' && content[i + 1] == '.' && content[i + 2] == '.')
                {
                    delay = punctuationDelay * 2f;
                    i += 2;
                    targetText.text += "..";
                }
                else
                {
                    delay = punctuationDelay;
                }
            }

            //handle ff (reduce delay)
            if (Keyboard.current[fastForwardKey].isPressed)
            {
                delay *= 0.25f;
            }

            yield return new WaitForSeconds(delay);
        }

        FinishTyping();
    }

    public void Skip()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        //show all text if skipped
        targetText.text = fullText;

        FinishTyping();
    }

    private void FinishTyping()
    {
        IsTyping = false;
        Completed = true;
        typingCoroutine = null;
    }
}
