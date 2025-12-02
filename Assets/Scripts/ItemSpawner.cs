using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawn")]
    public string[] itemTags;
    public float spawnInterval = 5f;
    public float spawnRadius = 5f;

    private float timer;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandomItem();
            timer = 0;
        }
    }

    void SpawnRandomItem()
    {
        // 1. Pilih item secara acak
        int randomIndex = Random.Range(0, itemTags.Length);
        string tagToSpawn = itemTags[randomIndex];

        // 2. Tentukan posisi acak di sekitar pemain
        Vector2 randomPos = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + (Vector3)randomPos;

        // 3. Spawn dari Object Pooler
        ObjectPooler.instance.SpawnFromPool(tagToSpawn, spawnPos, Quaternion.identity);
    }
}