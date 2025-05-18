using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.03f;

    public AudioClip blipCourt;
    public AudioClip blipMoyen;
    public AudioClip blipLong;
    public AudioClip blipTrèsLong;

    private AudioSource audioSource;

    private Coroutine typingCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void ShowDialogue(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        AudioClip selectedClip = GetClipFromText(message);

        if (audioSource != null && selectedClip != null)
            audioSource.PlayOneShot(selectedClip);

        typingCoroutine = StartCoroutine(TypeSentence(message));
    }

    private AudioClip GetClipFromText(string text)
    {
        int wordCount = text.Split(' ').Length;

        if (wordCount <= 4)
            return blipCourt;
        else if (wordCount <= 8)
            return blipMoyen;
        else if (wordCount <= 13)
            return blipLong;
        else
            return blipTrèsLong;
    }



    private System.Collections.IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
