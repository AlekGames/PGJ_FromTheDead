using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public GameObject letterOrbPrefab;

    public GameObject letterSlotPrefab;
    public Transform letterContainer;


    [System.Serializable]
    public class WordWithHint
    {
        public string word;
        public string hint;
    }

    public WordWithHint[] wordsWithHints;

    // Données internes
    private WordWithHint currentWord;
    private string wordToGuess;
    private char[] currentGuess;
    private int attemptsLeft = 3;

    public static int difficultyLevel = 1;

    private bool isGameOver = false;

    private int ghostStep = 0;
  
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    public AudioClip gameStart;
    private AudioSource audioSource;

    public TimerManager timerManager;

    public DialogueManager dialogueManager;

    public IntroSequence introSequence;

    public AudioClip tickingSound;
    private AudioSource tickingSource;

    public UIScreenShake screenShake;

    public Transform grandmaTarget;
    public Animator grandmaAnimator;
    void Start()
    {
       
        audioSource = GetComponent<AudioSource>();
        dialogueManager.dialogueText.text = "";

        if (difficultyLevel == 1) {
        StartCoroutine(introSequence.RunIntro(this));
        }

       
        tickingSource = gameObject.AddComponent<AudioSource>();
        tickingSource.loop = true;
        tickingSource.playOnAwake = false;
        tickingSource.clip = tickingSound;


    }


 
    public void StartNewGame()
    {
       


       if (audioSource != null && gameStart != null)
        {
            audioSource.PlayOneShot(gameStart);
        }

       
        // Choisir un mot aléatoire
        currentWord = wordsWithHints[Random.Range(0, wordsWithHints.Length)];
        wordToGuess = currentWord.word.ToUpper();
        currentGuess = wordToGuess.ToCharArray();

        // Masquer toutes les lettres au départ
        for (int i = 0; i < currentGuess.Length; i++)
            currentGuess[i] = '_';

        // Nombre de lettres à révéler selon le niveau
        int lettersToReveal = wordToGuess.Length - GetLettersToGuessCount();

        // Choisir aléatoirement quelles lettres seront révélées
        System.Collections.Generic.List<int> indexes = new System.Collections.Generic.List<int>();
        for (int i = 0; i < wordToGuess.Length; i++) indexes.Add(i);

        for (int i = 0; i < lettersToReveal; i++)
        {
            int randomIndex = Random.Range(0, indexes.Count);
            int revealIndex = indexes[randomIndex];
            indexes.RemoveAt(randomIndex);

            currentGuess[revealIndex] = wordToGuess[revealIndex];
        }


      
        /*if (hintText != null)
            hintText.text = $"{currentWord.hint}";
        */
        dialogueManager.ShowDialogue(currentWord.hint);

        
        Debug.Log("Mot à deviner : " + wordToGuess);
        Debug.Log("indice : " + currentWord.hint);
        Debug.Log("Difficulté : " + difficultyLevel);
        Debug.Log("Lettres à deviner : " + GetLettersToGuessCount() + " sur " + wordToGuess.Length);

        if (tickingSource != null && tickingSound != null)
         tickingSource.Play();

        GenerateLetterSlots();
        StartCoroutine(DelayedUpdateDisplayedWord());
        timerManager.StartTimer();
    }

    int GetLettersToGuessCount()
    {
        int target = 0;

        switch (difficultyLevel)
        {
            case 1: target = 2; break;
            case 2: target = 3; break;
            case 3: target = 4; break;
            case 4: target = 5; break;
            default: target = 5; break;
        }

        return Mathf.Min(target, wordToGuess.Length);
    }



    void GenerateLetterSlots()
    {
        // Nettoyer les anciens slots
        foreach (Transform child in letterContainer)
        {
            Destroy(child.gameObject);
        }

        // Créer un slot par lettre
        for (int i = 0; i < wordToGuess.Length; i++)
        {
        GameObject orb = Instantiate(letterOrbPrefab, letterContainer);
        TextMeshProUGUI text = orb.GetComponentInChildren<TextMeshProUGUI>();
        text.text = currentGuess[i].ToString();

        }
    }

    void UpdateDisplayedWord()
    {
        for (int i = 0; i < letterContainer.childCount; i++)
        {
            TextMeshProUGUI text = letterContainer.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();

            string currentLetter = text.text;
            string newLetter = currentGuess[i].ToString();

            if (currentLetter == "_" && newLetter != "_")
            {
                StartCoroutine(FadeInLetter(text, newLetter));
            }
            else
            {
                text.text = newLetter;
            }
        }
    }


    System.Collections.IEnumerator DelayedUpdateDisplayedWord()
    {
        yield return null; // attendre une frame

        UpdateDisplayedWord();
    }


    public void TryLetter(char letter)
    {
        bool found = false;

        for (int i = 0; i < wordToGuess.Length; i++)
        {
            if (char.ToUpper(wordToGuess[i]) == char.ToUpper(letter))
            {
                currentGuess[i] = wordToGuess[i];
                found = true;
            }
        }

        if (found)
        {
            audioSource.PlayOneShot(correctSound);
        }
        else
        {
            attemptsLeft--;

            if (screenShake != null)
                Debug.Log("Screen shake lancé !");
                screenShake.Shake();


            audioSource.PlayOneShot(incorrectSound);
            Debug.Log("Lettre incorrecte. Essais restants : " + attemptsLeft);
            MoveGhostUp();
        }

        UpdateDisplayedWord();
        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        if (new string(currentGuess) == wordToGuess)
        {
            Debug.Log("Gagné !");

        foreach (Transform child in letterContainer)
        {
            OrbPopEffect popEffect = child.GetComponent<OrbPopEffect>();
            if (popEffect != null)
            {
                popEffect.Pop(grandmaTarget);
            }
        }
            grandmaAnimator.SetTrigger("Win");

            
            // Incrémenter la difficulté (max = 4)
            if (difficultyLevel < 4)
                difficultyLevel++;

            // Lancer un nouveau mot avec difficulté augmentée
            //Invoke("StartNewGame", 2f);
            dialogueManager.ShowDialogue("come back from the dead now...");
            StartCoroutine(ShowWinScreen());  
        }

        else if (attemptsLeft <= 0)
        {
            Debug.Log("Perdu !");
            StartCoroutine(ShowGameOver());
        }
    }

    void Update()
    {
        
        if (isGameOver)
            return;
        
        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                TryLetter(c);
            }
        }
    }

    private System.Collections.IEnumerator ShowWinScreen()
    {

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("WinScreen");
    }

    private System.Collections.IEnumerator ShowGameOver()
    {
        isGameOver = true;
        
        string GameOverText = "YOU LOST";

        foreach (Transform child in letterContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (char c in GameOverText)
        {
            GameObject slot = Instantiate(letterSlotPrefab, letterContainer);
            TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();
            text.text = c == ' ' ? " " : c.ToString();
        }

        //hintText.text = "you really died now..";
        dialogueManager.ShowDialogue("you really died now..");

        if (tickingSource != null)
        tickingSource.Stop();


        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("LoseScreen");

    }

    void MoveGhostUp()
    {
        ghostStep++;
/*
        switch (ghostStep)
        {
            case 1:
                ghostAnimator.SetTrigger("Shake1");
                break;
            case 2:
                ghostAnimator.SetTrigger("Shake2");
                break;
            case 3:
                ghostAnimator.SetTrigger("Shake3");
                break;
        }*/
        /*

        if (ghostParticles != null)
        {
            ghostParticles.transform.position = ghostTransform.position - new Vector3(0, 0f, 1f);
            ghostParticles.Stop();
            ghostParticles.Clear();
            ghostParticles.Play();
            StartCoroutine(StopGhostParticlesAfter(0.6f));
        }
        */

    }

    public void TriggerGameOverByTimer()
    {
        Debug.Log("GAME OVER: Temps écoulé !");
        ShowGameOver();
    }

    /*System.Collections.IEnumerator StopGhostParticlesAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ghostParticles != null)
            ghostParticles.Stop();
    }
    */

    System.Collections.IEnumerator FadeInLetter(TextMeshProUGUI text, string newLetter)
    {
    text.text = newLetter;
    text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);

    float duration = 0.4f;
    float elapsed = 0f;

    Vector3 originalScale = text.transform.localScale;
    text.transform.localScale = originalScale * 1.3f; 

    while (elapsed < duration)
    {
        float t = elapsed / duration;

        // Fondu alpha
        float alpha = Mathf.Lerp(0f, 1f, t);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        // Scale vers normal
        text.transform.localScale = Vector3.Lerp(originalScale * 1.3f, originalScale, t);

        elapsed += Time.deltaTime;
        yield return null;
    }

    text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
    text.transform.localScale = originalScale;
    }

}
