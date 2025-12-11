using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    // Singleton agar mudah diakses oleh HUD
    public static AIDirector instance;

    [Header("Pengaturan Game Loop")]
    public float gameDuration = 10f; // Durasi game dalam detik (misal 300 = 5 menit)

    [Header("Status Game")]
    public float gameTime = 0f;       // Waktu yang sudah berjalan
    public bool bossSpawned = false;  // Penanda apakah Boss sudah muncul

    // --- PENGATURAN LAMA (Stress & Wave) ---
    [Header("Pengaturan Intensitas")]
    public float damageStressAmount = 10f;
    public float killReliefAmount = 2f;
    public float stressDecayRate = 1.5f;

    [Header("Pengaturan Spawn")]
    public float baseSpawnInterval = 1f;
    public float minSpawnInterval = 0.2f;
    public float maxSpawnInterval = 3f;

    [Header("Pengaturan Ritme")]
    public float baseWaveDuration = 20f;
    public float baseRestDuration = 8f;

    // --- VARIABEL INTERNAL ---
    private float currentStress = 0f;
    private float stateTimer;
    private float spawnTimer;
    private DirectorState currentState;

    private Transform playerTransform;
    private PlayerExperience playerExperience; // Referensi untuk cek level pemain

    private enum DirectorState
    {
        Spawning,
        Resting
    }

    void Awake()
    {
        // Setup Singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Cari Player & Komponennya
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerExperience = playerObject.GetComponent<PlayerExperience>();
        }

        // Setup awal state director
        currentState = DirectorState.Resting;
        stateTimer = baseRestDuration;
    }

    // --- EVENT HANDLERS (Sistem Stress) ---
    void OnEnable()
    {
        HealthSystem.OnPlayerDamaged += HandlePlayerDamaged;
        HealthSystem.OnEnemyKilled += HandleEnemyKilled;
    }
    void OnDisable()
    {
        HealthSystem.OnPlayerDamaged -= HandlePlayerDamaged;
        HealthSystem.OnEnemyKilled -= HandleEnemyKilled;
    }
    void HandlePlayerDamaged(float damageAmount)
    {
        currentStress += damageStressAmount;
    }
    void HandleEnemyKilled()
    {
        currentStress -= killReliefAmount;
    }

    // --- UPDATE LOOP ---
    void Update()
    {
        // 1. CEK APAKAH BOSS SUDAH ADA?
        // Jika Boss sudah muncul, Director berhenti kerja (tidak spawn musuh kecil lagi)
        if (bossSpawned) return;

        // 2. HITUNG WAKTU MUNDUR
        gameTime += Time.deltaTime;

        // 3. CEK JIKA WAKTU HABIS -> SPAWN BOSS
        if (gameTime >= gameDuration)
        {
            SpawnBoss();
            return; // Keluar dari fungsi Update agar kode di bawah tidak dijalankan
        }

        // --- JIKA BELUM BOSS, JALANKAN LOGIKA NORMAL (WAVES) ---

        // A. Pulihkan Stres (Decay)
        if (currentStress != 0)
        {
            float decay = stressDecayRate * Time.deltaTime;
            if (currentStress > 0)
                currentStress = Mathf.Max(0, currentStress - decay);
            else
                currentStress = Mathf.Min(0, currentStress + decay);
        }

        // B. Logika State Machine (Pindah Fase Resting <-> Spawning)
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            if (currentState == DirectorState.Spawning)
            {
                currentState = DirectorState.Resting;
                // Durasi istirahat dipengaruhi stres (makin stres, makin lama istirahat)
                stateTimer = baseRestDuration + (currentStress * 0.1f);
                stateTimer = Mathf.Clamp(stateTimer, baseRestDuration * 0.5f, baseRestDuration * 2f);
            }
            else if (currentState == DirectorState.Resting)
            {
                currentState = DirectorState.Spawning;
                // Durasi wave dipengaruhi stres (makin stres, wave makin sebentar)
                stateTimer = baseWaveDuration - (currentStress * 0.2f);
                stateTimer = Mathf.Clamp(stateTimer, baseWaveDuration * 0.5f, baseWaveDuration * 2f);
            }
        }

        // C. Logika Spawning Musuh
        if (currentState == DirectorState.Spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy(); // Panggil fungsi spawn musuh biasa

                // Hitung interval spawn berikutnya berdasarkan stres
                float spawnInterval = baseSpawnInterval + (currentStress * 0.05f);
                spawnTimer = Mathf.Clamp(spawnInterval, minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    // --- LOGIKA SPAWN MUSUH BIASA ---
    void SpawnEnemy()
    {
        if (playerTransform == null) return;

        // Ambil level player
        int playerLevel = 1;
        if (playerExperience != null)
        {
            playerLevel = playerExperience.currentLevel;
        }

        // --- Komposisi Wave Dinamis (Berdasarkan Level & Stress) ---
        List<string> spawnPool = new List<string>();

        // 1. Musuh Dasar (Selalu ada)
        spawnPool.Add("TimeAnomaly");
        spawnPool.Add("TimeAnomaly");
        spawnPool.Add("TimeAnomaly");

        // 2. Musuh Cepat (Kalau pemain jago / stress rendah)
        if (currentStress < 0)
        {
            spawnPool.Add("ChronoHound");
            spawnPool.Add("ChronoHound");
        }

        // 3. Musuh Ranged (Mulai Level 2)
        if (playerLevel >= 2)
        {
            spawnPool.Add("TemporalWeaver");
        }

        // 4. Musuh Tank (Mulai Level 4, asalkan tidak terlalu stress)
        if (playerLevel >= 4 && currentStress > -10)
        {
            spawnPool.Add("VoidBehemoth");
        }

        // 5. Musuh Elite (Mulai Level 5 & Pemain Jago Banget)
        if (playerLevel >= 5 && currentStress < -20)
        {
            spawnPool.Add("EliteAnomaly");
        }

        // Pilih musuh random dari pool
        string enemyTagToSpawn = spawnPool[Random.Range(0, spawnPool.Count)];

        // Tentukan posisi spawn (Random di sekitar player)
        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle.normalized * 15f;

        // Spawn dari Pool
        ObjectPooler.instance.SpawnFromPool(enemyTagToSpawn, spawnPos, Quaternion.identity);
    }

    // --- LOGIKA SPAWN BOSS (BARU) ---
    void SpawnBoss()
    {
        bossSpawned = true;
        Debug.Log("[!] WAKTU HABIS! FINAL BOSS MUNCUL! [!]");

        // 1. Hapus/Matikan semua musuh kecil yang tersisa biar duel adil (Opsional)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            e.SetActive(false);
        }

        // 2. Spawn Boss di posisi tetap (misal di atas player)
        if (playerTransform != null)
        {
            Vector2 bossSpawnPos = (Vector2)playerTransform.position + new Vector2(0, 10f);

            // Pastikan Tag di ObjectPooler adalah "FinalBoss"
            ObjectPooler.instance.SpawnFromPool("FinalBoss", bossSpawnPos, Quaternion.identity);
        }
    }

    // --- HELPER UNTUK HUD ---
    public float GetTimeLeft()
    {
        // Mengembalikan sisa waktu (minimal 0)
        return Mathf.Max(0, gameDuration - gameTime);
    }
}