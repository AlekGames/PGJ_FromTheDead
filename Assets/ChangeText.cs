using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void changeText(string newText)
    {
        text.text = newText;
    }
}
