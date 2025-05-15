using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Références")]
    public WordLibrary wordLibrary;
    public GameObject letterSlotPrefab;
    public Transform letterContainer;

    private string wordToGuess;
    private char[] currentGuess;
    private int attemptsLeft = 3;

    void Start()
    {
        StartNewGame();
    }

    void StartNewGame()
    {
        wordToGuess = wordLibrary.GetRandomWord();
        Debug.Log("mot à deviner : " + wordToGuess);
        currentGuess = new string('_', wordToGuess.Length).ToCharArray();
        GenerateLetterSlots();
        UpdateDisplayedWord();
    }

    void GenerateLetterSlots()
    {
        // Supprimer les anciens slots
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
            text.text = currentGuess[i].ToString();
        }
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
            Debug.Log("Lettre incorrecte. Essais restants : " + attemptsLeft);
        }

        UpdateDisplayedWord();

        if (new string(currentGuess) == wordToGuess)
        {
            Debug.Log("Gagné !");
        }
        else if (attemptsLeft <= 0)
        {
            Debug.Log("Perdu !");
        }
    }

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                TryLetter(c);
            }
        }
    }
}
