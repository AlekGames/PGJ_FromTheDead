using UnityEngine;

public class WordLibrary : MonoBehaviour
{
    public string[] words = { "COOKIE", "UNITY", "GAMING", "SCRIPT", "EDITOR", "PLAYER" };

    public string GetRandomWord()
    {
        int index = Random.Range(0, words.Length);
        return words[index].ToUpper(); // pour éviter les confusions de casse
    }
}
