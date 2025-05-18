using UnityEngine;

public class UIScreenShake : MonoBehaviour
{
    public float duration = 0.1f;
    public float magnitude = 5f;

    private Vector3 originalPos;
 
    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    System.Collections.IEnumerator ShakeRoutine()
    {
        originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
