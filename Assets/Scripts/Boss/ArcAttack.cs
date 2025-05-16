using UnityEngine;

public class ArcAttack : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab;
    public int numberOfPrefabs = 8;
    public float radius = 5f;

    void SpawnPrefabsInCircle()
    {
        if (prefab == null || numberOfPrefabs <= 0) return;

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfPrefabs;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Vector3 spawnPos = transform.position + direction * radius;

            // Rotate to face outward (away from center)
            Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 180);

            Instantiate(prefab, spawnPos, rotation);
        }
    }
}
