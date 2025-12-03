using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawn")]
    public string[] itemTags;

    [Header("Waktu Spawn (Acak)")]
    public float minSpawnInterval = 10f; // Paling cepat muncul
    public float maxSpawnInterval = 15f; // Paling lambat muncul

    [Header("Posisi Spawn (Acak)")]
    public float minDistance = 3f;  // Jarak minimal dari pemain (biar gak spawn di muka)
    public float maxDistance = 7f; // Jarak maksimal

    private float timer;
    private float nextSpawnTime;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Set waktu spawn pertama
        SetNextSpawnTime();
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        // Cek apakah sudah waktunya spawn
        if (timer >= nextSpawnTime)
        {
            SpawnRandomItem();
            timer = 0; // Reset timer
            SetNextSpawnTime(); // Acak waktu untuk spawn berikutnya
        }
    }

    void SetNextSpawnTime()
    {
        // Tentukan durasi acak untuk spawn berikutnya
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void SpawnRandomItem()
    {
        if (itemTags.Length == 0) return;

        // 1. Pilih item acak
        int randomIndex = Random.Range(0, itemTags.Length);
        string tagToSpawn = itemTags[randomIndex];

        // 2. Tentukan POSISI yang benar-benar acak (Donat Shape)
        // Arah acak (360 derajat)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        // Jarak acak (antara min dan max)
        float randomDistance = Random.Range(minDistance, maxDistance);

        // Gabungkan Arah * Jarak
        Vector3 spawnOffset = randomDirection * randomDistance;
        Vector3 spawnPos = player.position + spawnOffset;

        // 3. Spawn dari Pool
        ObjectPooler.instance.SpawnFromPool(tagToSpawn, spawnPos, Quaternion.identity);
    }
}