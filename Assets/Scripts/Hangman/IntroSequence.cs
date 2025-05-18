using System.Collections;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    public CanvasGroup deathGroup;
    public CanvasGroup dialogueBoxGroup;
    public DialogueManager dialogueManager;
    public GameObject wordPanel;
    [TextArea(2, 5)]
    public string[] introLines;

    public IEnumerator RunIntro(GameManager gameManager)
    {
        //wordPanel.SetActive(false);

        yield return StartCoroutine(FadeIn(deathGroup, 1f));
        //yield return StartCoroutine(FadeIn(dialogueBoxGroup, 1f));

        foreach (string line in introLines)
        {
            dialogueManager.ShowDialogue(line);
            yield return new WaitForSeconds(line.Length * 0.05f + 1.5f);
        }

        //wordPanel.SetActive(true);
        gameManager.StartNewGame();
    }

    IEnumerator FadeIn(CanvasGroup group, float duration)
    {
        float t = 0f;
        group.alpha = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }

        group.alpha = 1f;
    }
}
