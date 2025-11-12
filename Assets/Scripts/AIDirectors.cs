using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    // --- Pengaturan (Sama seperti sebelumnya) ---
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

    // --- Variabel Internal ---
    private float currentStress = 0f;
    private float stateTimer;
    private float spawnTimer;
    private DirectorState currentState;
    private Transform playerTransform;

    // --- PERBAIKAN BUG #1 ---
    private PlayerExperience playerExperience; // Script buat ngecek level player
    // --- AKHIR PERBAIKAN ---

    private enum DirectorState
    {
        Spawning,
        Resting
    }

    void Start()
    {
        // Cari Player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;

            // --- PERBAIKAN BUG #1 ---
            // Ambil script experience-nya dari Player
            playerExperience = playerObject.GetComponent<PlayerExperience>();
            // --- AKHIR PERBAIKAN ---
        }

        currentState = DirectorState.Resting;
        stateTimer = baseRestDuration;
    }

    // --- Event Handlers (Sama seperti sebelumnya) ---
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
    void HandlePlayerDamaged(int damageAmount)
    {
        currentStress += damageStressAmount;
    }
    void HandleEnemyKilled()
    {
        currentStress -= killReliefAmount;
    }

    // --- Logika Update (Sama seperti sebelumnya) ---
    void Update()
    {
        // 1. Pulihkan Stres (Decay to 0)
        if (currentStress != 0)
        {
            float decay = stressDecayRate * Time.deltaTime;
            if (currentStress > 0)
                currentStress = Mathf.Max(0, currentStress - decay);
            else
                currentStress = Mathf.Min(0, currentStress + decay);
        }

        // 2. Logika State Machine (Ritme)
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            if (currentState == DirectorState.Spawning)
            {
                currentState = DirectorState.Resting;
                stateTimer = baseRestDuration + (currentStress * 0.1f);
                stateTimer = Mathf.Clamp(stateTimer, baseRestDuration * 0.5f, baseRestDuration * 2f);
            }
            else if (currentState == DirectorState.Resting)
            {
                currentState = DirectorState.Spawning;
                stateTimer = baseWaveDuration - (currentStress * 0.2f);
                stateTimer = Mathf.Clamp(stateTimer, baseWaveDuration * 0.5f, baseWaveDuration * 2f);
            }
        }

        // 3. Logika Spawning
        if (currentState == DirectorState.Spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy(); // Panggil fungsi spawn yang baru

                float spawnInterval = baseSpawnInterval + (currentStress * 0.05f);
                spawnTimer = Mathf.Clamp(spawnInterval, minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    // --- PERBAIKAN BUG #2 ---
    void SpawnEnemy()
    {
        if (playerTransform == null) return;

        // Ambil level player (dengan perbaikan Bug #1)
        int playerLevel = 1;
        if (playerExperience != null)
        {
            playerLevel = playerExperience.currentLevel;
        }

        // --- Logika Probabilitas Dinamis ---
        // Kita nggak pake 'if-else' yang kaku lagi. Kita pake 'Weighted Random'.
        // Ini bikin komposisi gelombang (Wave Composition) lebih mantep.

        List<string> spawnPool = new List<string>();

        // 1. Musuh Dasar (Melee) - Selalu ada
        spawnPool.Add("TimeAnomaly");
        spawnPool.Add("TimeAnomaly");
        spawnPool.Add("TimeAnomaly");

        // 2. Musuh Cepat (Rusher)
        if (currentStress < 0) 
        {
            spawnPool.Add("ChronoHound");
            spawnPool.Add("ChronoHound");
        }

        // 3. Musuh Jarak Jauh (Ranged)
        if (playerLevel >= 2) // Muncul setelah 1x upgrade
        {
            spawnPool.Add("TemporalWeaver");
        }

        // 4. Musuh Tank
        if (playerLevel >= 4 && currentStress > -10)
        {
            spawnPool.Add("VoidBehemoth");
        }

        // 5. Musuh Elite
        if (playerLevel >= 5 && currentStress < -20) // Muncul kalo pemain jago & level tinggi
        {
            spawnPool.Add("EliteAnomaly");
        }

        // --- Akhir Logika ---

        // Pilih musuh secara acak dari 'spawnPool' yang udah kita buat
        string enemyTagToSpawn = spawnPool[Random.Range(0, spawnPool.Count)];

        // Tentukan posisi spawn
        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle.normalized * 15f;

        // Panggil dari Object Pooler
        ObjectPooler.instance.SpawnFromPool(enemyTagToSpawn, spawnPos, Quaternion.identity);
    }
}