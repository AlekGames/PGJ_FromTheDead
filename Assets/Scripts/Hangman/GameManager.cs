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

    void Start()
    {
        StartNewGame();
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
            hintText.text = $"\"{currentWord.hint}\"";

        Debug.Log("Mot à deviner : " + wordToGuess);
        Debug.Log("indice : " + currentWord.hint);
        Debug.Log("Difficulté : " + difficultyLevel);
        Debug.Log("Lettres à deviner : " + GetLettersToGuessCount() + " sur " + wordToGuess.Length);

        // Générer les lettres à l’écran
        GenerateLetterSlots();
        StartCoroutine(DelayedUpdateDisplayedWord());
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
    int count = Mathf.Min(letterContainer.childCount, currentGuess.Length);

    for (int i = 0; i < count; i++)
    {
        TextMeshProUGUI text = letterContainer.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
        text.text = currentGuess[i].ToString();
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

        if (!found)
        {
            attemptsLeft--;
            Debug.Log($"Lettre incorrecte. Essais restants : {attemptsLeft}");
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
            Invoke("StartNewGame", 0.5f); // ou 1f pour une petite pause
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
}
