using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public void LoadNextScene()
    {
        SoundManager.PlaySound(SoundType.BUTTON);
        SceneManager.LoadScene(sceneToLoad);
    }
}
