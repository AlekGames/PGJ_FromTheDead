using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
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

    private int difficultyLevel = 1;

    private bool isGameOver = false;

    public Transform ghostTransform;

    private int ghostStep = 0;

    public Animator ghostAnimator;

    public ParticleSystem ghostParticles;

    public AudioClip correctSound;
    public AudioClip incorrectSound;
    private AudioSource audioSource;

    public TimerManager timerManager;

    void Start()
    {
        StartNewGame();
        audioSource = GetComponent<AudioSource>();


    }

    void StartNewGame()
    {
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


        // Afficher l’indice
        if (hintText != null)
            hintText.text = $"{currentWord.hint}";

        Debug.Log("Mot à deviner : " + wordToGuess);
        Debug.Log("indice : " + currentWord.hint);
        Debug.Log("Difficulté : " + difficultyLevel);
        Debug.Log("Lettres à deviner : " + GetLettersToGuessCount() + " sur " + wordToGuess.Length);

        // Générer les lettres à l’écran
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
            Instantiate(letterSlotPrefab, letterContainer);
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
            
            // Incrémenter la difficulté (max = 4)
            if (difficultyLevel < 4)
                difficultyLevel++;

            // Lancer un nouveau mot avec difficulté augmentée
            Invoke("StartNewGame", 1f); 
        }

        else if (attemptsLeft <= 0)
        {
            Debug.Log("Perdu !");
            ShowGameOver();
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

    void ShowGameOver()
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

        hintText.text = "you really died now..";

    }

    void MoveGhostUp()
    {
        ghostStep++;

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
        }

        if (ghostParticles != null)
        {
            ghostParticles.transform.position = ghostTransform.position;
            ghostParticles.Stop();
            ghostParticles.Clear();
            ghostParticles.Play();
            StartCoroutine(StopGhostParticlesAfter(0.6f));
        }
    }

    public void TriggerGameOverByTimer()
    {
        Debug.Log("GAME OVER: Temps écoulé !");
        ShowGameOver();
    }

    System.Collections.IEnumerator StopGhostParticlesAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ghostParticles != null)
            ghostParticles.Stop();
    }

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
