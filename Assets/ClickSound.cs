using UnityEngine;

public class ClickSound : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.PlaySound(SoundType.CLICK);
        }
    }
}
