using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate ;
    public float spawnSafeDistance;

    private Camera mainCam;
    private Transform player;
    private float timer; // Timer manual kita

    void Start()
    {
        mainCam = Camera.main;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Kita tidak lagi menggunakan InvokeRepeating di sini
    }

    // Kita pindahkan logika ke Update
    void Update()
    {
        // Tambahkan waktu yang berlalu ke timer
        timer += Time.deltaTime;

        // Jika timer sudah mencapai atau melebihi spawnRate
        if (timer >= spawnRate)
        {
            SpawnEnemy(); // Panggil fungsi spawn
            timer = 0; // Reset timer kembali ke nol
        }
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        float camHeight = mainCam.orthographicSize;
        float camWidth = mainCam.orthographicSize * mainCam.aspect;

        Vector2 spawnPos;
        int attempts = 0;

        do
        {
            float spawnX = Random.Range(mainCam.transform.position.x - camWidth - 2, mainCam.transform.position.x + camWidth + 2);
            float spawnY = Random.Range(mainCam.transform.position.y - camHeight - 2, mainCam.transform.position.y + camHeight + 2);
            spawnPos = new Vector2(spawnX, spawnY);
            attempts++;
            if (attempts > 50)
            {
                return;
            }
        }
        while (Vector2.Distance(spawnPos, player.position) < spawnSafeDistance);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}